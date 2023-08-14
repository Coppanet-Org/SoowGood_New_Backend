using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class DoctorScheduleDto : FullAuditedEntityDto<long>
    {
        public long? DoctorProfileId { get; set; }
        //public DoctorProfile? DoctorProfile { get; set; }
        public string? DoctorName { get; set; }
        public ScheduleType? ScheduleType { get; set; }
        public string? ScheduleTypeName { get; set; }
        public ConsultancyType? ConsultancyType { get; set; }
        public string? ConsultancyTypeName { get; set; }
        public long? DoctorChamberId { get; set; }
        //public DoctorChamber? DoctorChamber { get; set; }
        public string? Chamber { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? OffDayFrom { get; set; }
        public DateTime? OffDayTo { get; set; }
        public List<DoctorScheduleDaySessionDto>? DoctorScheduleDaySession { get; set; }
        public List<DoctorFeesSetupDto>? DoctorFeesSetup { get; set; }
    }
}
