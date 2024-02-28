using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

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
            var diagonsticPathologyServiceManagements = await _diagonsticPathologyServiceManagementRepository.GetListAsync();
            return ObjectMapper.Map<List<DiagonsticPathologyServiceManagement>, List<DiagonsticPathologyServiceManagementDto>>(diagonsticPathologyServiceManagements);
        }   
    }
}
