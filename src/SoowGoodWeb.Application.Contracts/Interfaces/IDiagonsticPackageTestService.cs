using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IDiagonsticPackageTestService : IApplicationService
    {
        Task<List<DiagonsticPackageTestDto>> GetListAsync();
        Task<DiagonsticPackageTestDto> GetAsync(int id);
        Task<DiagonsticPackageTestDto> CreateAsync(DiagonsticTestInputDto input);
        Task<DiagonsticPackageTestDto> UpdateAsync(DiagonsticPackageTestInputDto input);
    }
}
