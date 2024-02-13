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
using SoowGoodWeb.InputDto;
using SoowGoodWeb.SslCommerz;
using AgoraIO.Media;
using System.Globalization;

namespace SoowGoodWeb.Services
{
    public class AppointmentService : SoowGoodWebAppService, IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<DoctorChamber> _doctorChamberRepository;
        private readonly IRepository<DoctorScheduleDaySession> _doctorScheduleSessionRepository;
        private readonly IRepository<PatientProfile> _patientProfileRepository;
        private readonly IRepository<AgentProfile> _agentProfileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly SslCommerzGatewayManager _sslCommerzGatewayManager;


        private readonly uint _expireTimeInSeconds = 3600;
        public AppointmentService(IRepository<Appointment> appointmentRepository,
            IRepository<DoctorChamber> doctorChamberRepository,
            IRepository<DoctorScheduleDaySession> doctorScheduleSessionRepository,
            IRepository<PatientProfile> patientProfileRepository,
            SslCommerzGatewayManager sslCommerzGatewayManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _appointmentRepository = appointmentRepository;
            //_doctorScheduleRepository = doctorScheduleRepository;
            _doctorChamberRepository = doctorChamberRepository;
            _doctorScheduleSessionRepository = doctorScheduleSessionRepository;
            _patientProfileRepository = patientProfileRepository;
            _sslCommerzGatewayManager = sslCommerzGatewayManager;

            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentInputDto input)
        {
            var response = new AppointmentDto();
            try
            {
                string consultancyType;
                long lastSerial;
                var chamberName = "";
                if (input.DoctorChamberId > 0)

                {
                    var appChamber = await _doctorChamberRepository.FirstOrDefaultAsync(c => c.Id == input.DoctorChamberId);
                    chamberName = appChamber.ChamberName;
                }

                var list = new List<string>();
                if (input is { DoctorScheduleId: > 0, DoctorScheduleDaySessionId: > 0 })
                {
                    var mainSession = await _doctorScheduleSessionRepository.GetAsync(s => s.Id == input.DoctorScheduleDaySessionId && s.DoctorScheduleId == input.DoctorScheduleId);
                    var stTime = Convert.ToDateTime(mainSession.StartTime);
                    var enTime = Convert.ToDateTime(mainSession.EndTime);
                    var totalHr = (enTime - stTime).TotalHours;

                    var hrMnt = totalHr * 60;
                    var slotPerPatient = hrMnt / mainSession.NoOfPatients;
                    string[]? slots = null;

                    for (var appointment = stTime; appointment < enTime; appointment = appointment.AddMinutes((double)slotPerPatient!))
                    {
                        list.Add(appointment.ToString("HH:mm"));
                        slots = list.ToArray();
                    }

                    lastSerial = await GetAppCountByScheduleIdSessionIdAsync(input.DoctorScheduleId, input.DoctorScheduleDaySessionId);


                    for (var i = lastSerial; i < mainSession.NoOfPatients;)
                    {
                        input.AppointmentTime = slots != null ? slots[i] : "";
                        break;
                    }
                    consultancyType = (input.ConsultancyType > 0 ? (ConsultancyType)input.ConsultancyType : 0).ToString();
                    input.AppointmentSerial = (lastSerial + 1).ToString();
                    input.AppointmentCode = input.DoctorCode + input.AppointmentDate?.ToString("yyyyMMdd") + consultancyType + "SL00" + input.AppointmentSerial;
                }
                else
                {
                    input.ConsultancyType = ConsultancyType.OnlineRT;
                    input.AppointmentDate = DateTime.Today;
                    input.AppointmentTime = DateTime.Now.ToString("HH:mm");
                    input.AppointmentType = AppointmentType.New;
                    lastSerial = await GetAppCountByRealTimeConsultancyAsync(input.AppointmentDate);
                    consultancyType = ConsultancyType.OnlineRT.ToString();
                    input.AppointmentSerial = (lastSerial + 1).ToString();
                    input.AppointmentCode = input.DoctorCode + input.AppointmentDate?.ToString("yyyyMMdd") + consultancyType + "SL00" + input.AppointmentSerial;
                }
                var newEntity = ObjectMapper.Map<AppointmentInputDto, Appointment>(input);

                var doctorChamber = await _appointmentRepository.InsertAsync(newEntity);

                response = ObjectMapper.Map<Appointment, AppointmentDto>(doctorChamber);

                response.AppointmentTypeName = response.AppointmentType.ToString();
                response.ConsultancyTypeName = response.ConsultancyType.ToString();
                response.DoctorChamberName = !string.IsNullOrEmpty(chamberName) ? chamberName.ToString() : "SoowGood Online";

            }
            catch (Exception ex)
            {
                return response;
            }

            return response;//ObjectMapper.Map<Appointment, AppointmentDto>(doctorChamber);
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
            var appointments = item.Where(d => d.DoctorProfileId == doctorId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>> GetAppointmentListWithSearchFilterAsync(long doctorId, string? name, ConsultancyType? consultancy, string? fromDate, string? toDate, AppointmentStatus? aptStatus, int? skipValue, int? currentLimit)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            var appointments = item.Where(d => d.DoctorProfileId == doctorId).ToList();// && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();

            if (!string.IsNullOrEmpty(name))
            {
                appointments = appointments.Where(p => p.PatientName != null && p.PatientName.Contains(name)).ToList();
            }
            if (consultancy > 0)
            {
                appointments = appointments.Where(p => p.ConsultancyType == consultancy).ToList();
            }
            if (aptStatus > 0)
            {
                appointments = appointments.Where(p => p.AppointmentStatus == aptStatus).ToList();
            }
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                appointments = appointments.Where(p =>
                p.AppointmentDate != null
                && p.AppointmentDate.Value.Date >= DateTime.ParseExact(fromDate, "dd/MM/yyyy", provider, DateTimeStyles.None)
                && p.AppointmentDate.Value.Date <= DateTime.ParseExact(toDate, "dd/MM/yyyy", provider, DateTimeStyles.None)).ToList();
            }
            if (!string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate))
            {
                appointments = appointments.Where(p =>
                p.AppointmentDate != null
                && p.AppointmentDate.Value.Date >= DateTime.ParseExact(fromDate, "dd/MM/yyyy", provider, DateTimeStyles.None)
                && p.AppointmentDate.Value.Date <= DateTime.ParseExact(fromDate, "dd/MM/yyyy", provider, DateTimeStyles.None)).ToList();
            }
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>?> GetAppointmentListForDoctorWithSearchFilterAsync(long doctorId, DataFilterModel? dataFilter, FilterModel filterModel)
        {
            var provider = CultureInfo.InvariantCulture;
            try
            {
                if (dataFilter?.toDate is null or "Invalid Date")
                {
                    if (dataFilter != null)
                    {
                        dataFilter.toDate = dataFilter.fromDate;
                    }
                }
                var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
                var appointments = item.Where(d => d.DoctorProfileId == doctorId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();// && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();

                if (!string.IsNullOrEmpty(dataFilter?.name))
                {
                    appointments = appointments.Where(p => p.PatientName != null && p.PatientName.ToLower().Contains(dataFilter.name.ToLower().Trim())).ToList();
                }

                if (dataFilter?.consultancyType > 0 || dataFilter?.appointmentStatus > 0
                                                    || (!string.IsNullOrEmpty(dataFilter?.fromDate) && !string.IsNullOrEmpty(dataFilter.toDate)))
                {
                    appointments = appointments.Where(p => dataFilter.toDate != null && p.AppointmentDate != null && dataFilter.fromDate != null && (p.ConsultancyType == dataFilter.consultancyType
                        || p.AppointmentStatus == dataFilter.appointmentStatus
                        || (p.AppointmentDate.Value.Date >= DateTime.ParseExact(dataFilter.fromDate, "MM/dd/yyyy", provider, DateTimeStyles.None)
                            && p.AppointmentDate.Value.Date <= DateTime.ParseExact(dataFilter.toDate, "MM/dd/yyyy", provider, DateTimeStyles.None)))).ToList();
                }

                appointments = appointments.Skip(filterModel.Offset)
                                   .Take(filterModel.Limit).ToList();

                return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<int> GetAppointmentCountForDoctorWithSearchFilterAsync(long doctorId, DataFilterModel? dataFilter)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                if (dataFilter?.toDate == "Invalid Date")
                {
                    dataFilter.toDate = dataFilter.fromDate;
                }
                var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
                var appointments = item.Where(d => d.DoctorProfileId == doctorId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();// && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();

                if (!string.IsNullOrEmpty(dataFilter.name))
                {
                    appointments = appointments.Where(p => p.PatientName.Contains(dataFilter.name)).ToList();
                }

                if (dataFilter.consultancyType > 0 || dataFilter.appointmentStatus > 0
                    || (!string.IsNullOrEmpty(dataFilter.fromDate) && !string.IsNullOrEmpty(dataFilter.toDate)))
                {
                    appointments = appointments.Where(p => p.ConsultancyType == dataFilter.consultancyType
                                                            || p.AppointmentStatus == dataFilter.appointmentStatus
                                                            || (p.AppointmentDate.Value.Date >= DateTime.ParseExact(dataFilter.fromDate, "MM/dd/yyyy", provider, DateTimeStyles.None)
                                                            && p.AppointmentDate.Value.Date <= DateTime.ParseExact(dataFilter.toDate, "MM/dd/yyyy", provider, DateTimeStyles.None))).ToList();
                }


                return appointments.Count();
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<List<AppointmentDto>> GetAppointmentListByPatientIdAsync(long patientId, string role)
        {
            var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
            var appointments = item.Where(d => d.AppointmentCreatorId == patientId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed) && d.AppointmentCreatorRole == role).ToList();
            return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>> GetAppointmentListForPatientWithSearchFilterAsync(long patientId, string role, DataFilterModel? dataFilter, FilterModel filterModel)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                if (dataFilter?.toDate == "Invalid Date")
                {
                    dataFilter.toDate = dataFilter.fromDate; ;
                }
                var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
                var appointments = item.Where(d => d.AppointmentCreatorId == patientId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed) && d.AppointmentCreatorRole == role).ToList();// && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();

                if (!string.IsNullOrEmpty(dataFilter.name))
                {
                    //appointments = appointments.Where(p => p.DoctorName.Contains(dataFilter.name)).ToList();
                    appointments = appointments.Where(p => p.DoctorName.ToLower().Contains(dataFilter.name.ToLower().Trim())).ToList();
                }
                if (dataFilter.consultancyType > 0 || dataFilter.appointmentStatus > 0
                    || (!string.IsNullOrEmpty(dataFilter.fromDate) && !string.IsNullOrEmpty(dataFilter.toDate)))
                {
                    appointments = appointments.Where(p => p.ConsultancyType == dataFilter.consultancyType
                                                            || p.AppointmentStatus == dataFilter.appointmentStatus
                                                            || (p.AppointmentDate.Value.Date >= DateTime.ParseExact(dataFilter.fromDate, "MM/dd/yyyy", provider, DateTimeStyles.None)
                                                            && p.AppointmentDate.Value.Date <= DateTime.ParseExact(dataFilter.toDate, "MM/dd/yyyy", provider, DateTimeStyles.None))).ToList();
                }

                appointments = appointments.Skip(filterModel.Offset)
                                   .Take(filterModel.Limit).ToList();

                return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<int> GetAppointmentCountForPatientWithSearchFilterAsync(long patientId, string role, DataFilterModel? dataFilter)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                if (dataFilter?.toDate == "Invalid Date")
                {
                    dataFilter.toDate = dataFilter.fromDate; ;
                }
                var item = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule);
                var appointments = item.Where(d => d.AppointmentCreatorId == patientId && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed) && d.AppointmentCreatorRole == role).ToList();// && (d.AppointmentStatus == AppointmentStatus.Confirmed || d.AppointmentStatus == AppointmentStatus.Completed)).ToList();

                if (!string.IsNullOrEmpty(dataFilter.name))
                {
                    appointments = appointments.Where(p => p.DoctorName.Contains(dataFilter.name)).ToList();
                }
                if (dataFilter.consultancyType > 0 || dataFilter.appointmentStatus > 0
                    || (!string.IsNullOrEmpty(dataFilter.fromDate) && !string.IsNullOrEmpty(dataFilter.toDate)))
                {
                    appointments = appointments.Where(p => p.ConsultancyType == dataFilter.consultancyType
                                                            || p.AppointmentStatus == dataFilter.appointmentStatus
                                                            || (p.AppointmentDate.Value.Date >= DateTime.ParseExact(dataFilter.fromDate, "MM/dd/yyyy", provider, DateTimeStyles.None)
                                                            && p.AppointmentDate.Value.Date <= DateTime.ParseExact(dataFilter.toDate, "MM/dd/yyyy", provider, DateTimeStyles.None))).ToList();
                }

                return appointments.Count;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<List<AppointmentDto>?> GetListAppointmentListByAdminAsync()
        {
            List<AppointmentDto>? result = null;
            DoctorScheduleDaySession? weekDayName = null;
            var allAppoinment = await _appointmentRepository.WithDetailsAsync(s => s.DoctorSchedule, c => c.DoctorSchedule.DoctorChamber);
            //var  = await _appointmentRepository.GetListAsync();
            if (!allAppoinment.Any())
            {
                return result;
            }

            result = new List<AppointmentDto>();
            try
            {

                foreach (var item in allAppoinment)
                {
                    var patientDetails = await _patientProfileRepository.GetAsync(p => p.Id == item.PatientProfileId);
                    var agentDetails = await _agentProfileRepository.WithDetailsAsync(a => a.AgentMaster, s => s.AgentSupervisor);
                    //if(item.AppointmentCreatorRole=="agent")
                    var agent = item.AppointmentCreatorRole == "" ? agentDetails.Where(a => a.Id == item.AppointmentCreatorId).FirstOrDefault() : null;
                    if (item.AppointmentCreatorId > 0)
                    {

                    }
                    if (item.DoctorScheduleDaySessionId > 0)
                    {
                        weekDayName = await _doctorScheduleSessionRepository.GetAsync(p => p.Id == item.DoctorScheduleDaySessionId);
                    }
                    result.Add(new AppointmentDto()
                    {
                        Id = item.Id,
                        PatientName = item.PatientName,
                        AppointmentDate = Convert.ToDateTime(item.AppointmentDate).Date,
                        AppointmentTime = item.AppointmentTime,
                        AppointmentSerial = item.AppointmentSerial,
                        AppointmentType = item.AppointmentType,
                        AppointmentTypeName = item.AppointmentType > 0 ? ((AppointmentType)item.AppointmentType).ToString() : "n/a",
                        DoctorName = item.DoctorName,
                        DoctorScheduleId = item.DoctorScheduleId,
                        DoctorScheduleName = item.DoctorScheduleId > 0 ? item.DoctorSchedule?.ScheduleName : "n/a",
                        AppointmentCode = item.AppointmentCode,
                        AppointmentStatus = item.AppointmentStatus,
                        DoctorCode = item.DoctorCode,
                        PatientCode = item.PatientCode,
                        PatientMobileNo = patientDetails.PatientMobileNo,
                        PatientEmail = patientDetails.PatientEmail,
                        AppointmentStatusName = item.AppointmentStatus > 0 ? ((AppointmentStatus)item.AppointmentStatus).ToString() : "n/a",
                        AppointmentPaymentStatus = item.AppointmentPaymentStatus,
                        AppointmentPaymentStatusName = item.AppointmentPaymentStatus > 0 ? ((AppointmentPaymentStatus)item.AppointmentPaymentStatus).ToString() : "n/a",
                        ConsultancyType = item.ConsultancyType,
                        ConsultancyTypeName = item.ConsultancyType > 0 ? ((ConsultancyType)item.ConsultancyType).ToString() : "n/a",
                        DoctorChamberId = item.DoctorChamberId,
                        DoctorChamberName = item.DoctorChamberId > 0 ? item.DoctorSchedule?.DoctorChamber?.ChamberName : "n/a",
                        DoctorFee = item.DoctorFee,
                        PatientLocation = patientDetails?.City?.ToString(),
                        DoctorScheduleDaySessionId = item.DoctorScheduleDaySessionId,
                        ScheduleDayofWeek = weekDayName?.ScheduleDayofWeek?.ToString(),
                        CancelledByRole = item.CancelledByRole,
                        PaymentTransactionId = item.PaymentTransactionId,
                        AppointmentCreatorRole = item.AppointmentCreatorRole,
                        BoothName = item.AppointmentCreatorRole == "agent" ? agent?.Address : "N/A",
                        AgentMasterName = item.AppointmentCreatorRole == "agent" ?agent?.AgentMaster?.AgentMasterOrgName : "N/A",
                        AgentSupervisorName = item.AppointmentCreatorRole == "agent" ?agent?.AgentSupervisor?.AgentSupervisorOrgName : "N/A",
                    });
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            result = result.OrderByDescending(a => a.AppointmentDate).ToList();
            var list = result.OrderBy(item => item.AppointmentSerial)
                .GroupBy(item => item.AppointmentDate)
                .OrderBy(g => g.Key).Select(g => new { g }).ToList();


            return result;
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
                        DoctorProfileId = doctorId,
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
            catch (Exception ex)
            {
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

        public string testBuildTokenWithUID(RtcTokenBuilerDto input)
        {
            uint privilegeExpiredTs = _expireTimeInSeconds + (uint)Utils.getTimestamp();
            string token = RtcTokenBuilder.buildTokenWithUID(input.Appid, input.AppCertificate, input.ChanelName, input.Uid, RtcTokenBuilder.Role.RolePublisher, privilegeExpiredTs);
            return token;
            //Output.WriteLine(">> token");
            //Output.WriteLine(token);
        }

        public string testAcToken(RtcTokenBuilerDto input)
        {
            uint privilegeExpiredTs = _expireTimeInSeconds + (uint)Utils.getTimestamp();
            AccessToken accessToken = new AccessToken(input.Appid, input.AppCertificate, input.ChanelName, input.Uid.ToString(), privilegeExpiredTs, 1);
            accessToken.addPrivilege(Privileges.kJoinChannel, privilegeExpiredTs);
            accessToken.addPrivilege(Privileges.kPublishAudioStream, privilegeExpiredTs);
            accessToken.addPrivilege(Privileges.kPublishVideoStream, privilegeExpiredTs);
            accessToken.addPrivilege(Privileges.kPublishDataStream, privilegeExpiredTs);

            string token = accessToken.build();
            return token;
            //Output.WriteLine(">> token");
            //Output.WriteLine(token);
        }

        public async Task<ResponseDto> UpdateCallConsultationAppointmentAsync(string appCode)
        {
            var response = new ResponseDto();
            try
            {
                var itemAppointment = await _appointmentRepository.GetAsync(a => a.AppointmentCode == appCode);//.FindAsync(input.Id);
                itemAppointment.AppointmentStatus = AppointmentStatus.Completed;
                itemAppointment.IsCousltationComplete = true;



                var item = await _appointmentRepository.UpdateAsync(itemAppointment);
                //await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<Appointment, AppointmentDto>(item);
                if (result != null)
                {
                    response.Id = result.Id;
                    response.Value = "";
                    response.Success = true;
                    response.Message = "Consultation complete";
                }
                return response;//ObjectMapper.Map<Appointment, AppointmentDto>(item);
            }
            catch (Exception ex)
            {
                return null;
            }
            return response;
        }

        public async Task UpdateAppointmentPaymentStatusAsync(string appCode, string trnId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAsync(a => a.AppointmentCode == appCode);
                if (appointment != null && appointment.AppointmentStatus != AppointmentStatus.Confirmed) //&& app.AppointmentStatus != AppointmentStatus.Confirmed)
                {
                    appointment.AppointmentStatus = AppointmentStatus.Confirmed;
                    appointment.PaymentTransactionId = trnId;
                    appointment.AppointmentPaymentStatus = AppointmentPaymentStatus.Paid;
                    //app.FeePaid = string.IsNullOrWhiteSpace(paid_amount) ? 0 : double.Parse(paid_amount);

                    await _appointmentRepository.UpdateAsync(appointment);

                    //await SendNotification(application_code, applicant.Applicant.Mobile);
                }
            }
            catch (Exception ex) { }

        }

        public async Task<int> GetAppCountByRealTimeConsultancyAsync(DateTime? aptDate)
        {
            var appointments = await _appointmentRepository.GetListAsync(a => a.AppointmentDate == aptDate && a.ConsultancyType == ConsultancyType.OnlineRT);
            var appCount = appointments.Count();
            return appCount;
        }

        public async Task<ResponseDto> CancellAppointmentAsync(long appId, long cancelByid, string cancelByRole)
        {
            var response = new ResponseDto();
            try
            {
                var itemAppointment = await _appointmentRepository.GetAsync(a => a.Id == appId);//.FindAsync(input.Id);
                itemAppointment.AppointmentStatus = AppointmentStatus.Cancelled;
                itemAppointment.CancelledByEntityId = cancelByid;
                itemAppointment.CancelledByRole = cancelByRole;



                var item = await _appointmentRepository.UpdateAsync(itemAppointment);
                //await _unitOfWorkManager.Current.SaveChangesAsync();
                var result = ObjectMapper.Map<Appointment, AppointmentDto>(item);
                if (result != null)
                {
                    response.Id = result.Id;
                    response.Value = "";
                    response.Success = true;
                    response.Message = "Consultation complete";
                }
                return response;//ObjectMapper.Map<Appointment, AppointmentDto>(item);
            }
            catch (Exception ex)
            {
                return null;
            }
            return response;
        }

        public async Task<List<AppointmentDto>> GetSearchedPatientListByDoctorIdAsync(long doctorId, string name)
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
                        DoctorProfileId = doctorId,
                        PatientProfileId = patient.Id,
                        PatientCode = patient.PatientCode,
                        PatientName = patient.PatientName,
                        PatientMobileNo = patient.PatientMobileNo,
                        PatientEmail = patient.PatientEmail,
                        PatientLocation = patient.City
                    });
                }

                if (!string.IsNullOrEmpty(name))
                {
                    restultPatientList = restultPatientList.Where(p => p.PatientName.Contains(name)).ToList();
                }

                return restultPatientList;//ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                return restultPatientList;
            }

        }
    }
}
