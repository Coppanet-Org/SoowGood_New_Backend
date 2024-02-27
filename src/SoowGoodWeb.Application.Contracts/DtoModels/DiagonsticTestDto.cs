using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class DiagonsticTestDto : FullAuditedEntityDto<long>
    {
        public long? ServiceProviderId { get; set; }
        public string? ServiceProviderName { get; set; }
        public string? PackageName { get; set; }
        public string? PackageDescription { get; set; }
        public decimal? ProviderRate { get; set; }
    }
}
