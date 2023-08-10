using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoowGoodWeb.Services
{
    public class DoctorScheduleService : SoowGoodWebAppService, IDoctorScheduleService//, IDoctorScheduleDayOffService
    {
        private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorScheduleService(IRepository<DoctorSchedule> doctorScheduleRepository, IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<ResponseDto> CreateAsync(DoctorScheduleInputDto input)
        {
            var response = new ResponseDto();
            try
            {
                if(input.Id>0)
                {
                    DoctorScheduleInputDto upInputDto = input;
                }
                if (input.Id == 0)
                {
                    var newEntity = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

                    var doctorSchedule = await _doctorScheduleRepository.InsertAsync(newEntity);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    DoctorScheduleDaySessionDto? sessionResult = null;
                    var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(doctorSchedule);
                    if (result != null && result.Id > 0)
                    {
                        if (input.DoctorScheduleDaySessions?.Count > 0)
                        {
                            foreach (var i in input.DoctorScheduleDaySessions)
                            {
                                var session = new DoctorScheduleDaySessionInputDto();

                                session.DoctorScheduleId = result.Id;
                                session.ScheduleDayofWeek = i.ScheduleDayofWeek;
                                session.StartTime = i.StartTime;
                                session.EndTime = i.EndTime;
                                session.NoOfPatients = i.NoOfPatients;
                                session.IsActive = i.IsActive;
                                session.CreationTime = i.CreationTime;
                                session.CreatorId = i.CreatorId;
                                session.LastModificationTime = i.LastModificationTime;
                                session.LastModifierId = i.LastModifierId;
                                session.IsDeleted = i.IsDeleted;
                                session.DeleterId = i.DeleterId;
                                session.DeletionTime = i.DeletionTime;


                                var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(session);
                                var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
                                await _unitOfWorkManager.Current.SaveChangesAsync();
                                sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
                            }
                            if (sessionResult != null && sessionResult.Id > 0)
                            {
                                response.Id = result?.Id;
                                response.Value = "Schedule With Session created";
                                response.Success = true;
                                response.Message?.Add("Doctor Schedule with Session created successfully.");
                            }
                            else
                            {
                                response.Id = result?.Id;
                                response.Value = "Schedule with Session creation failed";
                                response.Success = false;
                                response.Message?.Add("Doctor Schedule with Session creation failed. Error from Session");
                            }
                        }
                        else
                        {
                            response.Id = 0;
                            response.Value = "Schedule with Session creation failed";
                            response.Success = false;
                            response.Message?.Add("Doctor Schedule creation failed. Session values should not nill");
                        }
                    }
                    else
                    {
                        response.Id = 0;
                        response.Value = "Schedule creation failed";
                        response.Success = false;
                        response.Message?.Add("Doctor Schedule creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Id = null;
                response.Value = "Exception";
                response.Success = false;
                response.Message?.Add(ex.Message);
            }
            return response;
        }


        public async Task<DoctorScheduleDto> GetAsync(int id)
        {
            var item = await _doctorScheduleRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }
        public async Task<List<DoctorScheduleDto>> GetListAsync()
        {
            var profiles = await _doctorScheduleRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(profiles);
        }
        //public async Task<DoctorScheduleDto> GetByUserIdAsync(Guid userId)
        //{
        //    var item = await _doctorScheduleRepository.GetAsync(x => x.UserId == userId);
        //    return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        //}        

        //public async Task<ResponseDto> UpdateAsync(DoctorScheduleInputDto input)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var updateItem = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

        //        var item = await _doctorScheduleRepository.UpdateAsync(updateItem);
        //        await _unitOfWorkManager.Current.SaveChangesAsync();

        //        var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);

        //        if (result != null && result.Id > 0)
        //        {
        //            response.Id = result?.Id;
        //            response.Value = "Schedule Updated";
        //            response.Success = true;
        //            response.Message?.Add("Doctor Schedule updated.");
        //        }
        //        else
        //        {
        //            response.Id = 0;
        //            response.Value = "Schedule update failed";
        //            response.Success = false;
        //            response.Message?.Add("Doctor Schedule update failed.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Id = null;
        //        response.Value = "Exception";
        //        response.Success = false;
        //        response.Message?.Add(ex.Message);
        //    }
        //    return response;//ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        //}

        public async Task<ResponseDto> UpdateAsync(DoctorScheduleInputDto input)
        {
            var response = new ResponseDto();
            try
            {
                //dt = datetime ?? DateTime.Now;
                var updateItem = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);
                var item = await _doctorScheduleRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                DoctorScheduleDaySessionDto? sessionResult = null;
                var result = ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
                if (result != null && result.Id > 0)
                {
                    if (input.DoctorScheduleDaySessions?.Count > 0)
                    {
                        foreach (var i in input.DoctorScheduleDaySessions)
                        {
                            if (i.Id > 0)
                            {
                                i.StartTime = TimeZoneInfo.ConvertTimeFromUtc((i.StartTime ?? DateTime.Now), TimeZoneInfo.FindSystemTimeZoneById(""));
                                i.EndTime = TimeZoneInfo.ConvertTimeFromUtc((i.EndTime ?? DateTime.Now), TimeZoneInfo.FindSystemTimeZoneById(""));
                                //TimeSpan time = new TimeSpan(36, 0, 0, 0);
                                //DateTime combined = i.StartTime.Add(time);
                                //i.StartTime = DateTime.Parse(i.StartTime?.TimeOfDay.ToString(), CultureInfo.InvariantCulture,
                                //          DateTimeStyles.NoCurrentDateDefault);// i.StartTime;
                                var updateSessionItem = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(i);
                                var sessionItem = await _doctorScheduleSessionRepository.UpdateAsync(updateSessionItem);
                                await _unitOfWorkManager.Current.SaveChangesAsync();
                                sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(sessionItem);
                            }
                            else
                            {

                                var session = new DoctorScheduleDaySessionInputDto();

                                session.DoctorScheduleId = result.Id;
                                session.ScheduleDayofWeek = i.ScheduleDayofWeek;
                                session.StartTime = i.StartTime;
                                session.EndTime = i.EndTime;
                                session.NoOfPatients = i.NoOfPatients;
                                session.IsActive = i.IsActive;
                                session.CreationTime = i.CreationTime;
                                session.CreatorId = i.CreatorId;
                                session.LastModificationTime = i.LastModificationTime;
                                session.LastModifierId = i.LastModifierId;
                                session.IsDeleted = i.IsDeleted;
                                session.DeleterId = i.DeleterId;
                                session.DeletionTime = i.DeletionTime;


                                var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(session);
                                var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
                                await _unitOfWorkManager.Current.SaveChangesAsync();
                                sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
                            }
                        }
                        if (sessionResult != null && sessionResult.Id > 0)
                        {
                            response.Id = result?.Id;
                            response.Value = "Schedule with Session Created";
                            response.Success = true;
                            response.Message?.Add("Doctor Schedule created successfully with sessions.");
                        }
                        else
                        {
                            response.Id = result?.Id;
                            response.Value = "Schedule with Session creation failed";
                            response.Success = false;
                            response.Message?.Add("Doctor Schedule creation failed. Error from Session");
                        }
                    }
                    else
                    {
                        response.Id = result?.Id;
                        response.Value = "Schedule with Session creation failed";
                        response.Success = false;
                        response.Message?.Add("Doctor Schedule creation failed. Session values nill");
                    }
                }
                else
                {
                    response.Id = result?.Id;
                    response.Value = "Schedule with Session creation failed";
                    response.Success = false;
                    response.Message?.Add("Doctor Schedule creation failed. Error in Schedule insertion.");
                }
            }
            catch (Exception ex)
            {

            }

            return response;//ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }
    }
}
