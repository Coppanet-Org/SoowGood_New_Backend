using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.InputDto
{
    public class FinancialSetupInputDto : FullAuditedEntityDto<long>
    {
        public long? PlatformFacilityId { get; set; }
        public string? AmountIn { get; set; }
        public decimal? Amount { get; set; }
        public string? ExternalAmountIn { get; set; }
        public decimal? ExternalAmount { get; set; }
        public decimal? ProviderAmount { get; set; }
        public bool? IsActivie { get; set; }
    }
}
