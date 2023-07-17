using Microsoft.AspNetCore.Authorization;
using SoowGoodWeb.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp;
using System.Xml.Linq;
using Volo.Abp.Users;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Transactions;

namespace SoowGoodWeb.Services
{

    public class UserAccountsService : SoowGoodWebAppService
    {
        string authClientUrl = PermissionHelper._identityClientUrl;
        string authUrl = PermissionHelper._authority;

        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _roleManager;
        //private readonly IdentityUserLogin _signInManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly IEmailSender _emailSender;

        public UserAccountsService(IdentityUserManager userManager, IdentityRoleManager roleManager, SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // POST /api/account/reset-password
        //public virtual async Task ResetPassword(ResetPasswordInputDto inputDto)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(inputDto.UserId);
        //        //var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //await _userManager.ResetPasswordAsync(user, resetToken, inputDto.NewPassword);
        //        await _userManager.RemovePasswordAsync(user);
        //        await _userManager.AddPasswordAsync(user, inputDto.NewPassword);
        //        await _userManager.UpdateAsync(user);
        //    }
        //    catch (Exception)
        //    {
        //        throw new UserFriendlyException("Password reset not successfull.");
        //    }
        //}

        //// POST /api/account/reset-password-request
        //public async Task ResetPasswordRequest(ResetPasswordRequestInputDto input)
        //{
        //    var user = await _userManager.FindByEmailAsync(input.EmailAddress);
        //    if (user == null)
        //    {
        //        return;
        //    }

        //    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        //    var body = L[
        //        "ResetPasswordRequest:EmailBody",
        //        user.Name,
        //        input.ReturnUrl.RemovePostFix("/"),
        //        System.Web.HttpUtility.UrlEncode(resetToken),
        //        user.TenantId,
        //        user.Id,
        //        System.Web.HttpUtility.UrlEncode(user.Email)
        //    ];

        //    try
        //    {
        //        await _emailSender.SendAsync(user.Email, L["ResetPasswordRequest:EmailSubject"], body);
        //    }
        //    catch (Exception)
        //    {
        //        throw new UserFriendlyException("Unable to process your request, please try again later.");
        //    }
        //    await _userManager.UpdateAsync(user);
        //}

        //public async Task<List<OrganizationUnitDto>> Offices(ChangePass input)
        //{
        //    var user = await _userManager.FindByNameAsync(input.UserName);
        //    var units = await _userManager.GetOrganizationUnitsAsync(user);
        //    return ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(units);
        //}
        [AllowAnonymous]
        public async Task<bool> RefreshAccessToken(IdentityUser user)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _userManager.RemoveAuthenticationTokenAsync(user, "SoowGoodWeb", "RefreshToken");
                    var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "SoowGoodWeb", "RefreshToken");
                    var result = await _userManager.SetAuthenticationTokenAsync(user, "SoowGoodWeb", "RefreshToken", newRefreshToken);

                    scope.Complete();
                    return result.Succeeded;

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task<IdentityUser?> Login(LoginDto userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.UserName);

            if (user != null)
            {
                var userole = await _userManager.GetRolesAsync(user); //_roleManager.GetRolesAsync(user);
                var res = await _signInManager.PasswordSignInAsync(user.UserName, userDto.password, userDto.RememberMe, lockoutOnFailure: false);
                if (res.Succeeded)
                {
                    //if(!string.IsNullOrEmpty(userole[0])) 
                    //    user.UserRole = userole[0];
                    //await RefreshAccessToken(user);
                    //user.Message = "User Exists! Login Successful!";
                    return user;
                }
            }
            return null;
        }
        [AllowAnonymous]
        public virtual async Task<string> SignupUser(UserInfoDto userDto, string password, string role)
        {
            try
            {
                var user = ObjectMapper.Map<UserInfoDto, IdentityUser>(userDto);
                var isUserExists = await _userManager.FindByNameAsync(user.UserName);
                //var exUser = _userManager.Users.Where(u => u.UserName == userDto.UserName && u.IsActive == true).FirstOrDefault();
                if (isUserExists == null)
                {
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {

                        var roleRes = await _userManager.AddToRoleAsync(user, role);
                        if (roleRes.Succeeded)
                        {

                            return "User Created Successfully";

                        }
                        else
                        {
                            var errl = "";
                            foreach (var e in roleRes.Errors)
                            {
                                errl += e.Description;
                            }
                            return "User Registration failed - : reason - " + errl;
                        }

                    }
                    else
                    {
                        var err = "";
                        foreach (var e in result.Errors)
                        {
                            err += e.Description;
                        }
                        return "User Registration failed - : reason - " + err;
                    }
                }
                else
                {
                    return "User Already Exists";
                }

            }
            catch (Exception)
            {
                throw new UserFriendlyException("User Create not successfull.");
                //return ex.Message;
            }
            return "Erro: User Create not successfull. !!!";
        }

        //private async Task<TokenResponse> GetToken()
        //{
        //    var authorityUrl = $"{authUrl}";

        //    var authority = new HttpClient();
        //    var discoveryDocument = await authority.GetDiscoveryDocumentAsync(authorityUrl);
        //    if (discoveryDocument.IsError)
        //    {
        //        //return null;
        //    }

        //    // Request Token
        //    var tokenResponse = await authority.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    {
        //        Address = discoveryDocument.TokenEndpoint,
        //        ClientId = PermissionHelper._clientId,
        //        ClientSecret = PermissionHelper._clientSecret,
        //        Scope = PermissionHelper._scope
        //    });

        //    if (tokenResponse.IsError)
        //    {
        //        //return null;
        //    }
        //    return tokenResponse; 
        //} , string passWord, string role
        //[AllowAnonymous]
        //public async Task<string> Register(UserRegInfoDto user)
        //{

        //    using (var client = new HttpClient())
        //    {
        //        //var tokenResponse = await GetToken();
        //        client.BaseAddress = new Uri(authClientUrl);
        //        //client.SetBearerToken(tokenResponse.AccessToken);
        //        //client.DefaultRequestHeaders.Accept.Clear();
        //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        var obj = new
        //        {

        //            userName = user.UserName,
        //            name = user.Name,
        //            surname = user.Surname,
        //            email = user.Email,
        //            phoneNumber = user.PhoneNumber,
        //            isActive = user.IsActive,
        //            lockoutEnabled = false,
        //            roleNames = user.RoleNames,
        //            password = user.Password

        //        };

        //        JsonContent content = JsonContent.Create(obj);

        //        //JsonContent content = JsonSerializer.Serialize<UserInfoDto>(obj);
        //        //GET Method  api/account/register

        //        try
        //        {
        //            //HttpResponseMessage response = await client.PostAsync($"api/app/account/signup-user?password={passWord}&role={role}", content);
        //            HttpResponseMessage response = await client.PostAsync($"api/identity/users", content);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                return "User Created Successfully";

        //            }
        //            else
        //            {
        //                Console.WriteLine("Internal server Error");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //    return null;
        //}
    }
}

