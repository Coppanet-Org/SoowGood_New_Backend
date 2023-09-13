using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoowGoodWeb.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp.ObjectMapping;
using Scriban.Syntax;

namespace SoowGoodWeb.Services
{
    public class AppointmentService : SoowGoodWebAppService, IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        //private readonly IRepository<DoctorChamber> _doctorChamberRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AppointmentService(IRepository<Appointment> appointmentRepository,
            //IRepository<DoctorChamber> doctorChamberRepository,
            IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _appointmentRepository = appointmentRepository;
            //_doctorScheduleRepository = doctorScheduleRepository;
            //_doctorChamberRepository = doctorChamberRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            //_unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<ResponseDto> CreateAsync(AppointmentInputDto input)
        {
            var response = new ResponseDto();
            //var br = 0;
            //try
            //{
            //    if (input.Id == 0)
            //    {
            //        //input.ScheduleName = input.cham;
            //        if(input.DoctorChamberId == 0)
            //        {
            //            input.DoctorChamberId = null;
            //            input.ScheduleName = ((ConsultancyType)input?.ConsultancyType!).ToString();
            //        }
            //        else
            //        {
            //            var chName = _doctorChamberRepository.FirstOrDefaultAsync(c => c.Id == input.DoctorChamberId);
            //            input.ScheduleName= ((ConsultancyType)input?.ConsultancyType!).ToString() + '_' + chName.Result?.ChamberName?.ToString();
            //        }
            //        var newEntity = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

            //        var doctorSchedule = await _doctorScheduleRepository.InsertAsync(newEntity);
            //        await _unitOfWorkManager.Current.SaveChangesAsync();
            //        var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(doctorSchedule);
            //        if (result is { Id: > 0 })
            //        {
            //            //if (input.DoctorScheduleDaySession?.Count > 0)
            //            //{
            //            //    foreach (var i in input.DoctorScheduleDaySession)
            //            //    {
            //            //        var session = new DoctorScheduleDaySessionInputDto
            //            //        {
            //            //            DoctorScheduleId = result.Id,
            //            //            ScheduleDayofWeek = i.ScheduleDayofWeek,
            //            //            StartTime = i.StartTime,
            //            //            EndTime = i.EndTime,
            //            //            NoOfPatients = i.NoOfPatients,
            //            //            IsActive = i.IsActive,
            //            //            //CreationTime = i.CreationTime,
            //            //            //CreatorId = i.CreatorId,
            //            //            //LastModificationTime = i.LastModificationTime,
            //            //            //LastModifierId = i.LastModifierId,
            //            //            //IsDeleted = i.IsDeleted,
            //            //            //DeleterId = i.DeleterId,
            //            //            //DeletionTime = i.DeletionTime
            //            //        };

            //            //        var sessionResult = CreateSessionAsync(session);
            //            //        if (sessionResult.Result.Success == true)
            //            //        {
            //            //            br++;
            //            //        }
            //            //        else
            //            //        {
            //            //            response.Id = 0;
            //            //            response.Value = sessionResult.Result.Value;
            //            //            response.Success = false;
            //            //            response.Message = sessionResult.Result.Message;
            //            //            return response;
            //            //            //break;                                    
            //            //        }
            //            //    }

            //            //if (br == input.DoctorScheduleDaySession?.Count)
            //            //{
            //            response.Id = result.Id;
            //            response.Value = "Schedule & Session Created";
            //            response.Success = true;
            //            response.Message = "Your Schedules & Sessions Created Successfully";
            //            //}
            //            //}
            //            //else
            //            //{
            //            //    response.Id = 0;
            //            //    response.Value = "Failed to Create Schedules and Sessions";
            //            //    response.Success = false;
            //            //    response.Message = "Failed to Create Your Schedule and session. Session infos not inserted";
            //            //}
            //        }
            //        else
            //        {
            //            response.Id = 0;
            //            response.Value = "Failed to Create Schedule.";
            //            response.Success = false;
            //            response.Message = "Failed to Create Your Schedule.";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    response.Id = null;
            //    response.Value = "Exception";
            //    response.Success = false;
            //    response.Message = ex.Message;
            //}

            return response;
        }

