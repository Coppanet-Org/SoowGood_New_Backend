﻿using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class PlatformFinancialSetupService : SoowGoodWebAppService//, IPlatformServiceAppService
    {
        private readonly IRepository<PlatformService> _platformServiceRepository;
        private readonly IRepository<PlatformFinancialSetup> _platformFinancialSetup;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public PlatformFinancialSetupService(IRepository<PlatformService> platformServiceRepository, IRepository<PlatformFinancialSetup> platformFinancialSetup, IUnitOfWorkManager unitOfWorkManager)
        {
            _platformServiceRepository = platformServiceRepository;
            _platformFinancialSetup = platformFinancialSetup;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<PlatformFinancialSetupDto> CreateAsync(PlatformFinancialSetupInputDto input)
        {
            var newEntity = ObjectMapper.Map<PlatformFinancialSetupInputDto, PlatformFinancialSetup>(input);

            var platformService = await _platformFinancialSetup.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<PlatformFinancialSetup, PlatformFinancialSetupDto>(platformService);
        }

        public async Task<PlatformFinancialSetupDto> UpdateAsync(PlatformFinancialSetupInputDto input)
        {
            var updateItem = ObjectMapper.Map<PlatformFinancialSetupInputDto, PlatformFinancialSetup>(input);

            var item = await _platformFinancialSetup.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<PlatformFinancialSetup, PlatformFinancialSetupDto>(item);
        }


        public async Task<PlatformFinancialSetupDto> GetAsync(int id)
        {
            var item = await _platformFinancialSetup.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<PlatformFinancialSetup, PlatformFinancialSetupDto>(item);
        }
        public async Task<List<PlatformFinancialSetupDto>> GetListAsync()
        {
            var platformServices = await _platformFinancialSetup.GetListAsync();            
            return ObjectMapper.Map<List<PlatformFinancialSetup>, List<PlatformFinancialSetupDto>>(platformServices);
        }

        
        //public async Task<List<DoctorProfileDto>> GetListAsync()
        //{
        //    List<DoctorProfileDto> list = null;
        //    var items = await _doctorProfileRepository.WithDetailsAsync(p => p.District);
        //    if (items.Any())
        //    {
        //        list = new List<DoctorProfileDto>();
        //        foreach (var item in items)
        //        {
        //            list.Add(new DoctorProfileDto()
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
        //public async Task<DoctorProfileDto> UpdateAsync(DoctorProfileInputDto input)
        //{
        //    var updateItem = ObjectMapper.Map<DoctorProfileInputDto, DoctorProfile>(input);

        //    var item = await _doctorProfileRepository.UpdateAsync(updateItem);

        //    return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
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
