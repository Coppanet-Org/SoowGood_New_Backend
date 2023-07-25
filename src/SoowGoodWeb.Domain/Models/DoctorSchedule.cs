using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class DoctorSchedule : FullAuditedEntity<long>
    {
        public long? DoctorId { get; set; }
        public DoctorProfile? DoctorProfile { get; set; }             
        public ScheduleType? ScheduleType { get; set; }
        public ConsultancyType? ConsultancyType { get; set; }
        public long? ChamberId { get; set; }
        public DoctorChamber? DoctorChamber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? NoOfPatients { get; set; }
        public bool? IsActive { get; set; }
        public List<DoctorScheduledDayOff>? DoctorScheduledDayOffs { get; set; }
        public List<DoctorFeesSetup>? DoctorFeesSetup { get; set; }
    }
}
