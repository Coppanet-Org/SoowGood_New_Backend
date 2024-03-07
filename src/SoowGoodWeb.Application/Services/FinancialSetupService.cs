using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class FinancialSetupService : SoowGoodWebAppService, IFinancialSetupService
    {
        private readonly IRepository<FinancialSetup> _financialSetupRepository;
        private readonly IRepository<PlatformFacility> _platformFacilityRepository;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
        private readonly IRepository<ServiceProvider> _serviceProviderRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public FinancialSetupService(IRepository<FinancialSetup> financialSetupRepository, IRepository<PlatformFacility> platformFacilityRepository, IRepository<DoctorProfile> doctorProfileRepository, IRepository<ServiceProvider> serviceProviderRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _financialSetupRepository = financialSetupRepository;
            _platformFacilityRepository = platformFacilityRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<FinancialSetupDto> CreateAsync(FinancialSetupInputDto input)
        {
            var result = new FinancialSetupDto();
            var platformFacility = await _platformFacilityRepository.GetAsync(d => d.Id == input.PlatformFacilityId);
            var newEntity = ObjectMapper.Map<FinancialSetupInputDto, FinancialSetup>(input);

            var financialSetup = await _financialSetupRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();
            result = ObjectMapper.Map<FinancialSetup, FinancialSetupDto>(financialSetup);
            result.FacilityName = platformFacility.ServiceName;
            return result;//ObjectMapper.Map<FinancialSetup, FinancialSetupDto>(result);
        }

        public async Task<FinancialSetupDto> GetAsync(int id)
        {
            var item = await _financialSetupRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<FinancialSetup, FinancialSetupDto>(item);
        }
        public async Task<List<FinancialSetupDto>> GetListAsync()
        {
            List<FinancialSetupDto>? result = null;
            var facilityEntityName = "";
            var allFinancialSetupDetails = await _financialSetupRepository.WithDetailsAsync(p=>p.PlatformFacility);
            if(!allFinancialSetupDetails.Any())
            {
                return result;
            }
            result = new List<FinancialSetupDto>();
            foreach (var item in allFinancialSetupDetails)
            {
                if (item.FacilityEntityID > 0)
                {
                    if(item.FacilityEntityType == FacilityEntityType.DoctorConsultation)
                    {
                        var doctorP = await _doctorProfileRepository.GetAsync(d=>d.Id == item.FacilityEntityID);
                        facilityEntityName = doctorP.FullName;
                    }
                    else if (item.FacilityEntityType == FacilityEntityType.ServiceFacility)
                    {
                        var sp = await _serviceProviderRepository.GetAsync(d=>d.Id==item.FacilityEntityID);
                        facilityEntityName = sp.ProviderOrganizationName;
                    }
                }
                result.Add(new FinancialSetupDto()
                {
                    
                    Id = item.Id,
                    PlatformFacilityId = item.PlatformFacilityId,                                        
                    
                    FacilityName = item.PlatformFacilityId > 0 ? item.PlatformFacility.ServiceName : "",
                    FacilityEntityType = item.FacilityEntityType,
                    FacilityEntityTypeName = item.FacilityEntityType>0? item.FacilityEntityType.ToString():"N/A",
                    DiagonsticServiceType = item.DiagonsticServiceType,
                    DiagonsticServiceTypeName = item.DiagonsticServiceType > 0 ? item.DiagonsticServiceType.ToString() : "N/A",
                    FacilityEntityID = item.FacilityEntityID,
                    FacilityEntityName=facilityEntityName,
                    AmountIn =item.AmountIn,
                    Amount=item.Amount,
                    ExternalAmount=item.ExternalAmount,
                    ExternalAmountIn=item.ExternalAmountIn,
                    IsActivie=item.IsActivie,
                    ProviderAmount=item.ProviderAmount,
                });
            }
            var resList = result.OrderByDescending(d=>d.Id).ToList();
            return resList;
        }
        
        public async Task<FinancialSetupDto> UpdateAsync(FinancialSetupInputDto input)
        {
            var updateItem = ObjectMapper.Map<FinancialSetupInputDto, FinancialSetup>(input);

            var item = await _financialSetupRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<FinancialSetup, FinancialSetupDto>(item);
        }

        public async Task DeleteAsync(long id)
        {
            await _financialSetupRepository.DeleteAsync(d => d.Id == id);
        }
        
    }
}
