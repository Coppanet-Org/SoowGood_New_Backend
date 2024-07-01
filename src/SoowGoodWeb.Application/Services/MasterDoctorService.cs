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
    public class MasterDoctorService : SoowGoodWebAppService, IMasterDoctorService
    {
        private readonly IRepository<MasterDoctor> _masterDoctorRepository;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public MasterDoctorService(IRepository<MasterDoctor>masterDoctorRepository, IRepository<DoctorProfile> doctorProfileRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _masterDoctorRepository =masterDoctorRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<MasterDoctorDto> CreateAsync(MasterDoctorInputDto input)
        {
            var result = new MasterDoctorDto();
            var doctor = await _doctorProfileRepository.GetAsync(d => d.Id == input.DoctorProfileId);
            var newEntity = ObjectMapper.Map<MasterDoctorInputDto, MasterDoctor>(input);

            var masterDoctor = await _masterDoctorRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();
            result = ObjectMapper.Map<MasterDoctor, MasterDoctorDto>(masterDoctor);
            result.DoctorName = doctor.FullName;
            return result;//ObjectMapper.Map<MasterDoctor, MasterDoctorDto>(result);
        }

        public async Task<MasterDoctorDto> GetAsync(int id)
        {
            var item = await _masterDoctorRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<MasterDoctor, MasterDoctorDto>(item);
        }
        public async Task<List<MasterDoctorDto>> GetListAsync()
        {
            var doctors = await _masterDoctorRepository.GetListAsync();
            return ObjectMapper.Map<List<MasterDoctor>, List<MasterDoctorDto>>(doctors);
        }
        public async Task<List<MasterDoctorDto>> GetMasterDoctorListByDoctorIdAsync(int doctorId)
        {
            var doctors = await _masterDoctorRepository.WithDetailsAsync(d => d.DoctorProfile);
            var doctDegrees = doctors.Where(dd => dd.DoctorProfileId == doctorId).ToList();
            return ObjectMapper.Map<List<MasterDoctor>, List<MasterDoctorDto>>(doctDegrees);
        }
        public async Task<List<MasterDoctorDto>> GetListByDoctorIdAsync(int doctorId)
        {
            List<MasterDoctorDto> list = null;
            var items = await _masterDoctorRepository.WithDetailsAsync(d => d.DoctorProfile);
            items = items.Where(i => i.DoctorProfileId == doctorId);
            if (items.Any())
            {
                list = new List<MasterDoctorDto>();
                foreach (var item in items)
                {
                    list.Add(new MasterDoctorDto()
                    {
                        Id = item.Id,
                        DoctorName = item.DoctorProfile?.FullName,
                       
                    });
                }
            }

            return list;
        }
        public async Task<MasterDoctorDto> UpdateAsync(MasterDoctorInputDto input)
        {
            var updateItem = ObjectMapper.Map<MasterDoctorInputDto, MasterDoctor>(input);

            var item = await _masterDoctorRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<MasterDoctor, MasterDoctorDto>(item);
        }

        public async Task DeleteAsync(long id)
        {
            await _masterDoctorRepository.DeleteAsync(d => d.Id == id);
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
