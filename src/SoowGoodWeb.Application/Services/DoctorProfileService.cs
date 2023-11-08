using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DoctorProfileService : SoowGoodWebAppService, IDoctorProfileService
    {
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
        private readonly IRepository<DoctorDegree> _doctorDegreeRepository;
        private readonly IRepository<DoctorSpecialization> _doctorSpecializationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorProfileService(IRepository<DoctorProfile> doctorProfileRepository
                                    , IUnitOfWorkManager unitOfWorkManager
                                    , IRepository<DoctorDegree> doctorDegreeRepository
                                    , IRepository<DoctorSpecialization> doctorSpecializationRepository)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _doctorDegreeRepository = doctorDegreeRepository;
            _doctorSpecializationRepository = doctorSpecializationRepository;
        }
        public async Task<DoctorProfileDto> CreateAsync(DoctorProfileInputDto input)
        {
            var totalDoctors = await _doctorProfileRepository.GetListAsync();
            var count = totalDoctors.Count();
            input.DoctorCode = "SG-D-" + (count + 1);
            var newEntity = ObjectMapper.Map<DoctorProfileInputDto, DoctorProfile>(input);

            var doctorProfile = await _doctorProfileRepository.InsertAsync(newEntity);

            //await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(doctorProfile);
        }

        public async Task<DoctorProfileDto> GetAsync(int id)
        {
            //var item = await _doctorProfileRepository.GetAsync(x => x.Id == id);

            //return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);

            var item = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, d => d.DoctorSpecialization);
            var profile = item.FirstOrDefault(item => item.Id == id);
            var result = profile != null ? ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(profile) : null;

            return result;
        }

        public async Task<DoctorProfileDto> GetByUserNameAsync(string userName)
        {
            var item = await _doctorProfileRepository.GetAsync(x => x.MobileNo == userName);

            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
        }
        public async Task<List<DoctorProfileDto>> GetListAsync()
        {
            var profiles = await _doctorProfileRepository.GetListAsync();
            return ObjectMapper.Map<List<DoctorProfile>, List<DoctorProfileDto>>(profiles);
        }

        public async Task<List<DoctorProfileDto>> GetListDoctorListByAdminAsync()
        {
            List<DoctorProfileDto>? result= null;
            var allProfile = await _doctorProfileRepository.GetListAsync(); ;
            if (!allProfile.Any())
            {
                return result;
            }

            result = new List<DoctorProfileDto>();
            foreach (var item in allProfile)
            {
                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender >0 ? ((Gender)item.Gender).ToString():"n/a",
                    Address = item.Address,
                    ProfileRole="Doctor"
                }) ;
            }
            return result;
        }

        public async Task<DoctorProfileDto> GetByUserIdAsync(Guid userId)
        {
            var item = await _doctorProfileRepository.GetAsync(x => x.UserId == userId);
            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
        }

        public async Task<DoctorProfileDto> UpdateAsync(DoctorProfileInputDto input)
        {
            var updateItem = ObjectMapper.Map<DoctorProfileInputDto, DoctorProfile>(input);

            var item = await _doctorProfileRepository.UpdateAsync(updateItem);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            var result = ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
            if (result != null)
            {
                if (input.Degrees?.Count > 0)
                {
                    foreach (var d in input.Degrees)
                    {
                        var degree = new DoctorDegreeInputDto
                        {
                            DoctorProfileId = result.Id,
                            DegreeId = d.DegreeId,
                            Duration = d.Duration,
                            PassingYear = d.PassingYear,
                            InstituteName = d.InstituteName,
                            InstituteCity = d.InstituteCity,
                            InstituteCountry = d.InstituteCountry
                        };
                        var newDegree = ObjectMapper.Map<DoctorDegreeInputDto, DoctorDegree>(degree);

                        var doctorDegree = await _doctorDegreeRepository.InsertAsync(newDegree);

                        //await _unitOfWorkManager.Current.SaveChangesAsync();

                        ObjectMapper.Map<DoctorDegree, DoctorDegreeDto>(doctorDegree);
                        
                    }
                }
                if (input.DoctorSpecialization?.Count > 0)
                {
                    foreach (var s in input.DoctorSpecialization)
                    {
                        var specialization = new DoctorSpecializationInputDto
                        {
                            DoctorProfileId = result.Id,
                            SpecialityId = s.SpecialityId,
                            SpecializationId = s.SpecializationId,
                            DocumentName = s.DocumentName,
                        };
                        var newDegree = ObjectMapper.Map<DoctorSpecializationInputDto, DoctorSpecialization>(specialization);

                        var doctorSpecialization = await _doctorSpecializationRepository.InsertAsync(newDegree);

                        //await _unitOfWorkManager.Current.SaveChangesAsync();

                        ObjectMapper.Map<DoctorSpecialization, DoctorSpecializationDto>(doctorSpecialization);
                    }
                }
            }
            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
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
