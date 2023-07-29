using Azure.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SoowGoodWeb.Enums;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net.Http.Headers;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Models;
using Volo.Abp.Uow;
using System.Threading.Tasks;
using SoowGoodWeb.DtoModels;
using System.Net.Mail;
using Attachment = SoowGoodWeb.Models.Attachment;
using Volo.Abp.ObjectMapping;
using SoowGoodWeb.Interfaces;

namespace SoowGoodWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : AbpController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<DocumentsAttachment> _attachmentRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public CommonController(
            IWebHostEnvironment webHostEnvironment,
            IRepository<DocumentsAttachment> attachmentRepository,
            IUnitOfWorkManager unitOfWorkManager
           )
        {
            _webHostEnvironment = webHostEnvironment;
            _attachmentRepository = attachmentRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [HttpPost, ActionName("Documents")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FileUploadDocuments()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    var idStr = Request.Form["entityId"].ToString();
                    int entityId = string.IsNullOrEmpty(idStr) ? 0 : Convert.ToInt32(idStr);
                    var entityType = Request.Form["entityType"].ToString();

                    var attachmentType = Request.Form["attachmentType"].ToString();

                    var directoryName = Request.Form["directoryName"][0];
                    var folderName = Path.Combine("wwwroot", "uploads", !string.IsNullOrEmpty(directoryName) ? directoryName : "Misc");
                    int insertCount = 0;
                    foreach (var file in files)
                    {
                        if (!Directory.Exists(folderName))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(folderName);
                        }

                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        // save attachment
                        var fileExist = GetDocumentAsync(entityType, entityId, attachmentType);
                        if (fileExist == null)
                        {
                            var attachement = new DocumentsAttachment();
                            attachement.FileName = idStr + "_" + fileName;
                            attachement.OriginalFileName = fileName;
                            attachement.Path = dbPath;
                            attachement.EntityType = (EntityType)Enum.Parse(typeof(EntityType), entityType);
                            attachement.EntityId = entityId;
                            attachement.AttachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType);
                            var attachmentResult = await _attachmentRepository.InsertAsync(attachement, autoSave: true);
                            //await _unitOfWorkManager.Current.SaveChangesAsync();
                            if (attachmentResult != null)//  == 0)
                            {
                                insertCount += 1;
                            }

                        }
                        else
                        {
                            var attachementDto = new DocumentsAttachmentDto();
                            attachementDto.Id = fileExist.Result.Id;
                            attachementDto.FileName = idStr + "_" + fileName;
                            attachementDto.OriginalFileName = fileName;
                            attachementDto.Path = dbPath;
                            attachementDto.EntityType = (EntityType)Enum.Parse(typeof(EntityType), entityType);
                            attachementDto.EntityId = entityId;
                            attachementDto.AttachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType);
                            var updateResult = await UpdateUploadAsync(attachementDto);
                            if (updateResult != null)
                            {

                            }
                        }

                        dbPath = dbPath.Replace(@"wwwroot\", string.Empty);

                    }

                    if (insertCount > 0)
                    {
                        result.Add("Status", "Success");
                        result.Add("Message", "Data save successfully!");
                    }
                    else
                    {
                        result.Add("Status", "Warning");
                        result.Add("Message", "Fail to save!");
                    }

                    return new JsonResult(result);
                }
                else
                {
                    result.Add("Status", "Warning");
                    result.Add("Message", "Attachment not found!");
                    return new JsonResult(result);
                }
            }
            catch (Exception ex)
            {
                result.Add("Status", "Error");
                result.Add("Message", $"Internal server error: {ex}");
                return new JsonResult(result);
            }
        }



        [HttpPost, ActionName("DeleteFileComplain")]
        public IActionResult DeleteFileComplain(FileDeleteInputDto input)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, input.FilePath);
                FileInfo fi = new FileInfo(filePath);
                if (fi != null)
                {
                    System.IO.File.Delete(filePath);
                    fi.Delete();
                }
                return new JsonResult(input.FilePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost, ActionName("DeleteFileAllotment")]
        public IActionResult DeleteFileAllotment(FileDeleteInputDto input)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, input.FilePath);
                FileInfo fi = new FileInfo(filePath);
                if (fi != null)
                {
                    System.IO.File.Delete(filePath);
                    fi.Delete();
                }
                return new JsonResult(input.FilePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet]
        public async Task<DocumentsAttachmentDto>? GetDocumentAsync(string entityType, long? entityId, string attachmentType)
        {
            var attachment = await _attachmentRepository.GetAsync(x => x.EntityType == (EntityType)Enum.Parse(typeof(EntityType), entityType) && x.EntityId == entityId && x.AttachmentType == (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType));
            if (attachment != null)
            {
                try
                {
                    return ObjectMapper.Map<DocumentsAttachment, DocumentsAttachmentDto>(attachment);
                }
                catch (Exception ex)
                {

                }
            }

            return null;
        }
        [HttpPut]
        public async Task<DocumentsAttachmentDto> UpdateUploadAsync(DocumentsAttachmentDto input)
        {
            var updateItem = ObjectMapper.Map<DocumentsAttachmentDto, DocumentsAttachment>(input);

            var item = await _attachmentRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<DocumentsAttachment, DocumentsAttachmentDto>(item);
        }
        //[HttpPost, ActionName("Allotment")]
        //[DisableRequestSizeLimit]
        //public IActionResult FileUploadAllotment()
        //{
        //    Dictionary<string, string> result = new Dictionary<string, string>();
        //    try
        //    {
        //        var files = Request.Form.Files;
        //        if (files.Count > 0)
        //        {
        //            var allotmentIdStr = Request.Form["allotmentId"].ToString();
        //            int allotmentId = string.IsNullOrEmpty(allotmentIdStr) ? 0 : Convert.ToInt32(allotmentIdStr);
        //            var entityType = Request.Form["entityType"].ToString();
        //            if (string.IsNullOrEmpty(entityType))
        //            {
        //                entityType = "None";
        //            }
        //            var attachmentType = Request.Form["attachmentType"].ToString();
        //            if (string.IsNullOrEmpty(attachmentType))
        //            {
        //                attachmentType = "None";
        //            }
        //            var directoryName = Request.Form["directoryName"][0];
        //            var folderName = Path.Combine("wwwroot", "uploads", directoryName);
        //            int insertCount = 0;

        //            foreach (var file in files)
        //            {
        //                if (!Directory.Exists(folderName))
        //                {
        //                    DirectoryInfo di = Directory.CreateDirectory(folderName);
        //                }

        //                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //                var fullPath = Path.Combine(pathToSave, fileName);
        //                var dbPath = Path.Combine(folderName, fileName);
        //                using (var stream = new FileStream(fullPath, FileMode.Create))
        //                {
        //                    file.CopyTo(stream);
        //                }
        //                // save attachment
        //                var attachement = new Attachment();
        //                attachement.FileName = fileName;
        //                attachement.OriginalFileName = fileName;
        //                attachement.Path = dbPath;
        //                attachement.EntityType = (EntityType)Enum.Parse(typeof(EntityType), entityType);
        //                attachement.EntityId = allotmentId;
        //                attachement.AttachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType);
        //                attachmentRepository.InsertAsync(attachement);
        //                insertCount += 1;
        //                dbPath = dbPath.Replace(@"wwwroot\", string.Empty);
        //            }

        //            if (insertCount > 0)
        //            {
        //                result.Add("Status", "Success");
        //                result.Add("Message", "Data save successfully!");
        //            }
        //            else
        //            {
        //                result.Add("Status", "Warning");
        //                result.Add("Message", "Fail to save!");
        //            }

        //            return new JsonResult(result);
        //        }
        //        else
        //        {
        //            result.Add("Status", "Warning");
        //            result.Add("Message", "Attachment not found!");
        //            return new JsonResult(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Add("Status", "Error");
        //        result.Add("Message", $"Internal server error: {ex}");
        //        return new JsonResult(result);
        //    }
        //}
    }
}
