using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class NotificationDto : FullAuditedEntityDto<long>
    {
        public string? Message { get; set; }
        public string? TransactionType { get; set; }
    }
}
