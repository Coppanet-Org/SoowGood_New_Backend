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

namespace SoowGoodWeb.Services
{
    public class AppointmentService : SoowGoodWebAppService, IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        //private readonly IRepository<DoctorChamber> _doctorChamberRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly SslCommerzGatewayManager _sslCommerzGatewayManager;

        public AppointmentService(IRepository<Appointment> appointmentRepository,
            //IRepository<DoctorChamber> doctorChamberRepository,
            IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository,
            SslCommerzGatewayManager sslCommerzGatewayManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _appointmentRepository = appointmentRepository;
            //_doctorScheduleRepository = doctorScheduleRepository;
            //_doctorChamberRepository = doctorChamberRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
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
                input.AppointmenCode = input.DoctorCode + "-" + input.PatientCode + "-" + input.AppointmentSerial;
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

        public async Task<List<AppointmentDto>> GeAppointmentListByDoctorIdAsync(long doctorId)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            //var appointments = await item.Where(d=> d.DoctorProfileId == doctorId && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            var appointments = item.Where(d => d.DoctorProfileId == doctorId).ToList();// && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>> GeAppointmentListByPatientIdAsync(long patientId)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            //var appointments = await item.Where(d=> d.DoctorProfileId == doctorId && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
            var appointments = item.Where(d => d.PatientProfileId == patientId).ToList();// && d.AppointmentStatus == AppointmentStatus.Confirmed).ToList();
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

    }
}
