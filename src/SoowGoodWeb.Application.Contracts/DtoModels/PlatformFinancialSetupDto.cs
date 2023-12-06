using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class PlatformFinancialSetupDto : FullAuditedEntityDto<long>
    {
        public long? PlatformServicesId { get; set; }
        public string? PlatformServicesName { get; set; }
        public string? AmountIn { get; set; } //Percent Or Flat Amount
        public decimal? FeeAmount { get; set; }
        public string? ExternalAmountIn { get; set; } //Percent Or Flat Amount
        public decimal? ExternalFeeAmount { get; set; }
        public bool? IsActive { get; set; }
    }
}
