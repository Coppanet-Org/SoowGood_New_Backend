using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IDoctorScheduleService : IApplicationService
    {
        Task<List<DoctorScheduleDto>> GetListAsync();
        Task<DoctorScheduleDto> GetAsync(int id);
        Task<DoctorScheduleDto> CreateAsync(DoctorScheduleInputDto input);
        Task<DoctorScheduleDto> UpdateAsync(DoctorScheduleInputDto input);

        //Task<List<DoctorScheduledDayOffDto>> GetDayOffsListAsync();
        //Task<DoctorScheduledDayOffDto> GetDayOffsAsync(int id);
        //Task<DoctorScheduledDayOffDto> CreateDayOffAsync(DoctorScheduledDayOffInputDto input);
        //Task<DoctorScheduledDayOffDto> UpdateDayOffAsync(DoctorScheduledDayOffInputDto input);
        //Task<DoctorScheduleDto> GetByUserIdAsync(long doctorId);
    }
}
