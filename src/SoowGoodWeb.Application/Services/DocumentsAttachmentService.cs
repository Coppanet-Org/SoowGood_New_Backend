using SoowGoodWeb.Enums;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace SoowGoodWeb.Services
{
    public class DocumentsAttachmentService : CrudAppService<DocumentsAttachment, DocumentsAttachmentDto, long>
    {
        private readonly IRepository<DocumentsAttachment, long> repository;
        public DocumentsAttachmentService(IRepository<DocumentsAttachment, long> repository) : base(repository)
        {
            this.repository = repository;
        }

        public async Task<DocumentsAttachmentDto?> GetDocumentInfo(string entityType, long? entityId, string attachmentType)
        {
            var attachment = await repository.GetAsync(x => x.EntityType == (EntityType)Enum.Parse(typeof(EntityType), entityType)
                                                                && x.EntityId == entityId
                                                                && x.AttachmentType == (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType)
                                                                && x.IsDeleted == false);
            if (attachment != null)
            {
                return ObjectMapper.Map<DocumentsAttachment, DocumentsAttachmentDto>(attachment);
            }

            return null;
        }

        public async Task<List<DocumentsAttachmentDto>?> GetAttachmentInfo(string entityType, long? entityId, string attachmentType)
        {
            var attachment = await repository.GetListAsync(x => x.EntityType == (EntityType)Enum.Parse(typeof(EntityType), entityType)
                                                                && x.EntityId == entityId
                                                                && x.AttachmentType == (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType)
                                                                && x.IsDeleted == false);            
            if (attachment != null)
            {
                return ObjectMapper.Map<List<DocumentsAttachment>, List<DocumentsAttachmentDto>>(attachment);
            }

            return null;
        }
    }
}
