using Microsoft.Extensions.Logging;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class PlatformPackageService : SoowGoodWebAppService, IPlatformPackageService
    {
        private readonly IRepository<PlatformPackage> _platformPackageRepository;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
       
        private readonly IRepository<DoctorSpecialization> _doctorSpecializationRepository;
        private readonly IRepository<DoctorDegree> _doctorDegreeRepository;
        private readonly IRepository<DocumentsAttachment> _documentsAttachment;
       
        private readonly ILogger<PlatformPackageService> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PlatformPackageService(IRepository<PlatformPackage> platformPackageRepository, IRepository<DoctorProfile> doctorProfileRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<DoctorSpecialization> doctorSpecializationRepository, IRepository<DoctorDegree> doctorDegreeRepository,
             ILogger<PlatformPackageService> logger,
             IRepository<DocumentsAttachment> documentsAttachment)
        {
            _platformPackageRepository = platformPackageRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
           
            _doctorSpecializationRepository = doctorSpecializationRepository;
            _doctorDegreeRepository = doctorDegreeRepository;
            _documentsAttachment = documentsAttachment;
           
            _logger = logger;
        }
        public async Task<PlatformPackageDto> CreateAsync(PlatformPackageInputDto input)
        {
            var result = new PlatformPackageDto();

            // If PackageProviderId is not null, try to retrieve the doctor profile
            if (input.PackageProviderId.HasValue)
            {
                var doctor = await _doctorProfileRepository.GetAsync(d => d.Id == input.PackageProviderId.Value);

                // If doctor is found, set the DoctorName
                if (doctor != null)
                {
                    result.DoctorName = doctor.FullName;
                }
                else
                {
                    throw new EntityNotFoundException($"DoctorProfile with id = {input.PackageProviderId.Value} does not exist.");
                }
            }

            // Map input DTO to entity and insert
            var newEntity = ObjectMapper.Map<PlatformPackageInputDto, PlatformPackage>(input);
            var platformPackage = await _platformPackageRepository.InsertAsync(newEntity);

            // Save changes
            await _unitOfWorkManager.Current.SaveChangesAsync();

            // Map the inserted entity back to DTO
            result = ObjectMapper.Map<PlatformPackage, PlatformPackageDto>(platformPackage);

            return result;
        }


       

        public async Task<PlatformPackageDto> GetAsync(int id)
        {
            var item = await _platformPackageRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<PlatformPackage, PlatformPackageDto>(item);
        }

        public async Task<List<PlatformPackageDto>> GetListAsync()
        {
            List<PlatformPackageDto>? result = null;

            var allPlatformPackageDetails = await _platformPackageRepository.WithDetailsAsync(s=>s.PackageProvider);
            if (!allPlatformPackageDetails.Any())
            {
                return result;
            }
            result = new List<PlatformPackageDto>();
            foreach (var item in allPlatformPackageDetails)
            {

                result.Add(new PlatformPackageDto()
                {
                    Id = item.Id,
                    PackageName = item.PackageName,
                    PackageTitle = item.PackageTitle,
                    PackageDescription = item.PackageDescription,
                    PackageFacilities = item.PackageFacilities,
                    PackageProviderId = item.PackageProviderId,
                    Price = item.Price,
                    Reason = item.Reason,
                    DoctorName = item.PackageProviderId > 0 ? item.PackageProvider.FullName : "",

                });
            }
            var resList = result.OrderByDescending(d => d.Id).ToList();
            return resList;
        }

        public Task<List<PlatformPackageDto>> GetPlatformPackageListByAgentMasterIdAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public async Task<PlatformPackageDto> UpdateAsync(PlatformPackageInputDto input)
        {
            var updateItem = ObjectMapper.Map<PlatformPackageInputDto, PlatformPackage>(input);

            var item = await _platformPackageRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<PlatformPackage, PlatformPackageDto>(item);
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
