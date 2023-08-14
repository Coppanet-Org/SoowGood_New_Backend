using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DoctorScheduleService : SoowGoodWebAppService, IDoctorScheduleService
    {
        private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorScheduleService(IRepository<DoctorSchedule> doctorScheduleRepository,
            IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<ResponseDto> CreateAsync(DoctorScheduleInputDto input)
        {
            var response = new ResponseDto();
            var br = 0;
            try
            {
                if (input.Id == 0)
                {
                    var newEntity = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

                    var doctorSchedule = await _doctorScheduleRepository.InsertAsync(newEntity);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(doctorSchedule);
                    if (result is { Id: > 0 })
                    {
                        if (input.DoctorScheduleDaySession?.Count > 0)
                        {
                            foreach (var i in input.DoctorScheduleDaySession)
                            {
                                var session = new DoctorScheduleDaySessionInputDto
                                {
                                    DoctorScheduleId = result.Id,
                                    ScheduleDayofWeek = i.ScheduleDayofWeek,
                                    StartTime = i.StartTime,
                                    EndTime = i.EndTime,
                                    NoOfPatients = i.NoOfPatients,
                                    IsActive = i.IsActive,
                                    //CreationTime = i.CreationTime,
                                    //CreatorId = i.CreatorId,
                                    //LastModificationTime = i.LastModificationTime,
                                    //LastModifierId = i.LastModifierId,
                                    //IsDeleted = i.IsDeleted,
                                    //DeleterId = i.DeleterId,
                                    //DeletionTime = i.DeletionTime
                                };

                                var sessionResult = CreateSessionAsync(session);
                                if (sessionResult.Result.Success == true)
                                {
                                    br++;
                                }
                                else
                                {
                                    response.Id = 0;
                                    response.Value = sessionResult.Result.Value;
                                    response.Success = false;
                                    response.Message = sessionResult.Result.Message;
                                    return response;
                                    //break;                                    
                                }
                            }
                            if (br == input.DoctorScheduleDaySession?.Count)
                            {
                                response.Id = result.Id;
                                response.Value = "Schedule & Session Created";
                                response.Success = true;
                                response.Message = "Your Schedules & Sessions Created Successfully";
                            }
                        }
                        else
                        {
                            response.Id = 0;
                            response.Value = "Failed to Create Schedules and Sessions";
                            response.Success = false;
                            response.Message = "Failed to Create Your Schedule and session. Session infos not inserted";
                        }
                    }
                    else
                    {
                        response.Id = 0;
                        response.Value = "Failed to Create Schedule.";
                        response.Success = false;
                        response.Message = "Failed to Create Your Schedule.";
                    }
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
        public async Task<ResponseDto> CreateSessionAsync(DoctorScheduleDaySessionInputDto inputDto)
        {
            var response = new ResponseDto();
            try
            {
                var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(inputDto);
                var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
                if (result is { Id: > 0 })
                {
                    response.Id = result.Id;
                    response.Value = "Session created";
                    response.Success = true;
                    response.Message = "The Session Create successfully.";
                }
                else
                {
                    response.Id = 0;
                    response.Value = "Failed to Create Session";
                    response.Success = false;
                    response.Message = "Failed to Create Session.";
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
        public async Task<ResponseDto> UpdateAsync(DoctorScheduleInputDto input)
        {
            var response = new ResponseDto();
            var br = 0;
            var totalItemCount = 0;
            try
            {
                var updateItem = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);
                var item = await _doctorScheduleRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
                if (result is { Id: > 0 })
                {
                    if (input.DoctorScheduleDaySession?.Count > 0)
                    {
                        foreach (var i in input.DoctorScheduleDaySession)
                        {
                            Task<ResponseDto>? sessionResult;
                            if (i.Id > 0)
                            {
                                totalItemCount += input.DoctorScheduleDaySession.Count(c => c.Id > 0);
                                
                                sessionResult = UpdateSessionAsync(i);
                                if (sessionResult.Result.Success == true)
                                {
                                    br++;
                                }
                                else
                                {
                                    response.Id = 0;
                                    response.Value = sessionResult.Result.Value;
                                    response.Success = false;
                                    response.Message = sessionResult.Result.Message;
                                    return response;
                                    //break;                                    
                                }
                            }
                            else
                            {
                                totalItemCount += input.DoctorScheduleDaySession.Count(c => c.Id == 0);
                                var session = new DoctorScheduleDaySessionInputDto
                                {
                                    DoctorScheduleId = result.Id,
                                    ScheduleDayofWeek = i.ScheduleDayofWeek,
                                    StartTime = i.StartTime,
                                    EndTime = i.EndTime,
                                    NoOfPatients = i.NoOfPatients,
                                    IsActive = i.IsActive
                                };

                                sessionResult = CreateSessionAsync(session);
                                if (sessionResult.Result.Success == true)
                                {
                                    br++;
                                }
                                else
                                {
                                    response.Id = 0;
                                    response.Value = sessionResult.Result.Value;
                                    response.Success = false;
                                    response.Message = sessionResult.Result.Message;
                                    return response;
                                    //break;                                    
                                }
                            }
                        }
                        if (br == totalItemCount)
                        {
                            response.Id = result.Id;
                            response.Value = "Schedule & Session Updated.";
                            response.Success = true;
                            response.Message = "Your Schedules & Sessions Updated Successfully.";
                        }
                    }
                    else
                    {
                        response.Id = 0;
                        response.Value = "Failed to Updated Schedules and Sessions.";
                        response.Success = false;
                        response.Message = "Failed to Updated your Schedules and Sessions.";
                    }
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

            return response;//ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }
        public async Task<ResponseDto> UpdateSessionAsync(DoctorScheduleDaySessionInputDto inputDto)
        {
            var response = new ResponseDto();
            try
            {
                var updateItem = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(inputDto);

                var item = await _doctorScheduleSessionRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(item);
                if (result is { Id: > 0 })
                {
                    response.Id = result.Id;
                    response.Value = "Session Updated";
                    response.Success = true;
                    response.Message = "The Session Updated successfully.";
                }
                else
                {
                    response.Id = 0;
                    response.Value = "Failed to Update Session";
                    response.Success = false;
                    response.Message = "Failed to Update Session.";
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
        public async Task<DoctorScheduleDto?> GetAsync(int id)
        {
            var item = await _doctorScheduleRepository.WithDetailsAsync(s => s.DoctorScheduleDaySession);
            var schedule = item.FirstOrDefault(x => x.Id == id);
            var result = schedule != null ? ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(schedule) : null;

            return result;
        }
        public async Task<List<DoctorScheduleDto>> GetListAsync()
        {
            var profiles = await _doctorScheduleRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(profiles);
        }

        public async Task<List<DoctorScheduleDto>> GetListByDoctorIdListAsync(long doctorId)
        {
            var item = await _doctorScheduleRepository.GetListAsync(s => s.DoctorProfileId == doctorId);//.WithDetailsAsync(s => s.DoctorScheduleDaySession);
            //var schedules = item.Where(s => s.DoctorProfileId == doctorId).ToList();
            return ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(item);
        }
    }
}
