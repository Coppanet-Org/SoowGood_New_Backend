using Microsoft.Extensions.Configuration;

namespace SoowGoodWeb
{
    public static class PermissionHelper
    {
        //public readonly IConfiguration _appConfiguration;
        //public PermissionHelper(IConfiguration configurationAccessor)
        //{
        //    _appConfiguration = configurationAccessor;
        //}
        ////public string SubmitUrl => _appConfiguration["Payment:SslCommerz:SubmitUrl"];
        //public string _authority => _appConfiguration["AuthServer:Authority"];
        //public string _identityClientUrl => _appConfiguration["AuthApi:AuthClientUrl"];

        //public string _clientId => _appConfiguration["ClientCredentials:ClientId"]; 
        //public string _clientSecret => _appConfiguration["ClientCredentials:ClientSecret"];
        //public string _scope => _appConfiguration["ClientCredentials:Scope"];

        public static readonly string _authority = "https://198.38.92.117";
        public static readonly string _identityClientUrl = "https://198.38.92.117:8443";
        //public static readonly string _authority = "https://localhost:44380";
        //public static readonly string _identityClientUrl = "https://localhost:44392";// http://idapi.mis1pwd.com";
        //public static readonly string _identityClientUrlDev = "https://localhost:44392";
        //public static readonly string _selfClientUrlDev = "https://localhost:44373";
        //public static readonly string _identityApiName = "/api/app/permission-map";
        //public static readonly string _permissionGroupKey = "permissionGroupKey";
        //public static readonly string _permissionGroupValue = "PWDEstimate";
        //public static readonly string _providerName = "providerName";
        //public static readonly string _providerValue = "R";
        //public static readonly string _providerKey = "providerKeys";
        public static readonly string _clientId = "SoowGoodWeb_App";
        public static readonly string _clientSecret = "1q2w3e*";
        public static readonly string _scope = "SoowGoodWeb";
    }
}
