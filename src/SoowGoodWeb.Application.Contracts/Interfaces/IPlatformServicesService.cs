using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IPlatformServicesService : IApplicationService
    {
        Task<List<PlatformServicesDto>> GetListAsync();
        Task<PlatformServicesDto> GetAsync(int id);
        Task<PlatformServicesDto> CreateAsync(PlatformServicesInputDto input);
        Task<PlatformServicesDto> UpdateAsync(PlatformServicesInputDto input);
    }
}
