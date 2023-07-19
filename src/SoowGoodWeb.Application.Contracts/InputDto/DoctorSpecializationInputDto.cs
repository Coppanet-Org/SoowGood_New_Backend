using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.InputDto
{
    public class DoctorSpecializationInputDto : FullAuditedEntityDto<long>
    {
        public long? DoctorId { get; set; }
        //public DoctorProfileInputDto DoctorProfile { get; set; }
        public long? SpecialityId { get; set; }
        //public SpecialityInputDto Speciality { get; set; }
        public long? SpecializationId { get; set; }
        //public SpecializationInputDto Specialization { get; set; }

    }
}
