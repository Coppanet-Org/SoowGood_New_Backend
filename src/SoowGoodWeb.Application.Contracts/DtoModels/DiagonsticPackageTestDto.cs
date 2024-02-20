using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class DiagonsticPackageTestDto : FullAuditedEntityDto<long>
    {
        public long? ServiceProviderId { get; set; }
        public string? ServiceProviderName { get; set; }
        public long? PathologyCategoryId { get; set; }
        public string? PathologyCategoryName { get; set; }
        public long? PathologyTestId { get; set; }
        public string? PathologyTestName { get; set; }
        public decimal? ProviderRate { get; set; }
    }
}
