using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IAppointmentService : IApplicationService
    {
        Task<List<AppointmentDto>> GetListAsync();
        Task<AppointmentDto?> GetAsync(int id);
        Task<ResponseDto> CreateAsync(AppointmentInputDto input);
        Task<ResponseDto> UpdateAsync(AppointmentInputDto input);

        //Task<List<DoctorScheduleDaySessionDto>> GetSessionListAsync();
        //Task<DoctorScheduleDaySessionDto> GetSessionAsync(int id);
        //Task<ResponseDto> CreateSessionAsync(DoctorScheduleDaySessionInputDto input);
        //Task<ResponseDto> UpdateSessionAsync(DoctorScheduleDaySessionInputDto input);
        //Task<ResponseDto> DeleteSessionAsync(long id);

        //Task<List<DoctorScheduledDayOffDto>> GetDayOffsListAsync();
        //Task<DoctorScheduledDayOffDto> GetDayOffsAsync(int id);
        //Task<DoctorScheduledDayOffDto> CreateDayOffAsync(DoctorScheduledDayOffInputDto input);
        //Task<DoctorScheduledDayOffDto> UpdateDayOffAsync(DoctorScheduledDayOffInputDto input);
        //Task<DoctorScheduleDto> GetByUserIdAsync(long doctorId);
    }
}
