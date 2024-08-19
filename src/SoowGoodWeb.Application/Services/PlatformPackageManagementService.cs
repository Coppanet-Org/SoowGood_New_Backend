﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Diagnostics;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoowGoodWeb.Services
{
    public class PlatformPackageManagementService : SoowGoodWebAppService, IPlatformPackageManagementService
    {
        private readonly IRepository<PlatformPackageManagement> _platformPackageManagementRepository;
        private readonly IRepository<DiagonsticTestRequested> _diagonsticTestRequestedRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public PlatformPackageManagementService(IRepository<PlatformPackageManagement> platformPackageManagementRepository, IRepository<DiagonsticTestRequested> diagonsticTestRequestedRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _platformPackageManagementRepository = platformPackageManagementRepository;
            _diagonsticTestRequestedRepository = diagonsticTestRequestedRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<PlatformPackageManagementDto> CreateAsync(PlatformPackageManagementInputDto input)
        {
            try 
            {
                var totalPackageRequests = await _platformPackageManagementRepository.GetListAsync();
                var count = totalPackageRequests.Count();
                var date = DateTime.Now;
                input.RequestDate = DateTime.Now;
                input.PackageRequestCode = "SGPACREQ" + date.ToString("yyyyMMdd") + (count + 1);
                var newEntity = ObjectMapper.Map<PlatformPackageManagementInputDto, PlatformPackageManagement>(input);

                var platformPackageManagement = await _platformPackageManagementRepository.InsertAsync(newEntity);

                await _unitOfWorkManager.Current.SaveChangesAsync();

                return ObjectMapper.Map<PlatformPackageManagement, PlatformPackageManagementDto>(platformPackageManagement);
            }
            catch (Exception ex) 
            {
                return null;
            }
            
        }

        public Task<PlatformPackageManagementDto> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PlatformPackageManagementDto>> GetListAsync()
        {

            List<PlatformPackageManagementDto>? result = null;

            var allPlatformPackageManagementDetails = await _platformPackageManagementRepository.WithDetailsAsync(s => s.PlatformPackage, p =>p.PatientProfile);
            if (!allPlatformPackageManagementDetails.Any())
            {
                return result;
            }
            result = new List<PlatformPackageManagementDto>();

            foreach (var item in allPlatformPackageManagementDetails)
            {

                result.Add(new PlatformPackageManagementDto()
                {
                    Id = item.Id,
                    AppointmentDate = item.AppointmentDate,
                    AppointmentPaymentStatus = item.AppointmentPaymentStatus,
                    PackageRequestCode = item.PackageRequestCode,
                    PlatformPackageId = item.PlatformPackageId,
                    DoctorName = item.PlatformPackageId > 0 ? item.PlatformPackage.PackageProvider.FullName : null,
                    PatientProfileId = item.PatientProfileId,
                    PatientCode=item.PatientProfileId>0?item.PatientProfile.PatientCode:null,
                    PatientName=item.PatientProfileId>0? item.PatientProfile.PatientName:null,
                    PatientMobileNo=item.PatientProfileId>0?item.PatientProfile.PatientMobileNo:null,
                    PackageFee=item.PackageFee,
                    RequestDate = item.RequestDate,
                    AppointmentStatus = item.AppointmentStatus,
                });
            }
            var resList = result.OrderByDescending(d => d.Id).ToList();
            return resList;
        }

        public async Task<PlatformPackageManagementDto> UpdateAsync(PlatformPackageManagementInputDto input)
        {
            var updateItem = ObjectMapper.Map<PlatformPackageManagementInputDto, PlatformPackageManagement>(input);

            var item = await _platformPackageManagementRepository.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<PlatformPackageManagement, PlatformPackageManagementDto>(item);
        }



       

        //public async Task<DiagonsticPathologyServiceManagementDto> UpdateServiceRequestStatusByAdmin(int Id, ServiceRequestStatus serviceRequestStatus)
        //{
        //    var user = await _diagonsticPathologyServiceManagementRepository.GetAsync(x => x.Id == Id);
        //    if (user != null)
        //    {
        //        if (user.ServiceRequestStatus != ServiceRequestStatus.Cancelled)
        //        {
        //            user.ServiceRequestStatus = serviceRequestStatus;
        //        }
        //    }
        //    var item = await _diagonsticPathologyServiceManagementRepository.UpdateAsync(user);
        //    await _unitOfWorkManager.Current.SaveChangesAsync();
        //    return ObjectMapper.Map<DiagonsticPathologyServiceManagement, DiagonsticPathologyServiceManagementDto>(item);

        //}

        //public async Task<DiagonsticPathologyServiceManagementDto> GetAsync(int id)
        //{
        //    DiagonsticPathologyServiceManagementDto? result = null;
        //    var serviceWithDetails = await _diagonsticPathologyServiceManagementRepository.WithDetailsAsync(dp => dp.DiagonsticPackage, p => p.ServiceProvider, d => d.DiagonsticTestRequested);
        //    if (!serviceWithDetails.Any())
        //    {
        //        return result;
        //    }
        //    var service = serviceWithDetails.FirstOrDefault(service => service.Id == id);
        //    if (service == null) { return result; }
        //    result = new DiagonsticPathologyServiceManagementDto();
        //    var testRequests = await _diagonsticTestRequestedRepository.WithDetailsAsync(d => d.DiagonsticTest);
        //    var requests = testRequests.Where(i => i.DiagonsticPathologyServiceManagementId == service.Id).ToList();
        //    var diagonsticTestRequests = ObjectMapper.Map<List<DiagonsticTestRequested>, List<DiagonsticTestRequestedDto>>(requests);



        //    result.Id = service.Id;
        //    result.DiagonsticPackageId = service.DiagonsticPackageId;
        //    result.DiagonsticPackageName = service.DiagonsticPackageId > 0 ? service.DiagonsticPackage.PackageName : "n/a";
        //    result.DiagonsticServiceType = service.DiagonsticServiceType;
        //    result.DiagonsticServiceTypeName = service.DiagonsticServiceType > 0 ? ((DiagonsticServiceType)service.DiagonsticServiceType).ToString() : "n/a";
        //    result.Discount = service.Discount;
        //    result.FinalFee = service.FinalFee;
        //    result.OrganizationCode = service.OrganizationCode;
        //    result.PatientCode = service.PatientCode;
        //    result.PatientName = service.PatientName;
        //    result.PatientProfileId = service.PatientProfileId;
        //    result.ProviderFee = service.ProviderFee;
        //    result.RequestDate = service.RequestDate;
        //    result.ServiceProviderId = service.ServiceProviderId;
        //    result.ServiceProviderName = service.ServiceProviderId > 0 ? service.ServiceProvider.ProviderOrganizationName : "n/a";
        //    result.ServiceRequestCode = service.ServiceRequestCode;
        //    result.ServiceRequestStatus = service.ServiceRequestStatus;
        //    result.ServiceRequestStatusName = service.ServiceRequestStatus > 0 ? ((ServiceRequestStatus)service.ServiceRequestStatus).ToString() : "n/a";
        //    result.DiagonsticTestRequested = diagonsticTestRequests;


        //    return result;
        //    //var item = await _diagonsticPathologyServiceManagementRepository.GetAsync(x => x.Id == id);

        //    //return ObjectMapper.Map<DiagonsticPathologyServiceManagement, DiagonsticPathologyServiceManagementDto>(item);
        //}
        //public async Task<List<DiagonsticPathologyServiceManagementDto>> GetListAsync()
        //{
        //    var id=0;
        //    List<DiagonsticPathologyServiceManagementDto>? result = null;

        //    var allDiagonsticPathologyServiceRequestDetails = await _diagonsticPathologyServiceManagementRepository.WithDetailsAsync(dp=>dp.DiagonsticPackage,p=>p.ServiceProvider,d=>d.DiagonsticTestRequested);
        //    if (!allDiagonsticPathologyServiceRequestDetails.Any())
        //    {
        //        return result;
        //    }


        //    result = new List<DiagonsticPathologyServiceManagementDto>();

        //    foreach (var item in allDiagonsticPathologyServiceRequestDetails)
        //    {


        //        var dto=new DiagonsticPathologyServiceManagementDto()
        //        {
        //            Id = item.Id,
        //            DiagonsticPackageId = item.DiagonsticPackageId,
        //            DiagonsticPackageName = item.DiagonsticPackageId > 0 ? item.DiagonsticPackage.PackageName : "n/a",
        //            DiagonsticServiceType = item.DiagonsticServiceType,
        //            DiagonsticServiceTypeName = item.DiagonsticServiceType > 0 ? ((DiagonsticServiceType)item.DiagonsticServiceType).ToString() : "n/a",
        //            Discount = item.Discount,
        //            FinalFee = item.FinalFee,
        //            OrganizationCode = item.OrganizationCode,
        //            PatientCode = item.PatientCode,
        //            PatientName = item.PatientName,
        //            PatientProfileId = item.PatientProfileId,
        //            ProviderFee = item.ProviderFee,
        //            RequestDate = item.RequestDate,
        //            ServiceProviderId = item.ServiceProviderId,
        //            ServiceProviderName = item.ServiceProviderId > 0 ? item.ServiceProvider.ProviderOrganizationName : "n/a",
        //            ServiceRequestCode = item.ServiceRequestCode,
        //            ServiceRequestStatus = item.ServiceRequestStatus,
        //            ServiceRequestStatusName = item.ServiceRequestStatus > 0 ? ((ServiceRequestStatus)item.ServiceRequestStatus).ToString() : "n/a",

        //        };

        //        if (item.DiagonsticTestRequested != null && item.DiagonsticTestRequested.Any())
        //        {
        //            dto.DiagonsticTestRequested = item.DiagonsticTestRequested.Select(d => new DiagonsticTestRequestedDto
        //            {
        //                DiagonsticPathologyServiceManagementId = d.DiagonsticPathologyServiceManagementId,
        //                DiagonsticTestId = d.DiagonsticTestId,
        //                PathologyCategoryAndTest = d.PathologyCategoryAndTest,
        //            }).ToList();
        //        };

        //        result.Add(dto);
        //    }
        //    var resList = result.OrderByDescending(d => d.Id).ToList();
        //    return resList;
        //    //var diagonsticPathologyServiceManagements = await _diagonsticPathologyServiceManagementRepository.GetListAsync();
        //    //return ObjectMapper.Map<List<DiagonsticPathologyServiceManagement>, List<DiagonsticPathologyServiceManagementDto>>(diagonsticPathologyServiceManagements);
        //}   
    }
}
