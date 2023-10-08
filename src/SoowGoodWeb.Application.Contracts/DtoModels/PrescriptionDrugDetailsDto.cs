﻿using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class PrescriptionDrugDetailsDto : FullAuditedEntityDto<long>
    {
        public long? PrescriptionMasterId { get; set; }
        public string? PrescriptionRefferenceCode { get; set; }
        public long? DrugRxId { get; set; }
        public string? DrugName { get; set; }
        public string? Dose { get; set; }
        public string? Duration { get; set; }
        public string? Instruction { get; set; }
    }
}
