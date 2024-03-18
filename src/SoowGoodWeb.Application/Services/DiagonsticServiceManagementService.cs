using AutoMapper;
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
    public class DiagonsticServiceManagementService : SoowGoodWebAppService, IDiagonsticPathologyServiceManagementService
    {
        private readonly IRepository<DiagonsticPathologyServiceManagement> _diagonsticPathologyServiceManagementRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public DiagonsticServiceManagementService(IRepository<DiagonsticPathologyServiceManagement> diagonsticPathologyServiceManagementRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _diagonsticPathologyServiceManagementRepository = diagonsticPathologyServiceManagementRepository;

            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<DiagonsticPathologyServiceManagementDto> CreateAsync(DiagonsticPathologyServiceManagementInputDto input)
        {
            var totalDiagServiceRequests = await _diagonsticPathologyServiceManagementRepository.GetListAsync();
            var count = totalDiagServiceRequests.Count();            
            var date = DateTime.Now;
            input.RequestDate = DateTime.Now;
            input.ServiceRequestCode = "SGPATH" + date.ToString("yyyyMMdd") + (count + 1);
            var newEntity = ObjectMapper.Map<DiagonsticPathologyServiceManagementInputDto, DiagonsticPathologyServiceManagement>(input);

            var diagonsticPathologyServiceManagement = await _diagonsticPathologyServiceManagementRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPathologyServiceManagement, DiagonsticPathologyServiceManagementDto>(diagonsticPathologyServiceManagement);
        }

        public async Task<DiagonsticPathologyServiceManagementDto> UpdateAsync(DiagonsticPathologyServiceManagementInputDto input)
        {
            var updateItem = ObjectMapper.Map<DiagonsticPathologyServiceManagementInputDto, DiagonsticPathologyServiceManagement>(input);

            var item = await _diagonsticPathologyServiceManagementRepository.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPathologyServiceManagement, DiagonsticPathologyServiceManagementDto>(item);
        }


        public async Task<DiagonsticPathologyServiceManagementDto> GetAsync(int id)
        {
            var item = await _diagonsticPathologyServiceManagementRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DiagonsticPathologyServiceManagement, DiagonsticPathologyServiceManagementDto>(item);
        }
        public async Task<List<DiagonsticPathologyServiceManagementDto>> GetListAsync()
        {
            List<DiagonsticPathologyServiceManagementDto>? result = null;

            var allDiagonsticPathologyServiceRequestDetails = await _diagonsticPathologyServiceManagementRepository.WithDetailsAsync(dp=>dp.DiagonsticPackage,p=>p.ServiceProvider);
            if (!allDiagonsticPathologyServiceRequestDetails.Any())
            {
                return result;
            }
            result = new List<DiagonsticPathologyServiceManagementDto>();
            foreach (var item in allDiagonsticPathologyServiceRequestDetails)
            {

                result.Add(new DiagonsticPathologyServiceManagementDto()
                {
                    Id = item.Id,
                    DiagonsticPackageId=item.DiagonsticPackageId,
                    DiagonsticPackageName=item.DiagonsticPackageId>0?item.DiagonsticPackage.PackageName: "n/a",
                    DiagonsticServiceType = item.DiagonsticServiceType,
                    DiagonsticServiceTypeName = item.DiagonsticServiceType > 0 ? ((DiagonsticServiceType)item.DiagonsticServiceType).ToString() : "n/a",
                    Discount = item.Discount,
                    FinalFee = item.FinalFee,
                    OrganizationCode = item.OrganizationCode,
                    PatientCode = item.PatientCode,
                    PatientName = item.PatientName,
                    PatientProfileId = item.PatientProfileId,
                    ProviderFee = item.ProviderFee,
                    RequestDate = item.RequestDate,
                    ServiceProviderId = item.ServiceProviderId,
                    ServiceProviderName=item.ServiceProviderId>0?item.ServiceProvider.ProviderOrganizationName: "n/a",
                    ServiceRequestCode = item.ServiceRequestCode,
                    ServiceRequestStatus = item.ServiceRequestStatus,
                    ServiceRequestStatusName=item.ServiceRequestStatus>0? ((ServiceRequestStatus)item.ServiceRequestStatus).ToString() : "n/a",

                });
            }
            var resList = result.OrderByDescending(d => d.Id).ToList();
            return resList;
            //var diagonsticPathologyServiceManagements = await _diagonsticPathologyServiceManagementRepository.GetListAsync();
            //return ObjectMapper.Map<List<DiagonsticPathologyServiceManagement>, List<DiagonsticPathologyServiceManagementDto>>(diagonsticPathologyServiceManagements);
        }   
    }
}
