using SoowGoodWeb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class DiagonsticPackage : FullAuditedEntity<long>
    {
        public long? ServiceProviderId { get; set; }
        public ServiceProvider? ServiceProvider  { get; set; }
        public long? PathologyCategoryId { get; set; }
        public PathologyCategory? PathologyCategory { get; set; }
        public long? PathologyTestId { get; set; }
        public PathologyTest? PathologyTest { get; set; }
        public string? PackageName { get; set; }
        public string? PackageDescription { get; set; }
    }
}
