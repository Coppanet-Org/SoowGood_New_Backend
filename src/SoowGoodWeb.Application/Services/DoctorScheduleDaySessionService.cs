using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DoctorScheduleDaySessionService : SoowGoodWebAppService, IDoctorScheduleDaySessionService//, IDoctorScheduleDayOffService
    {
        
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorScheduleDaySessionService(IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository, IUnitOfWorkManager unitOfWorkManager)
        {   
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<ResponseDto> CreateAsync(DoctorScheduleDaySessionInputDto input)
        {
            var response = new ResponseDto();
            try
            {

                var newEntity = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(input);

                var doctorSchedule = await _doctorScheduleSessionRepository.InsertAsync(newEntity);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                //DoctorScheduleDaySessionDto? sessionResult = null;
                var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(doctorSchedule);
                if (result != null && result.Id > 0)
                {
                    //if (input.DoctorScheduleDaySessions?.Count > 0)
                    //{
                    //    foreach (var i in input.DoctorScheduleDaySessions)
                    //    {
                    //        var session = new DoctorScheduleDaySessionInputDto();

                    //        session.DoctorScheduleId = result.Id;
                    //        session.ScheduleDayofWeek = i.ScheduleDayofWeek;
                    //        session.StartTime = i.StartTime;
                    //        session.EndTime = i.EndTime;
                    //        session.NoOfPatients = i.NoOfPatients;
                    //        session.IsActive = i.IsActive;
                    //        session.CreationTime = i.CreationTime;
                    //        session.CreatorId = i.CreatorId;
                    //        session.LastModificationTime = i.LastModificationTime;
                    //        session.LastModifierId = i.LastModifierId;
                    //        session.IsDeleted = i.IsDeleted;
                    //        session.DeleterId = i.DeleterId;
                    //        session.DeletionTime = i.DeletionTime;


                    //        var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(session);
                    //        var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
                    //        await _unitOfWorkManager.Current.SaveChangesAsync();
                    //        sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
                    //    }
                    //    if (sessionResult != null && sessionResult.Id > 0)
                    //    {
                    response.Id = result?.Id;
                    response.Value = "Schedule and Session Created";
                    response.Success = true;
                    response.Message?.Add("Doctor Schedule created successfully with sessions.");
                    //    }
                    //    else
                    //    {
                    //        response.Id = result?.Id;
                    //        response.Value = "Schedule with Session creation failed";
                    //        response.Success = false;
                    //        response.Message?.Add("Doctor Schedule creation failed. Error from Session");
                    //    }
                    //}
                    //else
                    //{
                    //    response.Id = result?.Id;
                    //    response.Value = "Schedule with Session creation failed";
                    //    response.Success = false;
                    //    response.Message?.Add("Doctor Schedule creation failed. Session values nill");
                    //}

                }
                else
                {
                    response.Id = result?.Id;
                    response.Value = "Schedule Session creation failed";
                    response.Success = false;
                    response.Message?.Add("Doctor Schedule Session creation failed.");
                }
                //return response;
                //ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(doctorSchedule);
            }
            catch (Exception ex)
            {
                response.Id = null;
                response.Value = "Schedule Session creation failed";
                response.Success = false;
                response.Message?.Add(ex.Message);
                //return response;
            }
            return response;
        }


        public async Task<DoctorScheduleDaySessionDto> GetAsync(int id)
        {
            var item = await _doctorScheduleSessionRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(item);
        }
        public async Task<List<DoctorScheduleDaySessionDto>> GetListAsync()
        {
            var profiles = await _doctorScheduleSessionRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorScheduleDaySession>, List<DoctorScheduleDaySessionDto>>(profiles);
        }
        //public async Task<DoctorScheduleDto> GetByUserIdAsync(Guid userId)
        //{
        //    var item = await _doctorScheduleRepository.GetAsync(x => x.UserId == userId);
        //    return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        //}

        public async Task<ResponseDto> UpdateAsync(DoctorScheduleDaySessionInputDto input)
        {
            var response = new ResponseDto();
            try
            {
                var updateItem = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(input);
                var item = await _doctorScheduleSessionRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                //DoctorScheduleDaySessionDto? sessionResult = null;
                var result = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(item);
                if (result != null && result.Id > 0)
                {
                    //if (input.DoctorScheduleDaySessions?.Count > 0)
                    //{
                    //foreach (var i in input.DoctorScheduleDaySessions)
                    //{
                    //    if (i.Id > 0)
                    //    {
                    //        var updateSessionItem = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(i);
                    //        var sessionItem = await _doctorScheduleSessionRepository.UpdateAsync(updateSessionItem);
                    //        await _unitOfWorkManager.Current.SaveChangesAsync();
                    //        sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(sessionItem);
                    //    }
                    //    else
                    //    {

                    //        var session = new DoctorScheduleDaySessionInputDto();

                    //        session.DoctorScheduleId = result.Id;
                    //        session.ScheduleDayofWeek = i.ScheduleDayofWeek;
                    //        session.StartTime = i.StartTime;
                    //        session.EndTime = i.EndTime;
                    //        session.NoOfPatients = i.NoOfPatients;
                    //        session.IsActive = i.IsActive;
                    //        session.CreationTime = i.CreationTime;
                    //        session.CreatorId = i.CreatorId;
                    //        session.LastModificationTime = i.LastModificationTime;
                    //        session.LastModifierId = i.LastModifierId;
                    //        session.IsDeleted = i.IsDeleted;
                    //        session.DeleterId = i.DeleterId;
                    //        session.DeletionTime = i.DeletionTime;


                    //        var newSession = ObjectMapper.Map<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>(session);
                    //        var scheduleSession = await _doctorScheduleSessionRepository.InsertAsync(newSession);
                    //        await _unitOfWorkManager.Current.SaveChangesAsync();
                    //        sessionResult = ObjectMapper.Map<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>(scheduleSession);
                    //    }
                    //}
                    //if (sessionResult != null && sessionResult.Id > 0)
                    //{
                    response.Id = result?.Id;
                    response.Value = "Schedule with Session Updated";
                    response.Success = true;
                    response.Message?.Add("Doctor Schedule updated successfully with sessions.");
                    //}
                    //else
                    //{
                    //    response.Id = result?.Id;
                    //    response.Value = "Schedule with Session creation failed";
                    //    response.Success = false;
                    //    response.Message?.Add("Doctor Schedule creation failed. Error from Session");
                    //}
                    //}
                    //else
                    //{
                    //    response.Id = result?.Id;
                    //    response.Value = "Schedule with Session creation failed";
                    //    response.Success = false;
                    //    response.Message?.Add("Doctor Schedule creation failed. Session values nill");
                    //}
                }
                else
                {
                    response.Id = 0;
                    response.Value = "Schedule session Can not update";
                    response.Success = false;
                    response.Message?.Add("Doctor Schedule session Update failed.");
                }
            }
            catch (Exception ex)
            {
                response.Id = null;
                response.Value = "Schedule Schedule session Update failed";
                response.Success = false;
                response.Message?.Add(ex.Message);
            }

            return response;//ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }
        
    }
}
