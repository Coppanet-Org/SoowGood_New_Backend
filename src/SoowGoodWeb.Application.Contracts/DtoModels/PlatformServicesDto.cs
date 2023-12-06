using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class PlatformServicesDto : FullAuditedEntityDto<long>
    {
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
    }
}
