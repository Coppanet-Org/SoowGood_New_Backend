using Microsoft.AspNetCore.Mvc;
using SoowGoodWeb.SslCommerzData;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Mvc;
using SoowGoodWeb.Interfaces;

namespace SoowGoodWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class PaymentServiceController : AbpController
    {
        private readonly ISslCommerzService _sslCommerzAppService;
        private readonly SslCommerzGatewayConfiguration _configuration;

        public PaymentServiceController(ISslCommerzService sslCommerzAppService,
                                    SslCommerzGatewayConfiguration configuration)
        {
            _sslCommerzAppService = sslCommerzAppService;
            _configuration = configuration;
        }

        /// <summary>
        /// SSLCOMMERZ LIVE
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/services/payment-success")]
        public async Task<RedirectResult> SuccessPaymentAsync()
        {
            await CompletePaymentProcess();

            return new RedirectResult(_configuration.ProdSuccessClientUrl);
        }

        [HttpPost]
        [Route("/api/services/payment-fail")]
        public async Task<RedirectResult> FailedPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.ProdFailClientUrl);
        }

        [HttpPost]
        [Route("/api/services/payment-cancel")]
        public async Task<RedirectResult> CancelledPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.ProdCancelClientUrl);
        }

        //****************SSLCOMMERZ LIVE*************//


        /// <summary>
        /// SSLCOMMERZ SANDBOX
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/services/test/payment-success")]
        public async Task<RedirectResult> SuccessTestPaymentAsync()
        {
            await CompleteTestPaymentProcess();

            return new RedirectResult(_configuration.DevSuccessClientUrl);
        }

        [HttpPost]
        [Route("/api/services/test/payment-fail")]
        public async Task<RedirectResult> FailedTestPaymentAsync()
        {
            await UpdatePaymentHistory();

            return new RedirectResult(_configuration.DevFailClientUrl);
        }

        [HttpPost]
        [Route("/api/services/test/payment-cancel")]
        public async Task<RedirectResult> CancelledTestPaymentAsync()
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
            var sslCommerzResponseDic = new Dictionary<string, string>();
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
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
    }
}
