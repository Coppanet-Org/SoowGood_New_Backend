namespace SoowGoodWeb
{
    public static class PaymentGatewayHelper
    {
        public static readonly bool IsActive = true;
        public static readonly string SubmitUrl = "gwprocess/v4/api.php";
        public static readonly string ValidationUrl = "validator/api/validationserverAPI.php";
        public static readonly string CheckingUrl = "validator/api/merchantTransIDvalidationAPI.php";

        // Sandbox or Test
        public static readonly string SandboxEnvironment = "sandbox";
        public static readonly string SandboxStoreId = "shell624bb8d56e27b";
        public static readonly string SandboxStorePassword = "shell624bb8d56e27b@ssl";
        public static readonly string SanboxUrl = "https://sandbox.sslcommerz.com/";
        public static readonly string SandboxSubmitUrl = SanboxUrl + SubmitUrl;
        public static readonly string SandboxValidationUrl = SanboxUrl + ValidationUrl;
        public static readonly string SandboxCheckingUrl = SanboxUrl + CheckingUrl;

        // Live or Prod
        public static readonly string LiveEnvironment = "securepay";
        public static readonly string LiveStoreId = "pwdgovbdlive";
        public static readonly string LiveStorePassword = "62564C7A57C5485843";
        public static readonly string LiveUrl = "https://securepay.sslcommerz.com/";
        public static readonly string LiveSubmitUrl = LiveUrl + SubmitUrl;
        public static readonly string LiveValidationUrl = LiveUrl + ValidationUrl;
        public static readonly string LiveCheckingUrl = LiveUrl + CheckingUrl;

        // Success, Fail or Cancel Callback Url
        public static readonly string DevSuccessCallbackUrl = "http://localhost:21021/api/services/test/payment-success";
        public static readonly string DevFailCallbackUrl = "http://localhost:21021/api/services/test/payment-fail";
        public static readonly string DevCancelCallbackUrl = "http://localhost:21021/api/services/test/payment-cancel";
        public static readonly string DevIpnLintener = "http://localhost:21021/api/services/test/ipn_listener"; // Validate Payment with IPN

        public static readonly string ProdSuccessCallbackUrl = "https://careerapi.mis1pwd.com/api/services/payment-success";
        public static readonly string ProdFailCallbackUrl = "https://careerapi.mis1pwd.com/api/services/payment-fail";
        public static readonly string ProdCancelCallbackUrl = "https://careerapi.mis1pwd.com/payment-api/services/cancel";
        public static readonly string ProdIpnLintener = "https://careerapi.mis1pwd.com/api/services/ipn_listener";

        // Success, Fail or Cancel Client Url
        public static readonly string DevSuccessClientUrl = "http://localhost:4200/sslcommerz/payment-success";
        public static readonly string DevFailClientUrl = "http://localhost:4200/sslcommerz/payment-fail";
        public static readonly string DevCancelClientUrl = "http://localhost:4200/sslcommerz/payment-cancel";

        public static readonly string ProdSuccessClientUrl = "http://recruitment.pwd.gov.bd/sslcommerz/payment-success";
        public static readonly string ProdFailClientUrl = "http://recruitment.pwd.gov.bd/sslcommerz/payment-fail";
        public static readonly string ProdCancelClientUrl = "http://recruitment.pwd.gov.bd/sslcommerz/payment-cancel";

        // Nagad
        public static readonly string Environment = "sandbox";        
        //public static readonly string BaseUrl = _appConfiguration["Payment:Nagad:BaseUrl"];
        public static readonly string InitializeAPI = "https://api.mynagad.com/api/dfs/check-out/initialize/";
        public static readonly string CheckOutAPI = "https://api.mynagad.com/api/dfs/check-out/complete/";
        public static readonly string PaymentVerificationAPI = "https://api.mynagad.com/api/dfs/verify/payment/";
        public static readonly string MerchantCallbackURL = "http://career.hbri.gov.bd/payment-result";
    }
}
