using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Account;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;

namespace SoowGoodWeb.Services
{
    public class DoctorProfileService : SoowGoodWebAppService, IDoctorProfileService
    {
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;
        private readonly IRepository<DoctorDegree> _doctorDegreeRepository;
        private readonly IRepository<DoctorSpecialization> _doctorSpecializationRepository;
        private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
        private readonly IRepository<DocumentsAttachment> _documentsAttachment;
        private readonly IRepository<FinancialSetup> _financialSetup;
        private readonly IRepository<DoctorFeesSetup> _doctorFeesSetup;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public DoctorProfileService(IRepository<DoctorProfile> doctorProfileRepository
                                    , IUnitOfWorkManager unitOfWorkManager
                                    , IRepository<DoctorDegree> doctorDegreeRepository
                                    , IRepository<DoctorSpecialization> doctorSpecializationRepository
                                    , IRepository<DoctorSchedule> doctorScheduleRepository
                                    , IRepository<DocumentsAttachment> documentsAttachment
                                    , IRepository<FinancialSetup> financialSetup
                                    , IRepository<DoctorFeesSetup> doctorFeesSetup)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _doctorDegreeRepository = doctorDegreeRepository;
            _doctorSpecializationRepository = doctorSpecializationRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
            _documentsAttachment = documentsAttachment;
            _financialSetup = financialSetup;
            _doctorFeesSetup = doctorFeesSetup;
        }
        public async Task<DoctorProfileDto> CreateAsync(DoctorProfileInputDto input)
        {
            var result = new DoctorProfileDto();
            try
            {

                var totalDoctors = await _doctorProfileRepository.GetListAsync();
                var count = totalDoctors.Count();
                var date = DateTime.Now;
                input.DoctorCode = "SGD" + date.ToString("yyyyMMdd") + (count + 1);
                var newEntity = ObjectMapper.Map<DoctorProfileInputDto, DoctorProfile>(input);

                var doctorProfile = await _doctorProfileRepository.InsertAsync(newEntity);

                //await _unitOfWorkManager.Current.SaveChangesAsync();
                result = ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(doctorProfile);
                //return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public async Task<DoctorProfileDto> GetAsync(int id)
        {
            //var item = await _doctorProfileRepository.GetAsync(x => x.Id == id);

            //return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);

            var item = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, sp => sp.Speciality, d => d.DoctorSpecialization);

            var profile = item.FirstOrDefault(item => item.Id == id);

            var result = profile != null ? ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(profile) : null;

            return result;
        }

        public async Task<DoctorProfileDto> GetDoctorDetailsByAdminAsync(int id)
        {
            DoctorProfileDto? result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            if (!profileWithDetails.Any())
            {
                return result;
            }
            var profile = profileWithDetails.FirstOrDefault(profile => profile.Id == id);
            if (profile == null) { return result; }
            result = new DoctorProfileDto();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var degrees = medicaldegrees.Where(i => i.DoctorProfileId == profile.Id).ToList();
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(degrees);


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var specializations = medcalSpecializations.Where(i => i.DoctorProfileId == profile.Id).ToList();
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(specializations);

            result.Id = profile.Id;
            result.Degrees = doctorDegrees;
            result.DoctorSpecialization = doctorSpecializations;
            result.FullName = profile.FullName;
            result.DoctorTitle = profile.DoctorTitle;
            result.DoctorTitleName = profile.DoctorTitle > 0 ? ((DoctorTitle)profile.DoctorTitle).ToString() : "n/a";
            result.MaritalStatus = profile.MaritalStatus;
            result.MaritalStatusName = profile.MaritalStatus > 0 ? ((MaritalStatus)profile.MaritalStatus).ToString() : "n/a";
            result.City = profile.City;
            result.ZipCode = profile.ZipCode;
            result.Country = profile.Country;
            result.IdentityNumber = profile.IdentityNumber;
            result.BMDCRegNo = profile.BMDCRegNo;
            result.BMDCRegExpiryDate = profile.BMDCRegExpiryDate;
            result.Email = profile.Email;
            result.MobileNo = profile.MobileNo;
            result.DateOfBirth = profile.DateOfBirth;
            result.Gender = profile.Gender;
            result.GenderName = profile.Gender > 0 ? ((Gender)profile.Gender).ToString() : "n/a";
            result.Address = profile.Address;
            result.ProfileRole = "Doctor";
            result.IsActive = profile.IsActive;
            result.UserId = profile.UserId;
            result.IsOnline = profile.IsOnline;
            result.profileStep = profile.profileStep;
            result.createFrom = profile.createFrom;
            result.DoctorCode = profile.DoctorCode;
            result.SpecialityId = profile.SpecialityId;
            result.SpecialityName = profile.SpecialityId > 0 ? profile.Speciality.SpecialityName : "n/a";

            return result;
        }

        public async Task<List<DoctorProfileDto>> GetDoctorListSearchByNameAsync(string? name, FilterModel? filterModel) //, int? skipValue, int? currentLimit)
        {
            List<DoctorProfileDto> result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<DoctorProfileDto>();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            if (!string.IsNullOrEmpty(name))
            {
                profiles = profiles.Where(p => p.FullName == name).ToList();
            }

            profiles = profiles.Skip(filterModel.Offset)
                               .Take(filterModel.Limit).ToList();

            foreach (var item in profiles)
            {
                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    Degrees = doctorDegrees.Where(d => d.DoctorProfileId == item.Id).ToList(),
                    DoctorSpecialization = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id).ToList(),
                    FullName = item.FullName,
                    DoctorTitle = item.DoctorTitle,
                    DoctorTitleName = item.DoctorTitle > 0 ? ((DoctorTitle)item.DoctorTitle).ToString() : "n/a",
                    MaritalStatus = item.MaritalStatus,
                    MaritalStatusName = item.MaritalStatus > 0 ? ((MaritalStatus)item.MaritalStatus).ToString() : "n/a",
                    City = item.City,
                    ZipCode = item.ZipCode,
                    Country = item.Country,
                    IdentityNumber = item.IdentityNumber,
                    BMDCRegNo = item.BMDCRegNo,
                    BMDCRegExpiryDate = item.BMDCRegExpiryDate,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,
                    UserId = item.UserId,
                    IsOnline = item.IsOnline,
                    profileStep = item.profileStep,
                    createFrom = item.createFrom,
                    DoctorCode = item.DoctorCode,
                    SpecialityId = item.SpecialityId,
                    SpecialityName = item.SpecialityId > 0 ? item.Speciality.SpecialityName : "n/a",
                });
            }

            return result;
        }

        public async Task<int> GetDoctorsCountByNameAsync(string? name) //, int? skipValue, int? currentLimit)
        {
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return 0;
            }
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            if (!string.IsNullOrEmpty(name))
            {
                profiles = profiles.Where(p => p.FullName == name).ToList();
            }

            return profiles.Count;
        }

        public async Task<List<DoctorProfileDto>> GetDoctorListFilterAsync(DataFilterModel? doctorFilterModel, FilterModel filterModel)
        {
            List<DoctorProfileDto> result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync(d=>d.DoctorProfile);

            

            profiles = (from doctors in profiles
                        join schedule in schedules on doctors.Id equals schedule.DoctorProfileId
                        select doctors).Distinct().ToList();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<DoctorProfileDto>();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);

            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            var attachedItems = await _documentsAttachment.WithDetailsAsync();

            var financialSetups = await _financialSetup.WithDetailsAsync();
            var fees = financialSetups.OrderBy(p => p.ProviderAmount).Where(a => a.ProviderAmount != null).ToList();

            var doctorFees = await _doctorFeesSetup.WithDetailsAsync(d => d.DoctorSchedule.DoctorProfile);

            if (!string.IsNullOrEmpty(doctorFilterModel?.name))
            {
                profiles = profiles.Where(p => p.FullName.ToLower().Contains(doctorFilterModel.name.ToLower().Trim())).ToList();
            }

            if (doctorFilterModel?.specialityId > 0)
            {
                profiles = profiles.Where(p => p.SpecialityId == doctorFilterModel?.specialityId).ToList();
                //doctorSpecializations = doctorSpecializations.Where(sp => sp.SpecialityId == doctorFilterModel.specialityId).ToList();
            }

            if (doctorFilterModel?.specializationId > 0)
            {
                doctorSpecializations = doctorSpecializations.Where(sp => sp.Id == doctorFilterModel.specializationId).ToList();
                profiles = (from t1 in profiles
                            join t2 in doctorSpecializations.Where(c => c.Id == doctorFilterModel.specializationId)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).ToList();
            }

            if (doctorFilterModel?.consultancyType > 0)
            {
                schedules = schedules.Where(c => c.ConsultancyType == doctorFilterModel.consultancyType);
                profiles = (from t1 in profiles
                            join t2 in schedules //.Where(c => c.ConsultancyType == doctorFilterModel.consultancyType)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).Distinct().ToList();
            }

            profiles = profiles.Skip(filterModel.Offset)
                               .Take(filterModel.Limit).ToList();

            foreach (var item in profiles)
            {
                var profilePics = attachedItems.Where(x => x.EntityType == EntityType.Doctor
                                                                && x.EntityId == item.Id
                                                                && x.AttachmentType == AttachmentType.ProfilePicture
                                                                && x.IsDeleted == false).FirstOrDefault();
                decimal? fee = 0;
                if (item.IsOnline == true)
                {
                    fee = fees.FirstOrDefault().ProviderAmount;
                }
                else
                {
                    var docfeees = doctorFees.Where(f => f.DoctorSchedule.DoctorProfile.Id == item.Id && f.TotalFee != null).OrderBy(a => a.TotalFee).ToList();
                    fee = docfeees?.FirstOrDefault()?.TotalFee;
                }

                var degrees = doctorDegrees.Where(d => d.DoctorProfileId == item.Id).ToList();
                string degStr = string.Empty;
                foreach (var d in degrees)
                {
                    degStr = degStr + d.DegreeName + ",";
                }

                degStr = degStr.Remove(degStr.Length - 1);

                var experties = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id && sp.SpecialityId == item.SpecialityId).ToList();
                string expStr = string.Empty;
                foreach (var e in experties)
                {
                    expStr = expStr + e.SpecializationName + ",";
                }

                expStr = expStr.Remove(expStr.Length - 1);

                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    Degrees = degrees,
                    Qualifications = degStr,
                    SpecialityId = item.SpecialityId,
                    SpecialityName = item.SpecialityId > 0 ? item.Speciality?.SpecialityName : "n/a",
                    DoctorSpecialization = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id && sp.SpecialityId == item.SpecialityId).ToList(),
                    AreaOfExperties = expStr,
                    FullName = item.FullName,
                    DoctorTitle = item.DoctorTitle,
                    DoctorTitleName = item.DoctorTitle > 0 ? ((DoctorTitle)item.DoctorTitle).ToString() : "n/a",
                    MaritalStatus = item.MaritalStatus,
                    MaritalStatusName = item.MaritalStatus > 0 ? ((MaritalStatus)item.MaritalStatus).ToString() : "n/a",
                    City = item.City,
                    ZipCode = item.ZipCode,
                    Country = item.Country,
                    IdentityNumber = item.IdentityNumber,
                    BMDCRegNo = item.BMDCRegNo,
                    BMDCRegExpiryDate = item.BMDCRegExpiryDate,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,
                    UserId = item.UserId,
                    IsOnline = item.IsOnline,
                    profileStep = item.profileStep,
                    createFrom = item.createFrom,
                    DoctorCode = item.DoctorCode,
                    ProfilePic = profilePics?.Path,
                    DisplayFee = fee
                });
            }

            return result;
        }
        //, int? skipValue, int? currentLimit)
        public async Task<int> GetDoctorsCountByFiltersAsync(DataFilterModel? doctorFilterModel)
        {
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return 0;
            }
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            if (!string.IsNullOrEmpty(doctorFilterModel?.name))
            {
                profiles = profiles.Where(p => p.FullName.ToLower().Contains(doctorFilterModel.name.ToLower())).ToList();
            }

            if (doctorFilterModel?.specialityId > 0)
            {
                profiles = profiles.Where(p => p.SpecialityId == doctorFilterModel?.specialityId).ToList();
                //doctorSpecializations = doctorSpecializations.Where(sp => sp.SpecialityId == doctorFilterModel.specialityId).ToList();
            }

            if (doctorFilterModel?.specializationId > 0)
            {
                doctorSpecializations = doctorSpecializations.Where(sp => sp.Id == doctorFilterModel.specializationId).ToList();
                profiles = (from t1 in profiles
                            join t2 in doctorSpecializations.Where(c => c.Id == doctorFilterModel.specializationId)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).ToList();
            }

            if (doctorFilterModel?.consultancyType > 0)
            {
                //schedules = schedules.Where(c=>c.ConsultancyType==consultType).ToList();
                profiles = (from t1 in profiles
                            join t2 in schedules.Where(c => c.ConsultancyType == doctorFilterModel.consultancyType)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).ToList();
            }

            return profiles.Count;
        }
        public async Task<List<DoctorProfileDto>> GetDoctorListWithSearchFilterAsync(string? name, ConsultancyType? consultancy, long? speciality, long? specialization, int? skipValue, int? currentLimit)
        {
            List<DoctorProfileDto> result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<DoctorProfileDto>();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            if (!string.IsNullOrEmpty(name))
            {
                profiles = profiles.Where(p => p.FullName == name).ToList();
            }

            if (speciality > 0)
            {
                profiles = profiles.Where(p => p.SpecialityId == speciality).ToList();
                doctorSpecializations = doctorSpecializations.Where(sp => sp.SpecialityId == speciality).ToList();
            }

            if (specialization > 0)
            {
                doctorSpecializations = doctorSpecializations.Where(sp => sp.Id == specialization).ToList();
                profiles = (from t1 in profiles
                            join t2 in doctorSpecializations.Where(c => c.Id == specialization)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).ToList();
            }

            if (consultancy > 0)
            {
                //schedules = schedules.Where(c=>c.ConsultancyType==consultType).ToList();
                profiles = (from t1 in profiles
                            join t2 in schedules.Where(c => c.ConsultancyType == consultancy)
                            on t1.Id equals t2.DoctorProfileId
                            select t1).ToList();
            }

            profiles = profiles.Skip((int)(skipValue > 0 ? skipValue : 0))
                               .Take((int)(currentLimit > 0 ? currentLimit : 0)).ToList();

            foreach (var item in profiles)
            {
                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    Degrees = doctorDegrees.Where(d => d.DoctorProfileId == item.Id).ToList(),
                    DoctorSpecialization = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id).ToList(),
                    FullName = item.FullName,
                    DoctorTitle = item.DoctorTitle,
                    DoctorTitleName = item.DoctorTitle > 0 ? ((DoctorTitle)item.DoctorTitle).ToString() : "n/a",
                    MaritalStatus = item.MaritalStatus,
                    MaritalStatusName = item.MaritalStatus > 0 ? ((MaritalStatus)item.MaritalStatus).ToString() : "n/a",
                    City = item.City,
                    ZipCode = item.ZipCode,
                    Country = item.Country,
                    IdentityNumber = item.IdentityNumber,
                    BMDCRegNo = item.BMDCRegNo,
                    BMDCRegExpiryDate = item.BMDCRegExpiryDate,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,
                    UserId = item.UserId,
                    IsOnline = item.IsOnline,
                    profileStep = item.profileStep,
                    createFrom = item.createFrom,
                    DoctorCode = item.DoctorCode,
                    SpecialityId = item.SpecialityId,
                    SpecialityName = item.SpecialityId > 0 ? item.Speciality.SpecialityName : "n/a",
                });
            }

            return result;
        }

        public async Task<DoctorProfileDto> GetByUserNameAsync(string userName)
        {
            var item = await _doctorProfileRepository.GetAsync(x => x.MobileNo == userName);

            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
        }
        public async Task<List<DoctorProfileDto>> GetListAsync()
        {
            List<DoctorProfileDto> result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);

            var schedules = await _doctorScheduleRepository.WithDetailsAsync(d => d.DoctorProfile);

            
            var profiles = profileWithDetails.ToList();

            profiles = (from doctors in profiles join schedule in schedules on doctors.Id equals schedule.DoctorProfileId
                        select doctors).Distinct().ToList();

            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<DoctorProfileDto>();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);

            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            var attachedItems = await _documentsAttachment.WithDetailsAsync();

            var financialSetups = await _financialSetup.WithDetailsAsync();
            var fees = financialSetups.OrderBy(p => p.ProviderAmount).Where(a => a.ProviderAmount != null).ToList();

            var doctorFees = await _doctorFeesSetup.WithDetailsAsync(d => d.DoctorSchedule.DoctorProfile);

            foreach (var item in profiles)
            {
                var profilePics = attachedItems.Where(x => x.EntityType == EntityType.Doctor
                                                                && x.EntityId == item.Id
                                                                && x.AttachmentType == AttachmentType.ProfilePicture
                                                                && x.IsDeleted == false).FirstOrDefault();

                decimal? fee = 0;
                if (item.IsOnline == true)
                {
                    fee = fees?.FirstOrDefault()?.ProviderAmount;
                }
                else
                {
                    var docfeees = doctorFees.Where(f => f.DoctorSchedule.DoctorProfile.Id == item.Id && f.TotalFee != null).OrderBy(a => a.TotalFee).ToList();
                    fee = docfeees?.FirstOrDefault()?.TotalFee;
                }

                var degrees = doctorDegrees.Where(d => d.DoctorProfileId == item.Id).ToList();
                string degStr = string.Empty;
                foreach (var d in degrees)
                {
                    degStr = degStr + d.DegreeName + ",";
                }

                degStr = degStr.Remove(degStr.Length - 1);

                var experties = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id && sp.SpecialityId == item.SpecialityId).ToList();
                string expStr = string.Empty;
                foreach (var e in experties)
                {
                    expStr = expStr + e.SpecializationName + ",";
                }

                expStr = expStr.Remove(expStr.Length - 1);

                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    Degrees = degrees,
                    Qualifications = degStr,
                    SpecialityId = item.SpecialityId,
                    SpecialityName = item.SpecialityId > 0 ? item.Speciality?.SpecialityName : "n/a",
                    DoctorSpecialization = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id && sp.SpecialityId == item.SpecialityId).ToList(),
                    AreaOfExperties = expStr,
                    FullName = item.FullName,
                    DoctorTitle = item.DoctorTitle,
                    DoctorTitleName = item.DoctorTitle > 0 ? ((DoctorTitle)item.DoctorTitle).ToString() : "n/a",
                    MaritalStatus = item.MaritalStatus,
                    MaritalStatusName = item.MaritalStatus > 0 ? ((MaritalStatus)item.MaritalStatus).ToString() : "n/a",
                    City = item.City,
                    ZipCode = item.ZipCode,
                    Country = item.Country,
                    IdentityNumber = item.IdentityNumber,
                    BMDCRegNo = item.BMDCRegNo,
                    BMDCRegExpiryDate = item.BMDCRegExpiryDate,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,
                    UserId = item.UserId,
                    IsOnline = item.IsOnline,
                    profileStep = item.profileStep,
                    createFrom = item.createFrom,
                    DoctorCode = item.DoctorCode,
                    ProfilePic = profilePics?.Path,
                    DisplayFee = fee
                });
            }

            return result;
            //var profiles = await _doctorProfileRepository.WithDetailsAsync(d => d.Degrees, s => s.DoctorSpecialization);
            //return ObjectMapper.Map<List<DoctorProfile>, List<DoctorProfileDto>>(profiles.ToList());
        }

        public async Task<List<DoctorProfileDto>> GetAllDoctorsSearchListAsync(string? name, int? consultType, long? speciality, long? specialization)
        {
            var profiles = await _doctorProfileRepository.WithDetailsAsync(d => d.Degrees, s => s.DoctorSpecialization);
            var specializations = await _doctorSpecializationRepository.GetListAsync(sp => sp.Id == specialization);
            //var doctors = new List<DoctorProfileDto>();
            if (profiles.Any())
            {
                //var doctors = from d in profiles join sp in specializations on d.Id equals sp.DoctorProfileId
                //profiles.Where(d => d.FullName == name && d.SpecialityId == speciality).ToList();
                //doctors = doctors.Where(sp=>sp.DoctorSpecialization)
            }
            return ObjectMapper.Map<List<DoctorProfile>, List<DoctorProfileDto>>(profiles.ToList());
        }

        public async Task<List<DoctorProfileDto>> GetListDoctorListByAdminAsync()
        {
            List<DoctorProfileDto>? result = null;
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
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,

                });
            }
            return result;
        }

        public async Task<DoctorProfileDto> UpdateActiveStatusByAdmin(int Id, bool activeStatus)
        {
            var user = await _doctorProfileRepository.GetAsync(x => x.Id == Id);
            if (user != null)
            {
                if (user.IsActive == false)
                {
                    user.IsActive = activeStatus;
                }
            }
            var item = await _doctorProfileRepository.UpdateAsync(user);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);

        }

        public async Task<DoctorProfileDto> UpdateDoctorsOnlineStatus(long Id, bool onlineStatus)
        {
            var user = await _doctorProfileRepository.GetAsync(x => x.Id == Id);

            if (user != null)
            {
                user.IsOnline = onlineStatus;
                var updatedUser = await _doctorProfileRepository.UpdateAsync(user);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(updatedUser);
            }
            else
            {
                return null;
            }
        }


        public async Task<DoctorProfileDto> GetByUserIdAsync(Guid userId)
        {
            var doctorProfiles = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, sp => sp.Speciality, d => d.DoctorSpecialization);
            var item = doctorProfiles.FirstOrDefault(x => x.UserId == userId);
            var result = item != null ? ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item) : null;
            return result;//ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
        }

        public async Task<DoctorProfileDto> UpdateAsync(DoctorProfileInputDto input)
        {
            var result = new DoctorProfileDto();
            try
            {
                var updateItem = ObjectMapper.Map<DoctorProfileInputDto, DoctorProfile>(input);
                var item = await _doctorProfileRepository.UpdateAsync(updateItem);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                result = ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
                if (result != null)
                {
                    if (input.Degrees?.Count > 0)
                    {
                        foreach (var d in input.Degrees)
                        {
                            var alldegrees = await _doctorDegreeRepository.WithDetailsAsync();
                            var existingDegree = alldegrees.FirstOrDefault(e => e.DegreeId == d.DegreeId && e.DoctorProfileId == d.DoctorProfileId);
                            //var exDegRes= ObjectMapper.Map<DoctorDegree, DoctorDegreeDto>(existingDegree);
                            if (existingDegree == null)
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
                            else
                            {
                                //var degree = new DoctorDegreeInputDto
                                //{
                                //    Id = exDegRes.Id,
                                //    DoctorProfileId = d.DoctorProfileId,
                                //    DegreeId = d.DegreeId,
                                //    //Duration = d.Duration,
                                //    PassingYear = d.PassingYear,
                                //    InstituteName = d.InstituteName,
                                //    InstituteCity = d.InstituteCity,
                                //    InstituteCountry = d.InstituteCountry
                                //};

                                existingDegree.DegreeId = d.DegreeId;
                                existingDegree.Duration = d.Duration;
                                existingDegree.PassingYear = d.PassingYear;
                                existingDegree.InstituteName = d.InstituteName;
                                existingDegree.InstituteCity = d.InstituteCity;
                                existingDegree.InstituteCountry = d.InstituteCountry;
                                //var updatedDegree = ObjectMapper.Map<DoctorDegreeDto, DoctorDegree>(exDegRes);

                                var doctorDegree = await _doctorDegreeRepository.UpdateAsync(existingDegree);

                                await _unitOfWorkManager.Current.SaveChangesAsync();

                                ObjectMapper.Map<DoctorDegree, DoctorDegreeDto>(doctorDegree);
                            }
                        }
                    }
                    if (input.DoctorSpecialization?.Count > 0)
                    {

                        foreach (var s in input.DoctorSpecialization)
                        {
                            var allExperties = await _doctorSpecializationRepository.WithDetailsAsync();
                            var existingSpecializations = allExperties.FirstOrDefault(e => e.SpecializationId == s.SpecializationId && e.DoctorProfileId == s.DoctorProfileId);
                            //var exSpRes = ObjectMapper.Map<DoctorSpecialization, DoctorSpecializationDto>(existingSpecializations);
                            if (existingSpecializations == null)
                            {
                                var specialization = new DoctorSpecializationInputDto
                                {
                                    DoctorProfileId = result.Id,
                                    SpecialityId = s.SpecialityId,
                                    SpecializationId = s.SpecializationId,
                                    DocumentName = s.DocumentName,
                                };
                                var newSpcializations = ObjectMapper.Map<DoctorSpecializationInputDto, DoctorSpecialization>(specialization);

                                var doctorSpecialization = await _doctorSpecializationRepository.InsertAsync(newSpcializations);

                                //await _unitOfWorkManager.Current.SaveChangesAsync();

                                ObjectMapper.Map<DoctorSpecialization, DoctorSpecializationDto>(doctorSpecialization);
                            }
                            else
                            {
                                //var specialization = new DoctorSpecializationInputDto
                                //{
                                //    Id = exSpRes.Id,
                                //    DoctorProfileId = s.DoctorProfileId,
                                //    SpecialityId = existingSpecializations.SpecialityId,
                                //    SpecializationId = s.SpecializationId,
                                //    DocumentName = s.DocumentName,
                                //};
                                existingSpecializations.SpecializationId = s.SpecializationId;
                                existingSpecializations.DocumentName = s.DocumentName;
                                //var updatedSpcializations = ObjectMapper.Map<DoctorSpecializationDto, DoctorSpecialization>(exSpRes);

                                var doctorSpecialization = await _doctorSpecializationRepository.UpdateAsync(existingSpecializations);

                                await _unitOfWorkManager.Current.SaveChangesAsync();

                                ObjectMapper.Map<DoctorSpecialization, DoctorSpecializationDto>(doctorSpecialization);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;//ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
        }

        public async Task<DoctorProfileDto> UpdateProfileStepAsync(long profileId, int step)
        {
            try
            {
                var profile = await _doctorProfileRepository.GetAsync(a => a.Id == profileId);//.FindAsync(input.Id);
                profile.profileStep = step;

                var item = await _doctorProfileRepository.UpdateAsync(profile);
                //await _unitOfWorkManager.Current.SaveChangesAsync();
                return ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<DoctorProfileDto>> GetCurrentlyOnlineDoctorListAsync()
        {
            List<DoctorProfileDto> result = null;
            var profileWithDetails = await _doctorProfileRepository.WithDetailsAsync(s => s.Degrees, p => p.Speciality, d => d.DoctorSpecialization);
            var profiles = profileWithDetails.Where(o => o.IsOnline == true).ToList();
            var schedules = await _doctorScheduleRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<DoctorProfileDto>();
            var medicaldegrees = await _doctorDegreeRepository.WithDetailsAsync(d => d.Degree);
            var doctorDegrees = ObjectMapper.Map<List<DoctorDegree>, List<DoctorDegreeDto>>(medicaldegrees.ToList());


            var medcalSpecializations = await _doctorSpecializationRepository.WithDetailsAsync(s => s.Specialization, sp => sp.Speciality);
            var doctorSpecializations = ObjectMapper.Map<List<DoctorSpecialization>, List<DoctorSpecializationDto>>(medcalSpecializations.ToList());

            foreach (var item in profiles)
            {
                result.Add(new DoctorProfileDto()
                {
                    Id = item.Id,
                    Degrees = doctorDegrees.Where(d => d.DoctorProfileId == item.Id).ToList(),
                    SpecialityId = item.SpecialityId,
                    SpecialityName = item.SpecialityId > 0 ? item.Speciality?.SpecialityName : "n/a",
                    DoctorSpecialization = doctorSpecializations.Where(sp => sp.DoctorProfileId == item.Id && sp.SpecialityId == item.SpecialityId).ToList(),
                    FullName = item.FullName,
                    DoctorTitle = item.DoctorTitle,
                    DoctorTitleName = item.DoctorTitle > 0 ? ((DoctorTitle)item.DoctorTitle).ToString() : "n/a",
                    MaritalStatus = item.MaritalStatus,
                    MaritalStatusName = item.MaritalStatus > 0 ? ((MaritalStatus)item.MaritalStatus).ToString() : "n/a",
                    City = item.City,
                    ZipCode = item.ZipCode,
                    Country = item.Country,
                    IdentityNumber = item.IdentityNumber,
                    BMDCRegNo = item.BMDCRegNo,
                    BMDCRegExpiryDate = item.BMDCRegExpiryDate,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    Address = item.Address,
                    ProfileRole = "Doctor",
                    IsActive = item.IsActive,
                    UserId = item.UserId,
                    IsOnline = item.IsOnline,
                    profileStep = item.profileStep,
                    createFrom = item.createFrom,
                    DoctorCode = item.DoctorCode,
                });
            }

            return result;
        }

        public async Task<DoctorProfileDto> UpdateDocotrProfileAsync(DoctorProfileInputDto input)
        {
            var result = new DoctorProfileDto();
            try
            {
                var itemDoctor = await _doctorProfileRepository.GetAsync(d => d.Id == input.Id);
                itemDoctor.FullName = input.FullName;
                itemDoctor.DoctorTitle = input.DoctorTitle;
                itemDoctor.Email = input.Email;
                itemDoctor.Gender = input.Gender;
                itemDoctor.DateOfBirth = input.DateOfBirth;
                itemDoctor.BMDCRegNo = input.BMDCRegNo;
                itemDoctor.BMDCRegExpiryDate = input.BMDCRegExpiryDate;
                itemDoctor.Address = input.Address;
                itemDoctor.City = input.City;
                itemDoctor.Country = input.Country;
                itemDoctor.ZipCode = input.ZipCode;
                itemDoctor.IdentityNumber = input.IdentityNumber;
                itemDoctor.SpecialityId = input.SpecialityId;

                var item = await _doctorProfileRepository.UpdateAsync(itemDoctor);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                result = ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
            }
            catch (Exception ex)
            {
            }
            return result;//ObjectMapper.Map<DoctorProfile, DoctorProfileDto>(item);
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
