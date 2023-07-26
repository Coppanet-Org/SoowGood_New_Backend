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

namespace SoowGoodWeb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonController : AbpController
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRepository<Attachment> attachmentRepository;
        //private readonly IRepository<Complain> complainRepository;
        public CommonController(
            IWebHostEnvironment _webHostEnvironment,
            IRepository<Attachment> _attachmentRepository
            //IRepository<Complain> _complainRepository
           )
        {
            webHostEnvironment = _webHostEnvironment;
            attachmentRepository = _attachmentRepository;
            //complainRepository = _complainRepository;
        }

        [HttpPost, ActionName("Complain")]
        [DisableRequestSizeLimit]
        public IActionResult FileUploadComplain()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    var complainIdStr = Request.Form["complaintId"].ToString();
                    int complainId = string.IsNullOrEmpty(complainIdStr) ? 0 : Convert.ToInt32(complainIdStr);
                    var entityType = Request.Form["entityType"].ToString();
                    if (string.IsNullOrEmpty(entityType))
                    {
                        entityType = "None";
                    }
                    var attachmentType = Request.Form["attachmentType"].ToString();
                    if (string.IsNullOrEmpty(attachmentType))
                    {
                        attachmentType = "None";
                    }
                    var directoryName = Request.Form["directoryName"][0];
                    var folderName = Path.Combine("wwwroot", "uploads", directoryName);
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
                        var attachement = new Attachment();
                        attachement.FileName = fileName;
                        attachement.OriginalFileName = fileName;
                        attachement.Path = dbPath;
                        attachement.EntityType = (EntityType)Enum.Parse(typeof(EntityType), entityType);
                        attachement.EntityId = complainId;
                        attachement.AttachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType);
                        attachmentRepository.InsertAsync(attachement);
                        insertCount += 1;
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

        [HttpPost, ActionName("Allotment")]
        [DisableRequestSizeLimit]
        public IActionResult FileUploadAllotment()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    var allotmentIdStr = Request.Form["allotmentId"].ToString();
                    int allotmentId = string.IsNullOrEmpty(allotmentIdStr) ? 0 : Convert.ToInt32(allotmentIdStr);
                    var entityType = Request.Form["entityType"].ToString();
                    if (string.IsNullOrEmpty(entityType))
                    {
                        entityType = "None";
                    }
                    var attachmentType = Request.Form["attachmentType"].ToString();
                    if (string.IsNullOrEmpty(attachmentType))
                    {
                        attachmentType = "None";
                    }
                    var directoryName = Request.Form["directoryName"][0];
                    var folderName = Path.Combine("wwwroot", "uploads", directoryName);
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
                        var attachement = new Attachment();
                        attachement.FileName = fileName;
                        attachement.OriginalFileName = fileName;
                        attachement.Path = dbPath;
                        attachement.EntityType = (EntityType)Enum.Parse(typeof(EntityType), entityType);
                        attachement.EntityId = allotmentId;
                        attachement.AttachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), attachmentType);
                        attachmentRepository.InsertAsync(attachement);
                        insertCount += 1;
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

        //[HttpPost, ActionName("DeleteFileComplain")]
        //public IActionResult DeleteFileComplain(FileDeleteInput input)
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(webHostEnvironment.WebRootPath, input.FilePath);
        //        FileInfo fi = new FileInfo(filePath);
        //        if (fi != null)
        //        {
        //            System.IO.File.Delete(filePath);
        //            fi.Delete();
        //        }
        //        return new JsonResult(input.FilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

        //[HttpPost, ActionName("DeleteFileAllotment")]
        //public IActionResult DeleteFileAllotment(FileDeleteInput input)
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(webHostEnvironment.WebRootPath, input.FilePath);
        //        FileInfo fi = new FileInfo(filePath);
        //        if (fi != null)
        //        {
        //            System.IO.File.Delete(filePath);
        //            fi.Delete();
        //        }
        //        return new JsonResult(input.FilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

        //[HttpPost, ActionName("Example")]
        //public async void Example2()
        //{
        //    DateTime day = (DateTime.Now).Date;
        //    var sdePhone = "";
        //    var complaints = await complainRepository.WithDetailsAsync(p => p.ComplainHistories);
        //    complaints = complaints.Where(c => c.IsDeleted == false
        //                                    && c.ComplainStatus == ComplainStatus.New);
        //    if (complaints.Count() > 0)
        //    {
        //        foreach (var c in complaints)
        //        {
        //            var dc = (day - c.Date.Date).TotalDays;
        //            if (dc >= 3)
        //            {
        //                //sdePhone = organizaitonUnitService.GetPostingById(c.PostingId).Result.EmpPhoneMobile;
        //                if (!string.IsNullOrEmpty(sdePhone))
        //                {
        //                    SmsRequestInput otpInput2 = new SmsRequestInput();
        //                    otpInput2.Sms = String.Format("adfadfadf");
        //                    otpInput2.CsmsId = sdePhone;
        //                    otpInput2.Msisdn = "";

        //                    //var res2 = await notificationAppService.SendSmsTestAlpha(otpInput2);
        //                }

        //            }
        //        }
        //    }

        //    //return complaints.ToList().Count;
        //}

    }
}
