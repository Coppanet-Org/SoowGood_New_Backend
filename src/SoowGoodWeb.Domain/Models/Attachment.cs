using SoowGoodWeb.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace SoowGoodWeb.Models
{
    public class Attachment : FullAuditedEntity<int>
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string Path { get; set; }
        public EntityType EntityType { get; set; }
        public int? EntityId { get; set; }
        public AttachmentType AttachmentType { get; set; }

    }

}