        public async Task<ResponseDto> UpdateAsync(AppointmentInputDto input)
        {
            var response = new ResponseDto();
            //try
            //{
            //    if (input.DoctorChamberId == 0)
            //    {
            //        input.DoctorChamberId = null;
            //        input.ScheduleName = ((ConsultancyType)input?.ConsultancyType!).ToString();
            //    }
            //    else
            //    {
            //        var chName = _doctorChamberRepository.FirstOrDefaultAsync(c => c.Id == input.DoctorChamberId);
            //        input.ScheduleName = ((ConsultancyType)input?.ConsultancyType!).ToString() + '_' + chName.Result?.ChamberName?.ToString();
            //    }
            //    var updateItem = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);
            //    var item = await _doctorScheduleRepository.UpdateAsync(updateItem);
            //    await _unitOfWorkManager.Current.SaveChangesAsync();
            //    var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
            //    if (result is { Id: > 0 })
            //    {
            //        //if (input.DoctorScheduleDaySession?.Count > 0)
            //        //{
            //        //    foreach (var i in input.DoctorScheduleDaySession)
            //        //    {
            //        //        Task<ResponseDto>? sessionResult;
            //        //        if (i.Id > 0)
            //        //        {
            //        //            totalItemCount += input.DoctorScheduleDaySession.Count(c => c.Id > 0);

            //        //            sessionResult = UpdateSessionAsync(i);
            //        //            if (sessionResult.Result.Success == true)
            //        //            {
            //        //                br++;
            //        //            }
            //        //            else
            //        //            {
            //        //                response.Id = 0;
            //        //                response.Value = sessionResult.Result.Value;
            //        //                response.Success = false;
            //        //                response.Message = sessionResult.Result.Message;
            //        //                return response;
            //        //                //break;                                    
            //        //            }
            //        //        }
            //        //        else
            //        //        {
            //        //            totalItemCount += input.DoctorScheduleDaySession.Count(c => c.Id == 0);
            //        //            var session = new DoctorScheduleDaySessionInputDto
            //        //            {
            //        //                DoctorScheduleId = result.Id,
            //        //                ScheduleDayofWeek = i.ScheduleDayofWeek,
            //        //                StartTime = i.StartTime,
            //        //                EndTime = i.EndTime,
            //        //                NoOfPatients = i.NoOfPatients,
            //        //                IsActive = i.IsActive
            //        //            };

            //        //            sessionResult = CreateSessionAsync(session);
            //        //            if (sessionResult.Result.Success == true)
            //        //            {
            //        //                br++;
            //        //            }
            //        //            else
            //        //            {
            //        //                response.Id = 0;
            //        //                response.Value = sessionResult.Result.Value;
            //        //                response.Success = false;
            //        //                response.Message = sessionResult.Result.Message;
            //        //                return response;
            //        //                //break;                                    
            //        //            }
            //        //        }
            //        //    }

            //        //if (br == totalItemCount)
            //        //{
            //        response.Id = result.Id;
            //        response.Value = "Schedule & Session Updated.";
            //        response.Success = true;
            //        response.Message = "Your Schedules & Sessions Updated Successfully.";
            //        // }
            //        //}
            //        //else
            //        //{
            //        //    response.Id = 0;
            //        //    response.Value = "Failed to Updated Schedules and Sessions.";
            //        //    response.Success = false;
            //        //    response.Message = "Failed to Updated your Schedules and Sessions.";
            //        //}
            //    }
            //    else
            //    {
            //        response.Id = result?.Id;
            //        response.Value = "Failed to Updated Schedule.";
            //        response.Success = false;
            //        response.Message = "Failed to Update Your Schedule.";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    response.Id = null;
            //    response.Value = "Update failed";
            //    response.Success = false;
            //    response.Message = ex.Message;
            //}

            return response; //ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }

        public async Task<AppointmentDto?> GetAsync(int id)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            var schedule = item.FirstOrDefault(x => x.Id == id);
            var result = schedule != null ? ObjectMapper.Map<Appointment, AppointmentDto>(schedule) : null;

            return result;
        }

        public async Task<List<AppointmentDto>> GetListAsync()
        {
            var appointments = await _appointmentRepository.GetListAsync();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<int> GetAppCountByScheduleIdSessionIdAsync(long scheduleId, long sessionId)
        {
            var appointments = await _appointmentRepository.GetListAsync(a => a.DoctorScheduleId == scheduleId && a.DoctorScheduleDaySessionId == sessionId);
            var appCount = appointments.Count();
            return appCount;
        }
        public async Task<int> GetLeftBookingCount(long sessionId, long scheduleId)
        {
            int resultNp = 0;
            var numberOfPatintforScheduleSession = await _doctorScheduleSessionRepository.GetAsync(s => s.Id == sessionId && s.DoctorScheduleId == scheduleId);
            int noOfPatients = (int)numberOfPatintforScheduleSession.NoOfPatients;

            int appCounts = await GetAppCountByScheduleIdSessionIdAsync(scheduleId, sessionId);
            if (noOfPatients == appCounts)
            {
                resultNp = 0;
            }
            else if(noOfPatients > appCounts)
            {
                resultNp = (noOfPatients - appCounts);
            }
            else
            {
                resultNp = noOfPatients;
            }

            return resultNp;//noOfPatients == appCounts? 0: (int)resultNp;
        }
    }
}
