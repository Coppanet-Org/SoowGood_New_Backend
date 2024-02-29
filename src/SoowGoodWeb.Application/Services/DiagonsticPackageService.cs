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
    public class DiagonsticPackageService : SoowGoodWebAppService, IDiagonsticPackageService
    {
        private readonly IRepository<DiagonsticPackage> _diagonsticPackageRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public DiagonsticPackageService(IRepository<DiagonsticPackage> diagonsticPackageRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _diagonsticPackageRepository = diagonsticPackageRepository;

            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<DiagonsticPackageDto> CreateAsync(DiagonsticPackageInputDto input)
        {
            var newEntity = ObjectMapper.Map<DiagonsticPackageInputDto, DiagonsticPackage>(input);

            var diagonsticPackage = await _diagonsticPackageRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPackage, DiagonsticPackageDto>(diagonsticPackage);
        }

        public async Task<DiagonsticPackageDto> UpdateAsync(DiagonsticPackageInputDto input)
        {
            var updateItem = ObjectMapper.Map<DiagonsticPackageInputDto, DiagonsticPackage>(input);

            var item = await _diagonsticPackageRepository.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPackage, DiagonsticPackageDto>(item);
        }


        public async Task<DiagonsticPackageDto> GetAsync(int id)
        {
            var item = await _diagonsticPackageRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DiagonsticPackage, DiagonsticPackageDto>(item);
        }
        public async Task<List<DiagonsticPackageDto>> GetListAsync()
        {
            List<DiagonsticPackageDto>? result = null;
            var alldiagonsticPackagewithDetails = await _diagonsticPackageRepository.WithDetailsAsync(s => s.ServiceProvider);
            //var list = allsupervisorwithDetails.ToList();

            if (!alldiagonsticPackagewithDetails.Any())
            {
                return result;
            }
            result = new List<DiagonsticPackageDto>();
            foreach (var item in alldiagonsticPackagewithDetails)
            {
                result.Add(new DiagonsticPackageDto()
                {
                    Id = item.Id,
                    ServiceProviderId = item.ServiceProviderId,
                    ServiceProviderName = item.ServiceProviderId > 0 ? item.ServiceProvider.ProviderOrganizationName : null,
                    PackageName = item.PackageName,
                    PackageDescription = item.PackageDescription,
                    ProviderRate = item.ProviderRate,
                });
            }
            return result;
            //var diagonsticPackages = await _diagonsticPackageRepository.GetListAsync();
            //return ObjectMapper.Map<List<DiagonsticPackage>, List<DiagonsticPackageDto>>(diagonsticPackages).OrderByDescending(a=>a.Id).ToList();


            //result = result.OrderByDescending(a => a.AppointmentDate).ToList();
            //var list = result.OrderBy(item => item.AppointmentSerial)
            //    .GroupBy(item => item.AppointmentDate)
            //    .OrderBy(g => g.Key).Select(g => new { g }).ToList();


            //return result;
        }
    }
}
