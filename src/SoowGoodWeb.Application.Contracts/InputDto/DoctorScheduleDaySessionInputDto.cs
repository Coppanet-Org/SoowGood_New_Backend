using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class DoctorScheduleDaySessionInputDto : FullAuditedEntityDto<long>
    {
        public long? DoctorScheduleId { get; set; }
        //public DoctorSchedule? DoctorSchedule { get; set; }
        public string? ScheduleDayofWeek { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NoOfPatients { get; set; }
        public bool? IsActive { get; set; }
    }
}
