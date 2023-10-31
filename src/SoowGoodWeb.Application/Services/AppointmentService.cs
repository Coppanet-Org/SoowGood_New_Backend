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
using SoowGoodWeb.InputDto;
using SoowGoodWeb.SslCommerz;
using System.Collections;
using AgoraIO.Media;
using System.Security.Principal;

namespace SoowGoodWeb.Services
{
    public class AppointmentService : SoowGoodWebAppService, IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        //private readonly IRepository<DoctorChamber> _doctorChamberRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IRepository<PatientProfile> _patientProfileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly SslCommerzGatewayManager _sslCommerzGatewayManager;


        private uint _expireTimeInSeconds = 3600;
        public AppointmentService(IRepository<Appointment> appointmentRepository,
            //IRepository<DoctorChamber> doctorChamberRepository,
            IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository,
            IRepository<PatientProfile> patientProfileRepository,
            SslCommerzGatewayManager sslCommerzGatewayManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _appointmentRepository = appointmentRepository;
            //_doctorScheduleRepository = doctorScheduleRepository;
            //_doctorChamberRepository = doctorChamberRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            _patientProfileRepository = patientProfileRepository;
            _sslCommerzGatewayManager = sslCommerzGatewayManager;

            //_unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentInputDto input)
        {
            //var response = new ResponseDto();
            //return response;
            if (input.DoctorScheduleId > 0 && input.DoctorScheduleDaySessionId > 0)
            {
                var mainSession = await _doctorScheduleSessionRepository.GetAsync(s => s.Id == input.DoctorScheduleDaySessionId && s.DoctorScheduleId == input.DoctorScheduleId);
                var stTime = Convert.ToDateTime(mainSession.StartTime);
                var enTime = Convert.ToDateTime(mainSession.EndTime);
                var totalhr = (enTime - stTime).TotalHours; //Convert.ToDateTime(mainSession.EndTime) - Convert.ToDateTime(mainSession.StartTime);
                var hrmnt = totalhr * 60;
                var slotPerPatient = hrmnt / mainSession.NoOfPatients;
                string[] slots = null;// = new string[0];
                List<string> list = new List<string>();
                //int durationOfSession = 60;
                //int gapBetweenSessions = 30;
                //DateTime start = DateTime.Today.AddHours(8);
                //DateTime end = DateTime.Today.AddHours(18);

                for (DateTime appointment = stTime; appointment < enTime; appointment = appointment.AddMinutes((double)slotPerPatient))
                {
                    list.Add(appointment.ToString("HH:mm"));
                    slots = list.ToArray();
                }

                long lastSerial = await GetAppCountByScheduleIdSessionIdAsync(input.DoctorScheduleId, input.DoctorScheduleDaySessionId);

                for (long i = lastSerial; i < mainSession.NoOfPatients; ++i)
                {
                    input.AppointmentTime = slots != null ? slots[i].ToString() : "";
                    break;
                }
                input.AppointmentSerial = (lastSerial + 1).ToString();
                input.AppointmentCode = input.DoctorCode + "-" + input.PatientCode + "-" + input.AppointmentSerial;
            }
            var newEntity = ObjectMapper.Map<AppointmentInputDto, Appointment>(input);

            var doctorChamber = await _appointmentRepository.InsertAsync(newEntity);

            //await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<Appointment, AppointmentDto>(doctorChamber);
        }

        public async Task<AppointmentDto> UpdateAsync(AppointmentInputDto input)
        {
            var updateItem = ObjectMapper.Map<AppointmentInputDto, Appointment>(input);

            var item = await _appointmentRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<Appointment, AppointmentDto>(item);
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

        public async Task<List<AppointmentDto>> GetAppointmentListByDoctorIdAsync(long doctorId)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            //var appointments = await item.Where(d=> d.DoctorProfileId == doctorId && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            var appointments = item.Where(d => d.DoctorProfileId == doctorId).ToList();// && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>> GetAppointmentListByPatientIdAsync(long patientId)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            //var appointments = await item.Where(d=> d.DoctorProfileId == doctorId && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            var appointments = item.Where(d => d.AppointmentCreatorId == patientId).ToList();// && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<int> GetAppCountByScheduleIdSessionIdAsync(long? scheduleId, long? sessionId)
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
            else if (noOfPatients > appCounts)
            {
                resultNp = (noOfPatients - appCounts);
            }
            else
            {
                resultNp = noOfPatients;
            }

            return resultNp;//noOfPatients == appCounts? 0: (int)resultNp;
        }

        public async Task<List<AppointmentDto>> GetPatientListByDoctorIdAsync(long doctorId)
        {
            var restultPatientList = new List<AppointmentDto>();
            try
            {
                var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
                //var appointments = await item.Where(d=> d.DoctorProfileId == doctorId && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
                var appointments = item.Where(d => d.DoctorProfileId == doctorId);// && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
                var patientIds = (from app in appointments
                                  select app.PatientProfileId).Distinct();
                foreach (var appointment in patientIds)
                {
                    var patient = await _patientProfileRepository.GetAsync(p => p.Id == appointment);
                    restultPatientList.Add(new AppointmentDto()
                    {
                        DoctorProfileId=doctorId,
                        PatientProfileId = patient.Id,
                        PatientCode = patient.PatientCode,
                        PatientName = patient.PatientName,
                        PatientMobileNo = patient.PatientMobileNo,
                        PatientEmail = patient.PatientEmail,
                        PatientLocation = patient.City
                    });
                }
                return restultPatientList;//ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
            }
            catch (Exception ex) {
                return restultPatientList;
            }
            
        }

        public string testBuildTokenWithUserAccount(string _appId, string _appCertificate, string _channelName, string _account)
        {
            uint privilegeExpiredTs = _expireTimeInSeconds + (uint)Utils.getTimestamp();
            string token = RtcTokenBuilder.buildTokenWithUserAccount(_appId, _appCertificate, _channelName, _account, RtcTokenBuilder.Role.RolePublisher, privilegeExpiredTs);
            return token;
            //Output.WriteLine(">> token");
            //Output.WriteLine(token);
        }

        public string testBuildTokenWithUID(string _appId, string _appCertificate, string _channelName, uint _uid)
        {
            uint privilegeExpiredTs = _expireTimeInSeconds + (uint)Utils.getTimestamp();
            string token = RtcTokenBuilder.buildTokenWithUID(_appId, _appCertificate, _channelName, _uid, RtcTokenBuilder.Role.RolePublisher, privilegeExpiredTs);
            return token;
            //Output.WriteLine(">> token");
            //Output.WriteLine(token);
        }

    }
}
