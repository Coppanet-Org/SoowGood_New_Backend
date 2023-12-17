using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class FinancialSetupDto : FullAuditedEntityDto<long>
    {
        public long? PlatformFacilityId { get; set; }
        public string? FacilityName { get; set; }
        public string? AmountIn { get; set; }
        public decimal? Amount { get; set; }
        public string? ExternalAmountIn { get; set; }
        public decimal? ExternalAmount { get; set; }
        public decimal? ProviderAmount { get; set; }
        public bool? IsActivie { get; set; }
    }
}
