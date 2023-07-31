using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DoctorScheduleService : SoowGoodWebAppService, IDoctorScheduleService//, IDoctorScheduleDayOffService
    {
        private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorScheduleService(IRepository<DoctorSchedule> doctorScheduleRepository,  IUnitOfWorkManager unitOfWorkManager)
        {
            _doctorScheduleRepository = doctorScheduleRepository;

            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<DoctorScheduleDto> CreateAsync(DoctorScheduleInputDto input)
        {
            var newEntity = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

            var doctorSchedule = await _doctorScheduleRepository.InsertAsync(newEntity);

            //await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(doctorSchedule);
        }

        public async Task<DoctorScheduleDto> GetAsync(int id)
        {
            var item = await _doctorScheduleRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }
        public async Task<List<DoctorScheduleDto>> GetListAsync()
        {
            var profiles = await _doctorScheduleRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(profiles);
        }
        //public async Task<DoctorScheduleDto> GetByUserIdAsync(Guid userId)
        //{
        //    var item = await _doctorScheduleRepository.GetAsync(x => x.UserId == userId);
        //    return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        //}
        
        public async Task<DoctorScheduleDto> UpdateAsync(DoctorScheduleInputDto input)
        {
            var updateItem = ObjectMapper.Map<DoctorScheduleInputDto, DoctorSchedule>(input);

            var item = await _doctorScheduleRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<DoctorSchedule, DoctorScheduleDto>(item);
        }

        //public async Task<List<DoctorScheduleDto>> GetListAsync()
        //{
        //    List<DoctorScheduleDto> list = null;
        //    var items = await _doctorScheduleRepository.WithDetailsAsync(p => p.District);
        //    if (items.Any())
        //    {
        //        list = new List<DoctorScheduleDto>();
        //        foreach (var item in items)
        //        {
        //            list.Add(new DoctorScheduleDto()
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                Description = item.Description,
        //                DistrictId = item.DistrictId,
        //                DistrictName = item.District?.Name,
        //                CivilSubDivisionId = item.CivilSubDivisionId,
        //                EmSubDivisionId = item.EmSubDivisionId,
        //            });
        //        }
        //    }

        //    return list;
        //}
        //public async Task<List<QuarterDto>> GetListByDistrictAsync(int id)
        //{
        //    List<QuarterDto> list = null;
        //    var items = await repository.WithDetailsAsync(p => p.District);
        //    items = items.Where(i => i.DistrictId == id);
        //    if (items.Any())
        //    {
        //        list = new List<QuarterDto>();
        //        foreach (var item in items)
        //        {
        //            list.Add(new QuarterDto()
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                Description = item.Description,
        //                DistrictId = item.DistrictId,
        //                DistrictName = item.District?.Name,
        //                CivilSubDivisionId = item.CivilSubDivisionId,
        //                EmSubDivisionId = item.EmSubDivisionId,
        //            });
        //        }
        //    }

        //    return list;
        //}

        //public async Task<int> GetCountAsync()
        //{
        //    return (await quarterRepository.GetListAsync()).Count;
        //}
        //public async Task<List<QuarterDto>> GetSortedListAsync(FilterModel filterModel)
        //{
        //    var quarters = await quarterRepository.WithDetailsAsync();
        //    quarters = quarters.Skip(filterModel.Offset)
        //                    .Take(filterModel.Limit);
        //    return ObjectMapper.Map<List<Quarter>, List<QuarterDto>>(quarters.ToList());
        //}
        ////public async Task<int> GetCountBySDIdAsync(Guid? civilSDId, Guid? emSDId)
        //public async Task<int> GetCountBySDIdAsync(Guid? sdId)
        //{
        //    var quarters = await quarterRepository.WithDetailsAsync();
        //    //if (civilSDId != null && emSDId != null)
        //    //{
        //    //    quarters = quarters.Where(q => q.CivilSubDivisionId == civilSDId && q.EmSubDivisionId == emSDId);
        //    //}
        //    if (sdId != null)
        //    {
        //        quarters = quarters.Where(q => q.CivilSubDivisionId == sdId || q.EmSubDivisionId == sdId);
        //    }
        //    //else if (emSDId != null)
        //    //{
        //    //    quarters = quarters.Where(q => q.EmSubDivisionId == emSDId);
        //    //}
        //    return quarters.Count();
        //}
        ////public async Task<List<QuarterDto>> GetSortedListBySDIdAsync(Guid? civilSDId, Guid? emSDId, FilterModel filterModel)
        //public async Task<List<QuarterDto>> GetSortedListBySDIdAsync(Guid? sdId, FilterModel filterModel)
        //{
        //    var quarters = await quarterRepository.WithDetailsAsync();
        //    //if (civilSDId != null && emSDId != null)
        //    //{
        //    //    quarters = quarters.Where(q => q.CivilSubDivisionId == civilSDId && q.EmSubDivisionId == emSDId);
        //    //}
        //    //else if (civilSDId != null)
        //    //{
        //    if (sdId != null)
        //        quarters = quarters.Where(q => q.CivilSubDivisionId == sdId || q.EmSubDivisionId == sdId);
        //    //}
        //    //else if (emSDId != null)
        //    //{
        //    //    quarters = quarters.Where(q => q.EmSubDivisionId == emSDId);
        //    //}
        //    quarters = quarters.Skip(filterModel.Offset)
        //                    .Take(filterModel.Limit);
        //    return ObjectMapper.Map<List<Quarter>, List<QuarterDto>>(quarters.ToList());
        //}
        //public async Task<List<QuarterDto>> GetListBySDIdAsync(Guid? sdId)
        //{
        //    var quarters = await quarterRepository.WithDetailsAsync();
        //    if (sdId != null)
        //    {
        //        quarters = quarters.Where(q => q.CivilSubDivisionId == sdId || q.EmSubDivisionId == sdId);
        //    }
        //    return ObjectMapper.Map<List<Quarter>, List<QuarterDto>>(quarters.ToList());
        //}
    }
}
