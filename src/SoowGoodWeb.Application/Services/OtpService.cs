using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using SoowGoodWeb.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace SoowGoodWeb.Services
{
    //[Authorize]
    public class OtpService : SoowGoodWebAppService, IOtpService
    {

        private readonly IdentityUserManager _userManager;
        private readonly IRepository<Otp, int> _repository;
        private readonly ISmsService _smsService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public OtpService(IdentityUserManager userManager, IRepository<Otp, int> repository,
            ISmsService smsService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userManager = userManager;
            this._repository = repository;
            this._smsService = smsService;
            this._unitOfWorkManager = unitOfWorkManager;
        }


        //[HttpGet]
        //[AllowAnonymous]
        public async Task<bool> ApplyOtp(string clientKey, string mobileNo)
        {
            var isUserExists = await _userManager.FindByNameAsync(mobileNo);
            if (isUserExists == null)
            {
                if (!string.IsNullOrEmpty(clientKey) && clientKey.Equals("SoowGood_App", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(mobileNo))
                {
                    int otp = Utility.GetRandomNo(1000, 9999);
                    Otp otpEntity = new Otp();
                    otpEntity.OtpNo = otp;
                    otpEntity.MobileNo = mobileNo;
                    otpEntity.ExpireDateTime = DateTime.Now.AddMinutes(3);
                    otpEntity.OtpStatus = OtpStatus.New;
                    await _repository.InsertAsync(otpEntity);
                    // stp start
                    SmsRequestParamDto otpInput = new SmsRequestParamDto();
                    otpInput.Sms = String.Format(otp + " is your OTP to authenticate your Phone No. Do not share this OTP with anyone.");
                    otpInput.Msisdn = mobileNo;
                    otpInput.CsmsId = GenerateTransactionId(16);
                    try
                    {
                        var res = await _smsService.SendSmsGreenWeb(otpInput);
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                        throw new Exception(e.Message);
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        private static string GenerateTransactionId(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //[HttpGet]
        public async Task<bool> VarifyOtpAsync(int otp)
        {

            //if (otp > 0)
            //{
            //    var item = await _repository.FirstOrDefaultAsync(x => x.OtpNo == otp && x.OtpStatus == OtpStatus.New && x.ExpireDateTime >= DateTime.Now);
            //    if (item != null)
            //    {
            //        item.OtpStatus = OtpStatus.Varified;
            //        await _repository.UpdateAsync(item);
            //        await _unitOfWorkManager.Current.SaveChangesAsync();
            return true;
            //    }
            //}
            //return false;

        }

        public async Task<OtpDto> UpdateAsync(OtpDto input)
        {
            var item = await _repository.FirstOrDefaultAsync(x => x.OtpNo == input.OtpNo && x.ExpireDateTime >= DateTime.Now);
            if (item != null)
            {
                item.OtpStatus = input.OtpStatus;
                await _repository.UpdateAsync(item);
                return ObjectMapper.Map<Otp, OtpDto>(item);
            }
            return input;
        }


    }
}


