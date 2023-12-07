using SoowGoodWeb.DtoModels;
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
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public FinancialSetupService(IRepository<FinancialSetup> financialSetupRepository, IRepository<PlatformFacility> platformFacilityRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _financialSetupRepository = financialSetupRepository;
            _platformFacilityRepository = platformFacilityRepository;
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
            var platformFacilitys = await _financialSetupRepository.GetListAsync();
            return ObjectMapper.Map<List<FinancialSetup>, List<FinancialSetupDto>>(platformFacilitys);
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
