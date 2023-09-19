using SoowGoodWeb.DtoModels;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace SoowGoodWeb.PaymentsModels.SslCommerz
{
    public class SslCommerzGatewayManager : ApplicationService, ITransientDependency
    {
        string liveStoreId = PaymentGatewayHelper.LiveStoreId;
        string liveStorePassword = PaymentGatewayHelper.LiveStorePassword;
        string liveSubmitUrl = PaymentGatewayHelper.LiveSubmitUrl;
        string liveValidationUrl = PaymentGatewayHelper.LiveValidationUrl;
        string prodSuccessCallbackUrl = PaymentGatewayHelper.ProdSuccessCallbackUrl;
        string prodFailCallbackUrl = PaymentGatewayHelper.ProdFailCallbackUrl;
        string prodCancelCallbackUrl = PaymentGatewayHelper.ProdCancelCallbackUrl;
        string prodIpnLintener = PaymentGatewayHelper.ProdIpnLintener;
        string sandboxStoreId = PaymentGatewayHelper.SandboxStoreId;
        string sandboxStorePassword = PaymentGatewayHelper.SandboxStorePassword;
        string devSuccessCallbackUrl = PaymentGatewayHelper.DevSuccessCallbackUrl;
        string devFailCallbackUrl = PaymentGatewayHelper.DevFailCallbackUrl;
        string devCancelCallbackUrl = PaymentGatewayHelper.DevCancelCallbackUrl;
        string devIpnLintener = PaymentGatewayHelper.DevIpnLintener;

        // For Live
        public bool IsValidateStore()
        {
            
            if (string.IsNullOrWhiteSpace(liveStoreId))
            {
                throw new Exception("Please provide Store ID to initialize SSLCommerz");
            }

            if (string.IsNullOrWhiteSpace(liveStorePassword))
            {
                throw new Exception("Please provide Store Password to initialize SSLCommerz");
            }

            return true;
        }

        public async Task<SSLCommerzInitResponse> InitiatePaymentAsync(NameValueCollection postData)
        {
            var initResponse = new SSLCommerzInitResponse();
            
            try
            {
                var response = await SendPaymentRequestAsync(liveSubmitUrl, postData);
                initResponse = new JavaScriptSerializer().Deserialize<SSLCommerzInitResponse>(response);
                return initResponse;
            }
            catch (Exception e)
            {
                var jsonResponse = JsonConvert.SerializeObject(initResponse);
                var jsonPostData = JsonConvert.SerializeObject(postData.ToDictionary());
                throw new Exception($"InitResponse: {jsonResponse} {Environment.NewLine}- PostData: { jsonPostData } {Environment.NewLine}- ErrorMsg: { e.Message}");
            }
        }

        public async Task<TransactionValidationDto> ValidateTransactionAsync(Dictionary<string, string> responseDic)
        {
            return await Task.Run(() =>
            {
                string message;
                bool isValidTransaction;

                var hashVerified = IpnHashVerify(responseDic, liveStorePassword);
                if (hashVerified)
                {
                    responseDic.TryGetValue("val_id", out string valId);
                    var encodedValId = HttpUtility.UrlEncode(valId);
                    var encodedStoreId = HttpUtility.UrlEncode(liveStoreId);
                    var encodedStorePassword = HttpUtility.UrlEncode(liveStorePassword);

                    var validateUrl = liveValidationUrl + "?val_id=" + encodedValId + "&store_id=" + encodedStoreId + "&store_passwd=" + encodedStorePassword + "&v=1&format=json";

                    var request = (HttpWebRequest)WebRequest.Create(validateUrl);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();

                    var jsonResponse = string.Empty;
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        jsonResponse = reader.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        var validationResponse = new JavaScriptSerializer().Deserialize<SSLCommerzValidatorResponse>(jsonResponse);

                        if (validationResponse.status == "VALID" || validationResponse.status == "VALIDATED")
                        {
                            responseDic.TryGetValue("tran_id", out string merchantTrxId);
                            responseDic.TryGetValue("amount", out string merchantTrxAmount);
                            responseDic.TryGetValue("currency", out string merchantTrxCurrency);

                            if (!string.IsNullOrWhiteSpace(merchantTrxCurrency) && merchantTrxCurrency.Equals("BDT", StringComparison.OrdinalIgnoreCase))
                            {
                                if (merchantTrxId == validationResponse.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(validationResponse.amount)) < 1))
                                {
                                    isValidTransaction = true;
                                    message = "Amount mached!";
                                }
                                else
                                {
                                    isValidTransaction = false;
                                    message = "Amount not matching!";
                                }
                            }
                            else
                            {
                                if (merchantTrxId == validationResponse.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(validationResponse.currency_amount)) < 1) && merchantTrxCurrency == validationResponse.currency_type)
                                {
                                    isValidTransaction = true;
                                    message = "Currency amount matched";
                                }
                                else
                                {
                                    isValidTransaction = false;
                                    message = "Currency amount not matching";
                                }
                            }
                        }
                        else
                        {
                            isValidTransaction = false;
                            message = "This transaction is either expired or fails";
                        }
                    }
                    else
                    {
                        isValidTransaction = false;
                        message = "Unable to get transaction JSON status";
                    }
                }
                else
                {
                    isValidTransaction = false;
                    message = "Unable to verify hash";
                }

                return new TransactionValidationDto
                {
                    Message = message,
                    IsValidTransaction = isValidTransaction
                };
            });
        }

        public Task<bool> InitiateRefundAsync()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection CreatePostData(SslCommerzPostDataDto postDataDto)
        {
            var postData = new NameValueCollection
            {
                { "store_id", liveStoreId },
                { "store_passwd", liveStorePassword },
                { "total_amount", $"{postDataDto.total_amount}" },
                { "currency", $"{postDataDto.currency}" },
                { "tran_id", $"{postDataDto.tran_id}" },
                { "success_url", prodSuccessCallbackUrl },
                { "fail_url", prodFailCallbackUrl },
                { "cancel_url", prodCancelCallbackUrl },
                { "ipn_url", prodIpnLintener },
                { "cus_name", $"{postDataDto.cus_name}" },
                { "cus_email", $"{postDataDto.cus_email}" },
                { "cus_add1", $"{postDataDto.cus_add1}" },
                { "cus_add2", $"{postDataDto.cus_add2}" },
                { "cus_city", $"{postDataDto.cus_city}"  },
                { "cus_state", $"{postDataDto.cus_state}" },
                { "cus_postcode", $"{postDataDto.cus_postcode}" },
                { "cus_country", $"{postDataDto.cus_country}" },
                { "cus_phone", $"{postDataDto.cus_phone}" },
                { "cus_fax", $"{postDataDto.cus_fax}" },
                { "ship_name", $"{postDataDto.ship_name}" },
                { "ship_add1", $"{postDataDto.ship_add1}" },
                { "ship_add2", $"{postDataDto.ship_add2}" },
                { "ship_city", $"{postDataDto.ship_city}" },
                { "ship_state", $"{postDataDto.ship_state}" },
                { "ship_postcode", $"{postDataDto.ship_postcode}" },
                { "ship_country", $"{postDataDto.ship_country}" },
                { "value_a", $"{postDataDto.value_a}" },
                { "value_b", $"{postDataDto.value_b}" },
                { "value_c", $"{postDataDto.value_c}" },
                { "value_d", $"{postDataDto.value_d}" },
                { "shipping_method", $"{postDataDto.shipping_method}" },
                { "num_of_item", $"{postDataDto.num_of_item}" },
                { "product_name", $"{postDataDto.product_name}" },
                { "product_profile", $"{postDataDto.product_profile}" },
                { "product_category", $"{postDataDto.product_category}" }
            };

            return postData;
        }


        // For Sandbox
        public bool IsValidateTestStore()
        {
            if (string.IsNullOrWhiteSpace(PaymentGatewayHelper.SandboxStoreId))
            {
                throw new Exception("Please provide Store ID to initialize SSLCommerz");
            }

            if (string.IsNullOrWhiteSpace(PaymentGatewayHelper.SandboxStorePassword))
            {
                throw new Exception("Please provide Store Password to initialize SSLCommerz");
            }

            return true;
        }

        public async Task<SSLCommerzInitResponse> InitiateTestPaymentAsync(NameValueCollection postData)
        {
            var initResponse = new SSLCommerzInitResponse();
            try
            {
                string response = await SendPaymentRequestAsync(PaymentGatewayHelper.SandboxSubmitUrl, postData);
                initResponse = new JavaScriptSerializer().Deserialize<SSLCommerzInitResponse>(response);
                return initResponse;
            }
            catch (Exception e)
            {
                var jsonResponse = JsonConvert.SerializeObject(initResponse);
                var jsonPostData = JsonConvert.SerializeObject(postData.ToDictionary());
                throw new Exception($"InitResponse: {jsonResponse} {Environment.NewLine}- PostData: { jsonPostData } {Environment.NewLine}- ErrorMsg: { e.Message}");
            }
        }

        public async Task<TransactionValidationDto> ValidateTestTransactionAsync(Dictionary<string, string> responseDic)
        {
            return await Task.Run(() =>
            {
                string message;
                bool isValidTransaction;

                var hashVerified = IpnHashVerify(responseDic, PaymentGatewayHelper.SandboxStorePassword);
                if (hashVerified)
                {
                    responseDic.TryGetValue("val_id", out string valId);
                    var encodedValId = HttpUtility.UrlEncode(valId);
                    var encodedStoreId = HttpUtility.UrlEncode(PaymentGatewayHelper.SandboxStoreId);
                    var encodedStorePassword = HttpUtility.UrlEncode(PaymentGatewayHelper.SandboxStorePassword);

                    var validateUrl = liveValidationUrl + "?val_id=" + encodedValId + "&store_id=" + encodedStoreId + "&store_passwd=" + encodedStorePassword + "&v=1&format=json";

                    var request = (HttpWebRequest)WebRequest.Create(validateUrl);
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();

                    var jsonResponse = string.Empty;
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        jsonResponse = reader.ReadToEnd();
                    }

                    if (!string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        var validationResponse = new JavaScriptSerializer().Deserialize<SSLCommerzValidatorResponse>(jsonResponse);

                        if (validationResponse.status == "VALID" || validationResponse.status == "VALIDATED")
                        {
                            responseDic.TryGetValue("tran_id", out string merchantTrxId);
                            responseDic.TryGetValue("amount", out string merchantTrxAmount);
                            responseDic.TryGetValue("currency", out string merchantTrxCurrency);

                            if (!string.IsNullOrWhiteSpace(merchantTrxCurrency) && merchantTrxCurrency.Equals("BDT", StringComparison.OrdinalIgnoreCase))
                            {
                                if (merchantTrxId == validationResponse.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(validationResponse.amount)) < 1))
                                {
                                    isValidTransaction = true;
                                    message = "Amount mached!";
                                }
                                else
                                {
                                    isValidTransaction = false;
                                    message = "Amount not matching!";
                                }
                            }
                            else
                            {
                                if (merchantTrxId == validationResponse.tran_id && (Math.Abs(Convert.ToDecimal(merchantTrxAmount) - Convert.ToDecimal(validationResponse.currency_amount)) < 1) && merchantTrxCurrency == validationResponse.currency_type)
                                {
                                    isValidTransaction = true;
                                    message = "Currency amount matched";
                                }
                                else
                                {
                                    isValidTransaction = false;
                                    message = "Currency amount not matching";
                                }
                            }
                        }
                        else
                        {
                            isValidTransaction = false;
                            message = "This transaction is either expired or fails";
                        }
                    }
                    else
                    {
                        isValidTransaction = false;
                        message = "Unable to get Transaction JSON status";
                    }
                }
                else
                {
                    isValidTransaction = false;
                    message = "Unable to verify hash";
                }

                return new TransactionValidationDto
                {
                    Message = message,
                    IsValidTransaction = isValidTransaction
                };
            });
        }

        public Task<bool> InitiateTestRefundAsync()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection CreateTestPostData(SslCommerzPostDataDto postDataDto)
        {
            var postData = new NameValueCollection
            {
                { "store_id", sandboxStoreId },
                { "store_passwd", sandboxStorePassword },
                { "total_amount", $"{postDataDto.total_amount}" },
                { "currency", $"{postDataDto.currency}" },
                { "tran_id", $"{postDataDto.tran_id}" },
                { "success_url", devSuccessCallbackUrl},
                { "fail_url", devFailCallbackUrl },
                { "cancel_url", devCancelCallbackUrl },
                { "ipn_url", devIpnLintener },
                { "cus_name", $"{postDataDto.cus_name}" },
                { "cus_email", $"{postDataDto.cus_email}" },
                { "cus_add1", $"{postDataDto.cus_add1}" },
                { "cus_add2", $"{postDataDto.cus_add2}" },
                { "cus_city", $"{postDataDto.cus_city}"  },
                { "cus_state", $"{postDataDto.cus_state}" },
                { "cus_postcode", $"{postDataDto.cus_postcode}" },
                { "cus_country", $"{postDataDto.cus_country}" },
                { "cus_phone", $"{postDataDto.cus_phone}" },
                { "cus_fax", $"{postDataDto.cus_fax}" },
                { "ship_name", $"{postDataDto.ship_name}" },
                { "ship_add1", $"{postDataDto.ship_add1}" },
                { "ship_add2", $"{postDataDto.ship_add2}" },
                { "ship_city", $"{postDataDto.ship_city}" },
                { "ship_state", $"{postDataDto.ship_state}" },
                { "ship_postcode", $"{postDataDto.ship_postcode}" },
                { "ship_country", $"{postDataDto.ship_country}" },
                { "value_a", $"{postDataDto.value_a}" },
                { "value_b", $"{postDataDto.value_b}" },
                { "value_c", $"{postDataDto.value_c}" },
                { "value_d", $"{postDataDto.value_d}" },
                { "shipping_method", $"{postDataDto.shipping_method}" },
                { "num_of_item", $"{postDataDto.num_of_item}" },
                { "product_name", $"{postDataDto.product_name}" },
                { "product_profile", $"{postDataDto.product_profile}" },
                { "product_category", $"{postDataDto.product_category}" }
            };

            return postData;
        }

        
        // Common Actions
        public async Task<string> SendPaymentRequestAsync(string url, NameValueCollection postData)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = await client.UploadValuesTaskAsync(url, "POST", postData);
            }
            return Encoding.UTF8.GetString(response);
        }

        public bool IpnHashVerify(Dictionary<string, string> responseDic, string storePassword)
        {
            responseDic.TryGetValue("verify_sign", out string verify_sign);
            responseDic.TryGetValue("verify_key", out string verify_key);

            if (!string.IsNullOrWhiteSpace(verify_sign) && !string.IsNullOrWhiteSpace(verify_key))
            {
                var keyList = verify_key.Split("%2C");
                var dataArray = new List<KeyValuePair<string, string>>();

                foreach (var key in keyList)
                {
                    responseDic.TryGetValue(key, out string value);
                    dataArray.Add(new KeyValuePair<string, string>(key, value));
                }

                string hashedPass = this.MD5(storePassword);
                dataArray.Add(new KeyValuePair<string, string>("store_passwd", hashedPass));

                dataArray.Sort(
                    delegate (KeyValuePair<string, string> pair1,
                    KeyValuePair<string, string> pair2)
                    {
                        return pair1.Key.CompareTo(pair2.Key);
                    }
                );

                var hashString = string.Empty;
                foreach (var kv in dataArray)
                {
                    hashString += kv.Key + "=" + kv.Value + "&";
                }

                hashString = hashString.TrimEnd('&');
                var generatedHash = this.MD5(hashString);

                // Check if generated hash and verify_sign match or not
                if (generatedHash == verify_sign)
                {
                    return true; // Matched
                }
                return false; // Not Matched
            }
            else
            {
                return false;
            }
        }

        public string MD5(string s)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(s);
            byte[] hashedBytes = System.Security.Cryptography.MD5.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedString;
        }
    }
}