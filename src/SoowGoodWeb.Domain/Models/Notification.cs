using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class Notification : FullAuditedEntity<long>
    {
        public string? Message { get; set; }
        public string? TransactionType { get; set; }
    }
}
