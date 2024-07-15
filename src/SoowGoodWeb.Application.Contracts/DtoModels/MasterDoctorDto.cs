using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class MasterDoctorDto : FullAuditedEntityDto<long>
    {
        public long? DoctorProfileId { get; set; }
        //public DoctorProfile? DoctorProfile { get; set; }
        public long? AgentMasterId { get; set; }
        //public AgentMaster? AgentMaster { get; set; }

        public string? DoctorName { get; set; }
        //public DoctorTitle? DoctorTitle { get; set; }
        //public string? DoctorTitleName { get; set; }

        //public List<DoctorDegreeDto>? DoctorDegrees { get; set; }
        //public List<DoctorSpecializationDto>? DoctorSpecialization { get; set; }

    }
}
