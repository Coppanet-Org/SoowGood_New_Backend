using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.InputDto
{
    public class NotificationInputDto : FullAuditedEntityDto<long>
    {
        public string? Message { get; set; }
        public string? TransactionType { get; set; }
    }
}
