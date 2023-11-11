using Microsoft.AspNetCore.Mvc;
using SoowGoodWeb.SslCommerzData;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
//using Volo.Abp.AspNetCore.Mvc;
using SoowGoodWeb.Interfaces;
using Microsoft.AspNetCore.Http;
using Polly;
using System.IO.Pipelines;
using System.Buffers;

namespace SoowGoodWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class PaymentController : Controller
    {
        private readonly ISslCommerzService _sslCommerzAppService;
        private readonly SslCommerzGatewayConfiguration _configuration;

        public PaymentController(ISslCommerzService sslCommerzAppService,
                                    SslCommerzGatewayConfiguration configuration)
        {
            _sslCommerzAppService = sslCommerzAppService;
            _configuration = configuration;
        }

        /// <summary>
        /// SSLCOMMERZ LIVE
        /// </summary>
        /// <returns></returns>
        [HttpPost]//, ActionName("PaymentSuccess")]
        [Route("/api/services/payment-success")]
        public async Task<IActionResult> SuccessPaymentAsync()
        {
            await CompletePaymentProcess();

            return new RedirectResult(_configuration.ProdSuccessClientUrl);
        }

        [HttpPost]//, ActionName("PaymentFailed")]
        [Route("/api/services/payment-fail")]
        public async Task<IActionResult> FailedPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.ProdFailClientUrl);
        }

        [HttpPost]//, ActionName("PaymentCancelled")]
        [Route("/api/services/payment-cancel")]
        public async Task<IActionResult> CancelledPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.ProdCancelClientUrl);
        }

        //****************SSLCOMMERZ LIVE*************//


        /// <summary>
        /// SSLCOMMERZ SANDBOX
        /// </summary>
        /// <returns></returns>
        [HttpPost]//, ActionName("TestPaymentSuccess")]
        [Route("/api/services/test/payment-success")]
        public async Task<IActionResult> SuccessTestPaymentAsync()
        {
            await CompleteTestPaymentProcess();

            return new RedirectResult(_configuration.DevSuccessClientUrl);
        }

        [HttpPost]//, ActionName("TestPaymentFailed")]
        [Route("/api/services/test/payment-fail")]
        public async Task<IActionResult> FailedTestPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.DevFailClientUrl);
        }

        [HttpPost]//, ActionName("TestPaymentCancelled")]
        [Route("/api/services/test/payment-cancel")]
        public async Task<IActionResult> CancelledTestPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.DevCancelClientUrl);
        }

        //****************SSLCOMMERZ SANDBOX*************//

        private async Task CompletePaymentProcess(bool continueWithValidationCheck = false)
        {
            try
            {
                var sslCommerzResponseDic = await MapSslCommerzResponse();

                if (continueWithValidationCheck)
                {
                    var validationResult = await _sslCommerzAppService.ValidateTransactionAsync(sslCommerzResponseDic);

                    if ((bool)validationResult.IsValidTransaction)
                    {
                        await _sslCommerzAppService.UpdatePaymentHistory(sslCommerzResponseDic);

                        await _sslCommerzAppService.UpdateApplicantPaymentStatus(sslCommerzResponseDic);
                    }
                    else
                    {
                        Console.WriteLine($"Transaction Validation Status: {validationResult.Message}");
                    }
                }
                else
                {
                    await _sslCommerzAppService.UpdatePaymentHistory(sslCommerzResponseDic);

                    await _sslCommerzAppService.UpdateApplicantPaymentStatus(sslCommerzResponseDic);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task CompleteTestPaymentProcess(bool continueWithValidationCheck = false)
        {
            try
            {
                var sslCommerzResponseDic = await MapSslCommerzResponse();

                if (continueWithValidationCheck)
                {
                    var validationResult = await _sslCommerzAppService.ValidateTestTransactionAsync(sslCommerzResponseDic);

                    if ((bool)validationResult.IsValidTransaction)
                    {
                        await _sslCommerzAppService.UpdatePaymentHistory(sslCommerzResponseDic);

                        await _sslCommerzAppService.UpdateApplicantPaymentStatus(sslCommerzResponseDic);
                    }
                    else
                    {
                        Console.WriteLine($"Transaction Validation Status: {validationResult.Message}");
                    }
                }
                else
                {
                    await _sslCommerzAppService.UpdatePaymentHistory(sslCommerzResponseDic);

                    await _sslCommerzAppService.UpdateApplicantPaymentStatus(sslCommerzResponseDic);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task UpdatePaymentHistory()
        {
            try
            {
                var sslCommerzResponseDic = await MapSslCommerzResponse();

                await _sslCommerzAppService.UpdatePaymentHistory(sslCommerzResponseDic);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<Dictionary<string, string>> MapSslCommerzResponse()
        {
            //byte[] bytes = await Request.Body.GetAllBytesAsync();//.GetAllBytes();//.Body(Request.TotalBytes);
            //string s = Encoding.UTF8.GetString(bytes);

            var code = HttpContext.Response.StatusCode;
            var request = Request;
            var type = request.ContentType;


            string strInfoBody = string.Empty;
            bool infoBody = request.ContentLength > 0;
            if (infoBody)
            {
                request.EnableBuffering();
                request.Body.Position = 0;
                List<string> tmp = await GetListOfStringFromPipe(request.BodyReader);
                request.Body.Position = 0;

                strInfoBody = string.Concat("\r\nBody: ", string.Join("", tmp.ToArray()));
            }

            //request.EnableBuffering();
            //var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            //var a =request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var r = new StreamReader(request.Body.ToString()).ReadToEnd();

            var sslCommerzResponseDic = new Dictionary<string, string>();
            //var reader = new StreamReader(Request.Body, Encoding.UTF8);
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var result = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var keyValuePairs = result.Split('&');
                    foreach (var keyValuePair in keyValuePairs)
                    {
                        var keyValues = keyValuePair.Split('=');
                        if (!sslCommerzResponseDic.ContainsKey(keyValues[0]))
                        {
                            sslCommerzResponseDic.Add(keyValues[0], keyValues[1]);
                        }
                    }
                }
            }

            return sslCommerzResponseDic;
        }

        private async Task<List<string>> GetListOfStringFromPipe(PipeReader reader)
        {
            List<string> results = new List<string>();

            while (true)
            {
                ReadResult readResult = await reader.ReadAsync();
                var buffer = readResult.Buffer;

                SequencePosition? position = null;

                do
                {
                    // Look for a EOL in the buffer
                    position = buffer.PositionOf((byte)'\n');

                    if (position != null)
                    {
                        var readOnlySequence = buffer.Slice(0, position.Value);
                        AddStringToList(results, in readOnlySequence);

                        // Skip the line + the \n character (basically position)
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }
                }
                while (position != null);


                if (readResult.IsCompleted && buffer.Length > 0)
                {
                    AddStringToList(results, in buffer);
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                // At this point, buffer will be updated to point one byte after the last
                // \n character.
                if (readResult.IsCompleted)
                {
                    break;
                }
            }

            return results;
        }
        private static void AddStringToList(List<string> results, in ReadOnlySequence<byte> readOnlySequence)
        {
            // Separate method because Span/ReadOnlySpan cannot be used in async methods
            ReadOnlySpan<byte> span = readOnlySequence.IsSingleSegment ? readOnlySequence.First.Span : readOnlySequence.ToArray().AsSpan();
            results.Add(Encoding.UTF8.GetString(span));
        }
    }
}
