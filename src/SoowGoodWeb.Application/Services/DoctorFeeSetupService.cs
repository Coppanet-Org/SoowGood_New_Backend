﻿using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoowGoodWeb.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DoctorFeeSetupService : SoowGoodWebAppService, IDoctorFeeSetupService
    {
        private readonly IRepository<DoctorFeesSetup> _doctorFeeRepository;
        //private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DoctorFeeSetupService(IRepository<DoctorFeesSetup> doctorFeeRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _doctorFeeRepository = doctorFeeRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<ResponseDto> CreateAsync(DoctorFeesSetupInputDto input)
        {
            var response = new ResponseDto();
            try
            {
                var newEntity = ObjectMapper.Map<DoctorFeesSetupInputDto, DoctorFeesSetup>(input);

                var doctorSchedule = await _doctorFeeRepository.InsertAsync(newEntity);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<DoctorFeesSetup, DoctorFeesSetupDto>(doctorSchedule);
                if (result is { Id: > 0 })
                {
                    response.Id = result.Id;
                    response.Value = "Fee Created";
                    response.Success = true;
                    response.Message = "Your Visiting Fee Created Successfully";
                }
                else
                {
                    response.Id = 0;
                    response.Value = "Failed to Create Fee.";
                    response.Success = false;
                    response.Message = "Failed to Create Your Visiting Fee.";
                }
            }
            catch (Exception ex)
            {
                response.Id = null;
                response.Value = "Exception";
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> UpdateAsync(DoctorFeesSetupInputDto input)
        {
            var response = new ResponseDto();
            try
            {
                var updateItem = ObjectMapper.Map<DoctorFeesSetupInputDto, DoctorFeesSetup>(input);
                var item = await _doctorFeeRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<DoctorFeesSetup, DoctorFeesSetupDto>(item);
                if (result is { Id: > 0 })
                {

                    response.Id = result.Id;
                    response.Value = "Schedule & Session Updated.";
                    response.Success = true;
                    response.Message = "Your Schedules & Sessions Updated Successfully.";
                }
                else
                {
                    response.Id = result?.Id;
                    response.Value = "Failed to Updated Schedule.";
                    response.Success = false;
                    response.Message = "Failed to Update Your Schedule.";
                }
            }
            catch (Exception ex)
            {
                response.Id = null;
                response.Value = "Update failed";
                response.Success = false;
                response.Message = ex.Message;
            }

            return response; //ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }

        public async Task<DoctorFeesSetupDto?> GetAsync(int id)
        {
            var item = await _doctorFeeRepository.WithDetailsAsync();
            var schedule = item.FirstOrDefault(x => x.Id == id);
            var result = schedule != null ? ObjectMapper.Map<DoctorFeesSetup, DoctorFeesSetupDto>(schedule) : null;

            return result;
        }

        public async Task<List<DoctorFeesSetupDto>> GetListAsync()
        {
            var profiles = await _doctorFeeRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorFeesSetup>, List<DoctorFeesSetupDto>>(profiles);
        }

        public async Task<List<DoctorFeesSetupDto>?> GetListByDoctorIdListAsync(long doctorId)
        {
            List<DoctorFeesSetupDto>? result = null;
            var allFees = await _doctorFeeRepository.WithDetailsAsync(s => s.DoctorSchedule, d=>d.DoctorSchedule.DoctorChamber);
            var item = allFees.Where(s => s.DoctorSchedule != null && s.DoctorSchedule.DoctorProfileId == doctorId);
            if (!item.Any())
            {
                return result; // ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(schedules);
            }

            result = new List<DoctorFeesSetupDto>();
            foreach (var fee in item)
            {
                result.Add(new DoctorFeesSetupDto()
                {
                    Id = fee.Id,
                    DoctorScheduleId = fee.DoctorScheduleId,
                    DoctorSchedule = ((ConsultancyType)fee?.DoctorSchedule?.ConsultancyType!).ToString()
                                     + "_" + (fee.DoctorSchedule?.DoctorChamberId > 0 ? fee?.DoctorSchedule.DoctorChamber?.ChamberName : ""),
                    AppointmentType = fee.AppointmentType,
                    AppointmentTypeName=((AppointmentType)fee.AppointmentType).ToString(),
                    CurrentFee = fee.CurrentFee,
                    FeeAppliedFrom = fee.FeeAppliedFrom, 
                    PreviousFee=fee.PreviousFee,

                    Discount = fee.Discount,
                    DiscountAppliedFrom = fee.DiscountAppliedFrom,
                    DiscountPeriod = fee.DiscountPeriod,
                    FollowUpPeriod = fee.FollowUpPeriod,
                    ReportShowPeriod = fee.ReportShowPeriod,
                    TotalFee = fee.TotalFee,
                    //ScheduleTypeName = schedule.ScheduleType > 0
                    //    ? ((ScheduleType)schedule.ScheduleType).ToString()
                    //    : "N/A",
                    //IsActive = schedule.IsActive,
                    //Status = schedule.IsActive == true ? "Open" : "Close",
                    //OffDayFrom = schedule.IsActive == false ? schedule.OffDayFrom : null,
                    //OffDayTo = schedule.IsActive == false ? schedule.OffDayTo : null,
                    //Remarks = schedule.IsActive == false
                    //    ? ("Chamber is closed from " + schedule.OffDayFrom.ToString() + " to" +
                    //       schedule.OffDayFrom.ToString())
                    //    : "Chamber is Open"
                });
            }

            return result; // ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(schedules);
        }

        //public async Task<ResponseDto> DeleteSessionAsync(long id)
        //{
        //    var response = new ResponseDto();
        //    await _doctorScheduleSessionRepository.DeleteAsync(s => s.Id == id);
        //    response.Id = id;
        //    response.Value = "Session Deleted";
        //    response.Success = true;
        //    response.Message = "Session Deleted Permanently";

        //    return response;
        //}

        //public async Task<ResponseDto> CreateSessionAsync(DoctorScheduleDaySessionInputDto inputDto)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(inputDto);
        //        var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
        //        await _unitOfWorkManager.Current.SaveChangesAsync();
        //        var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
        //        if (result is { Id: > 0 })
        //        {
        //            response.Id = result.Id;
        //            response.Value = "Session created";
        //            response.Success = true;
        //            response.Message = "The Session Create successfully.";
        //        }
        //        else
        //        {
        //            response.Id = 0;
        //            response.Value = "Failed to Create Session";
        //            response.Success = false;
        //            response.Message = "Failed to Create Session.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Id = null;
        //        response.Value = "Exception";
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}
        //public async Task<ResponseDto> UpdateSessionAsync(DoctorScheduleDaySessionInputDto inputDto)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var updateItem = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(inputDto);

        //        var item = await _doctorScheduleSessionRepository.UpdateAsync(updateItem);
        //        await _unitOfWorkManager.Current.SaveChangesAsync();
        //        var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(item);
        //        if (result is { Id: > 0 })
        //        {
        //            response.Id = result.Id;
        //            response.Value = "Session Updated";
        //            response.Success = true;
        //            response.Message = "The Session Updated successfully.";
        //        }
        //        else
        //        {
        //            response.Id = 0;
        //            response.Value = "Failed to Update Session";
        //            response.Success = false;
        //            response.Message = "Failed to Update Session.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Id = null;
        //        response.Value = "Exception";
        //        response.Success = false;
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}
    }
}