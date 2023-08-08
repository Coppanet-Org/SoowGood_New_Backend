using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IDoctorScheduleDaySessionService : IApplicationService
    {
        Task<List<DoctorScheduleDaySessionDto>> GetListAsync();
        Task<DoctorScheduleDaySessionDto> GetAsync(int id);
        Task<ResponseDto> CreateAsync(DoctorScheduleDaySessionInputDto input);
        Task<ResponseDto> UpdateAsync(DoctorScheduleDaySessionInputDto input);
    }
}
