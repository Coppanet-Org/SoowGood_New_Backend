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
            if (input.PackageProviderId != null)
            {
                var doctor = await _doctorProfileRepository.GetAsync(d => d.Id == input.PackageProviderId);


                var newEntity = ObjectMapper.Map<PlatformPackageInputDto, PlatformPackage>(input);

                var platformPackage = await _platformPackageRepository.InsertAsync(newEntity);

                await _unitOfWorkManager.Current.SaveChangesAsync();
                result = ObjectMapper.Map<PlatformPackage, PlatformPackageDto>(platformPackage);
                result.DoctorName = doctor?.FullName;
            }
            return result;
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<PlatformPackageDto> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlatformPackageDto>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<PlatformPackageDto>> GetPlatformPackageListByAgentMasterIdAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<PlatformPackageDto> UpdateAsync(PlatformPackageInputDto input)
        {
            throw new NotImplementedException();
        }
    }
}
