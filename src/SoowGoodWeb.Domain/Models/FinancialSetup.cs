﻿using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class FinancialSetup : FullAuditedEntity<long>
    {

        public long? PlatformFacilityId { get; set; }
        public PlatformFacility? PlatformFacility { get; set; }
        public string? AmountIn { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        public string? ExternalAmountIn { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ExternalAmount { get; set; }
        public bool? IsActivie { get; set; }    
    }
}
