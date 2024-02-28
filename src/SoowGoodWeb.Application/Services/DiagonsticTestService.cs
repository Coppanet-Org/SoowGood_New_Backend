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
    public class DiagonsticTestService : SoowGoodWebAppService, IDiagonsticTestService
    {
        private readonly IRepository<DiagonsticTest> _diagonsticTestRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public DiagonsticTestService(IRepository<DiagonsticTest> diagonsticTestRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _diagonsticTestRepository = diagonsticTestRepository;

            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<DiagonsticTestDto> CreateAsync(DiagonsticTestInputDto input)
        {
            var newEntity = ObjectMapper.Map<DiagonsticTestInputDto, DiagonsticTest>(input);

            var diagonsticTest = await _diagonsticTestRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticTest, DiagonsticTestDto>(diagonsticTest);
        }

        public async Task<DiagonsticTestDto> UpdateAsync(DiagonsticTestInputDto input)
        {
            var updateItem = ObjectMapper.Map<DiagonsticTestInputDto, DiagonsticTest>(input);

            var item = await _diagonsticTestRepository.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticTest, DiagonsticTestDto>(item);
        }


        public async Task<DiagonsticTestDto> GetAsync(int id)
        {
            var item = await _diagonsticTestRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DiagonsticTest, DiagonsticTestDto>(item);
        }
        public async Task<List<DiagonsticTestDto>> GetListAsync()
        {
            List<DiagonsticTestDto>? result = null;
            var alldiagonsticTestwithDetails = await _diagonsticTestRepository.WithDetailsAsync(s => s.ServiceProvider, p=>p.PathologyCategory, t=>t.PathologyTest);
            //var list = allsupervisorwithDetails.ToList();

            if (!alldiagonsticTestwithDetails.Any())
            {
                return result;
            }
            result = new List<DiagonsticTestDto>();
            foreach (var item in alldiagonsticTestwithDetails)
            {
                result.Add(new DiagonsticTestDto()
                {
                    Id = item.Id,
                    ServiceProviderId= item.ServiceProviderId,
                    ServiceProviderName=item.ServiceProviderId > 0 ? item.ServiceProvider.ProviderOrganizationName : null,
                    PathologyCategoryId=item.PathologyCategoryId,
                    PathologyCategoryName=item.PathologyCategoryId>0? item.PathologyCategory.PathologyCategoryName : null,
                    PathologyTestId=item.PathologyTestId,
                    PathologyTestName=item.PathologyTestId>0?item.PathologyTest.PathologyTestName : null,
                    ProviderRate=item.ProviderRate,
                });
            }
            return result;
            //var diagonsticTests = await _diagonsticTestRepository.GetListAsync();
            //return ObjectMapper.Map<List<DiagonsticTest>, List<DiagonsticTestDto>>(diagonsticTests).OrderByDescending(a=>a.Id).ToList();


            //result = result.OrderByDescending(a => a.AppointmentDate).ToList();
            //var list = result.OrderBy(item => item.AppointmentSerial)
            //    .GroupBy(item => item.AppointmentDate)
            //    .OrderBy(g => g.Key).Select(g => new { g }).ToList();


            //return result;
        }
    }
}
