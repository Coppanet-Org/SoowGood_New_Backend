using Microsoft.AspNetCore.Authorization;
using SoowGoodWeb.DtoModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp;
using Microsoft.AspNetCore.Identity;
using System.Transactions;
using SoowGoodWeb.Models;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using System.Net.Http;
using IdentityModel.Client;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using System.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SoowGoodWeb.Services
{

    public class UserAccountsService : SoowGoodWebAppService
    {
        string authClientUrl = PermissionHelper._identityClientUrl;
        string authUrl = PermissionHelper._authority;

        private readonly IdentityUserManager _userManager;
        //private readonly IdentityRoleManager _roleManager;
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
        private readonly IRepository<PatientProfile> _patientProfileRepository;
        private readonly IRepository<AgentProfile> _agentProfileRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        //private readonly DoctorProfileService _doctorProfileservice;

        private readonly IEmailSender _emailSender;

        public UserAccountsService(IdentityUserManager userManager,
                                   //IdentityRoleManager roleManager,
                                   //SignInManager<IdentityUser> signInManager,
                                   IRepository<DoctorProfile> doctorProfileRepository,
                                   IRepository<PatientProfile> patientProfileRepository,
                                   IRepository<AgentProfile> agentProfileRepository,
                                   IUnitOfWorkManager unitOfWorkManager,
                                   IEmailSender emailSender)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
            //_signInManager = signInManager;
            _doctorProfileRepository = doctorProfileRepository;
            _patientProfileRepository = patientProfileRepository;
            _agentProfileRepository = agentProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _emailSender = emailSender;
            //_doctorProfileservice = new DoctorProfileService(_doctorProfileRepository, _unitOfWorkManager);
        }
        private async Task<TokenResponse> GetToken()
        {
            var authorityUrl = $"{PermissionHelper._authority}";

            var authority = new HttpClient();
            var discoveryDocument = await authority.GetDiscoveryDocumentAsync(authorityUrl);
            if (discoveryDocument.IsError)
            {
                //return null;
            }

            // Request Token
            var tokenResponse = await authority.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = PermissionHelper._clientId,
                ClientSecret = PermissionHelper._clientSecret,
                Scope = PermissionHelper._scope
            });

            if (tokenResponse.IsError)
            {
                //return null;
            }
            return tokenResponse;
        }

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
        public async Task<LoginResponseDto> Login(LoginDto userDto)
        {
            LoginResponseDto result = new LoginResponseDto();
            bool profileExists = false;
            var doctors = await _doctorProfileRepository.GetListAsync();//(d => d.MobileNo==mobileNo);
            var doctor = doctors.FirstOrDefault(d => d.MobileNo == userDto.UserName && d.IsDeleted == false);

            if (doctor == null)
            {
                var patients = await _patientProfileRepository.GetListAsync();//.GetAsync(p => p.MobileNo==mobileNo);
                var patient = patients.FirstOrDefault(p => p.MobileNo == userDto.UserName && p.IsDeleted == false);
                if (patient == null)
                {
                    var agents = await _agentProfileRepository.GetListAsync();//.GetAsync(a => a.MobileNo==mobileNo);
                    var agent = agents.FirstOrDefault(a => a.MobileNo == userDto.UserName && a.IsDeleted == false);
                    if (agent == null)
                    {
                        profileExists = false;
                    }
                    else
                    {
                        profileExists = true;
                    }
                }
                else
                {
                    profileExists = true;
                }
            }
            else
            {
                profileExists = true;
            }

            if (profileExists == true)
            {
                using (var client = new HttpClient())
                {
                    var tokenResponse = await GetToken();
                    client.BaseAddress = new Uri(authClientUrl);
                    client.SetBearerToken(tokenResponse.AccessToken);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var update = JsonSerializer.Serialize(userDto);
                    var requestContent = new StringContent(update, Encoding.UTF8, "application/json");
                    HttpResponseMessage response =
                        await client.PostAsync(($"api/app/account/login"), requestContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var newUserString = await response.Content.ReadAsStringAsync();
                        var newUser = JsonConvert.DeserializeObject<LoginResponseDto>(newUserString);

                        result = new LoginResponseDto()
                        {
                            UserId = newUser?.UserId,
                            UserName = newUser?.UserName,
                            RoleName = newUser?.RoleName,
                            Success = newUser.Success,
                            Message = newUser.Message
                        };
                        //return result;
                    }
                }
            }
            else
            {
                result = new LoginResponseDto()
                {                    
                    Success = false,
                    Message = "No User Found"
                };

            }
            return result;
        }

        [AllowAnonymous]
        public virtual async Task<UserSignUpResultDto> SignupUser(UserInfoDto userDto, string password, string role)
        {
            UserSignUpResultDto result = new UserSignUpResultDto();
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(authClientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method

                var update = JsonSerializer.Serialize(userDto);
                var requestContent = new StringContent(update, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(($"api/app/account/sign-up?password={password}&role={role}"), requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var newUserString = await response.Content.ReadAsStringAsync();
                    var newUser = JsonConvert.DeserializeObject<UserSignUpResultDto>(newUserString);

                    result = new UserSignUpResultDto()
                    {
                        UserId = newUser?.UserId,
                        UserName = newUser?.UserName,
                        Name = newUser?.Name,
                        Email = newUser?.Email,
                        PhoneNumber = newUser?.PhoneNumber,
                        Success = newUser?.Success,
                        Message = newUser.Message.ToList(),

                    };
                    return result;

                }
            }
            return result;
        }

        //[AllowAnonymous]
        public virtual async Task<ResetPasswordResponseDto> ResetPassword(ResetPasswordInputDto inputDto)
        {
            ResetPasswordResponseDto result = new ResetPasswordResponseDto();
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(authClientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method

                var update = JsonSerializer.Serialize(inputDto);
                var requestContent = new StringContent(update, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(($"api/app/account/reset-password"), requestContent);
                if (response.IsSuccessStatusCode)
                {
                    var newUserString = await response.Content.ReadAsStringAsync();
                    var newUser = JsonConvert.DeserializeObject<ResetPasswordResponseDto>(newUserString);

                    result = new ResetPasswordResponseDto()
                    {
                        //UserId = newUser?.UserId,
                        UserName = newUser?.UserName,
                        Name = newUser?.Name,
                        //Email = newUser?.Email,
                        //PhoneNumber = newUser?.PhoneNumber,
                        Success = newUser?.Success,
                        Message = newUser?.Message//.ToList(),

                    };
                    return result;

                }
            }
            return result;
        }
        // POST /api/account/reset-password
        public async Task<ResetPasswordResponseDto> ResetPassword_App(ResetPasswordInputDto inputDto)
        {
            ResetPasswordResponseDto result = new ResetPasswordResponseDto();
            try
            {
                var mainUser = await _userManager.FindByNameAsync(inputDto.UserId);
                if (mainUser != null)
                {
                    string userId = mainUser.Id.ToString();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var removePwd = await _userManager.RemovePasswordAsync(user);
                            if (removePwd.Succeeded)
                            {
                                var addnewPwd = await _userManager.AddPasswordAsync(user, inputDto.NewPassword);
                                if (addnewPwd.Succeeded)
                                {
                                    var updateUser = await _userManager.UpdateAsync(user);
                                    if (updateUser.Succeeded)
                                    {
                                        result.UserName = inputDto.UserId.ToString();
                                        result.Name = user.Name;
                                        result.Success = true;
                                        result.Message = "Dear Mr. " + user.Name + ",Your Password Updated Successfully at SoowGood.com. You can login now with your new password.";

                                    }
                                    else // if (updateUser.Errors.Count() > 0)
                                    {

                                        result.Success = false;
                                        foreach (var error in updateUser.Errors)
                                        {
                                            result.Message = "Password Reset Failed for. " + error;
                                        }
                                    }
                                }
                                else //if (addnewPwd.Errors.Count() > 0)
                                {

                                    result.Success = false;
                                    foreach (var error in addnewPwd.Errors)
                                    {
                                        result.Message = "Password Reset Failed for. " + error;
                                    }
                                }
                            }
                            else //if (removePwd.Errors.Count() > 0)
                            {

                                result.Success = false;
                                foreach (var error in removePwd.Errors)
                                {
                                    result.Message = "Password Reset Failed for. " + error;
                                }
                            }
                        }
                        else //if (removePwd.Errors.Count() > 0)
                        {
                            result.Success = false;
                            result.Message = "User not found !!!";
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new UserFriendlyException("Password reset not successfull.");
            }
            return result;
        }
        public async Task<bool> IsUserExists(string userName)
        {

            var allDoctors = await _doctorProfileRepository.GetListAsync();
            if (allDoctors.Any())
            {
                var doctor = allDoctors.FirstOrDefault(d => d.MobileNo == userName);
                if (doctor != null && doctor.Id > 0)
                {
                    return true;
                }
                else
                {
                    var allPatient = await _patientProfileRepository.GetListAsync();
                    if (allPatient.Any())
                    {
                        var patient = allPatient.FirstOrDefault(p => p.MobileNo == userName);
                        if (patient != null && patient.Id > 0)
                        {
                            return true;
                        }
                        else
                        {
                            var allAgent = await _agentProfileRepository.GetListAsync();
                            if (allAgent.Any())
                            {
                                var agent = allAgent.FirstOrDefault(a => a.MobileNo == userName);
                                if (agent != null && agent.Id > 0)
                                {
                                    return true;
                                }
                                else { return false; }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public async Task<AccountDeteleResponsesDto> DeleteAsync(long id, string role)
        {
            var response = new AccountDeteleResponsesDto();
            if (role == "Doctor")
            {
                var doctorDelete = _doctorProfileRepository.DeleteAsync(d => d.Id == id);
                if (doctorDelete != null)
                {
                    response.Success = true;
                    response.Message = "User Account removed";
                }
            }
            else if (role == "Patient")
            {
                var doctorDelete = _patientProfileRepository.DeleteAsync(d => d.Id == id);
                if (doctorDelete != null)
                {
                    response.Success = true;
                    response.Message = "User Account removed";
                }
            }
            return response;
        }
    }
}

