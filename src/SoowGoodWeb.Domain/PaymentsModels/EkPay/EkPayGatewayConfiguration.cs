using Microsoft.Extensions.Configuration;
using SoowGoodWeb.PaymentsModels;
using System;

namespace SoowGoodWeb.EkPayData
{
    public class EkPayGatewayConfiguration : IPaymentGatewayConfiguration
    {
        private readonly IConfiguration _appConfiguration;

        public EkPayGatewayConfiguration(IConfiguration configurationAccessor)
        {
            _appConfiguration = configurationAccessor;
        }

        public bool IsActive => _appConfiguration["Payment:eKPay:IsActive"].To<bool>();
        public string SubmitUrl => _appConfiguration["Payment:eKPay:SubmitUrl"];
        //public string ValidationUrl => _appConfiguration["Payment:eKPay:ValidationUrl"];
        //public string CheckingUrl => _appConfiguration["Payment:eKPay:CheckingUrl"];

        // Sandbox or Test
        public string SandboxEnvironment => _appConfiguration["Payment:eKPay:SandboxEnvironment"];
        public string SandboxStoreId => _appConfiguration["Payment:eKPay:SandboxStoreId"];
        public string SandboxStorePassword => _appConfiguration["Payment:eKPay:SandboxStorePassword"];
        public string SanboxUrl => _appConfiguration["Payment:eKPay:SanboxUrl"];
        public string SandboxSubmitUrl => SanboxUrl + SubmitUrl;
        //public string SandboxValidationUrl => SanboxUrl + ValidationUrl;
        //public string SandboxCheckingUrl => SanboxUrl + CheckingUrl;

        //// Live or Prod
        //public string LiveEnvironment => _appConfiguration["Payment:SslCommerz:LiveEnvironment"];
        //public string LiveStoreId => _appConfiguration["Payment:SslCommerz:LiveStoreId"];
        //public string LiveStorePassword => _appConfiguration["Payment:SslCommerz:LiveStorePassword"];
        //public string LiveUrl => _appConfiguration["Payment:SslCommerz:LiveUrl"];
        //public string LiveSubmitUrl => LiveUrl + SubmitUrl;
        //public string LiveValidationUrl => LiveUrl + ValidationUrl;
        //public string LiveCheckingUrl => LiveUrl + CheckingUrl;

        // Success, Fail or Cancel Callback Url
        public string DevSuccessCallbackUrl => _appConfiguration["Payment:eKPay:DevSuccessCallbackUrl"];
        public string DevFailCallbackUrl => _appConfiguration["Payment:eKPay:DevFailCallbackUrl"];
        public string DevCancelCallbackUrl => _appConfiguration["Payment:eKPay:DevCancelCallbackUrl"];
        //public string DevIpnLintener => _appConfiguration["Payment:eKPay:DevIpnLintener"];

        //public string ProdSuccessCallbackUrl => _appConfiguration["Payment:SslCommerz:ProdSuccessCallbackUrl"];
        //public string ProdFailCallbackUrl => _appConfiguration["Payment:SslCommerz:ProdFailCallbackUrl"];
        //public string ProdCancelCallbackUrl => _appConfiguration["Payment:SslCommerz:ProdCancelCallbackUrl"];
        //public string ProdIpnLintener => _appConfiguration["Payment:SslCommerz:ProdIpnLintener"];

        //// Success, Fail or Cancel Client Url
        public string DevSuccessClientUrl => _appConfiguration["Payment:SslCommerz:DevSuccessClientUrl"];
        public string DevFailClientUrl => _appConfiguration["Payment:SslCommerz:DevFailClientUrl"];
        public string DevCancelClientUrl => _appConfiguration["Payment:SslCommerz:DevCancelClientUrl"];

        //public string ProdSuccessClientUrl => _appConfiguration["Payment:SslCommerz:ProdSuccessClientUrl"];
        //public string ProdFailClientUrl => _appConfiguration["Payment:SslCommerz:ProdFailClientUrl"];
        //public string ProdCancelClientUrl => _appConfiguration["Payment:SslCommerz:ProdCancelClientUrl"];
    }
}