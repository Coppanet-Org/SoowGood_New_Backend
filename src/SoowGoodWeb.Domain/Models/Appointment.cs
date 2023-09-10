﻿using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class Appointment : FullAuditedEntity<long>
    {
        public long? DoctorProfileId { get; set; }
        public DoctorProfile? DoctorProfile { get; set; }             
        public string? DoctorName { get; set; }
        public long? PatientProfileId { get; set; }
        public PatientProfile? PatientProfile { get; set; }             
        public string? PatientName { get; set; }
        public long? DoctorScheduleId { get; set; }
        public DoctorSchedule? DoctorSchedule { get; set; }
        public string? ScheduleName { get; set; }
        public ConsultancyType? ConsultancyType { get; set; }
        public long? DoctorChamberId { get; set; }
        public long? DoctorScheduleDaySessionId { get; set; }
        public DoctorScheduleDaySession? DoctorScheduleDaySession { get; set; }
        public string? ScheduleDay { get; set; }
        public AppointmentType? AppointmentType { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentSerial { get; set; }
        public long? DoctorFeesSetupId { get; set; }
        public DoctorFeesSetup? DoctorFeesSetup { get; set; }
        public decimal? DoctorFee { get; set; }
        public decimal? AgentFee { get; set; }
        public decimal? PlatformFee { get; set; }
        public decimal? TotalFee { get; set; }
        public AppointmentStatus? AppointmentStatus { get; set; }
        public AppointmentPaymentStatus? AppointmentPaymentStatus { get; set; }
        public long? CancelledByEntityId { get; set; }
        public string? CancelledByRole { get; set; }
    }
}