using Microsoft.IdentityModel.Tokens;
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
    public class PatientProfileService : SoowGoodWebAppService, IPatientProfileService
    {
        private readonly IRepository<PatientProfile> _patientProfileRepository;
        private readonly IRepository<PatientProfile, long> _patientRepository;
        //private readonly IRepository<AgentDegree> _agentDegreeRepository;
        //private readonly IRepository<AgentSpecialization> _agentSpecializationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PatientProfileService(IRepository<PatientProfile> patientProfileRepository,
            IRepository<PatientProfile, long> patientRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _patientProfileRepository = patientProfileRepository;
            _patientRepository = patientRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<PatientProfileDto> CreateAsync(PatientProfileInputDto input)
        {
            var result = new PatientProfileDto();
            try
            {
                var totalPatients = await _patientProfileRepository.GetListAsync();
                var count = totalPatients.Count();
                var date = DateTime.Now;
                input.PatientCode = "SGP" + date.ToString("yyyyMMdd") + (count + 1);
                var newEntity = ObjectMapper.Map<PatientProfileInputDto, PatientProfile>(input);

                var patientProfile = await _patientProfileRepository.InsertAsync(newEntity);

                //await _unitOfWorkManager.Current.SaveChangesAsync();
                result = ObjectMapper.Map<PatientProfile, PatientProfileDto>(patientProfile);

            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public async Task<PatientProfileDto> GetAsync(long id)
        {
            var item = await _patientProfileRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);

            //var item = await _patientProfileRepository.WithDetailsAsync();
            //var profile = item.FirstOrDefault(item => item.Id == id);
            //var result = profile != null ? ObjectMapper.Map<PatientProfile, PatientProfileDto>(profile) : null;

            //return result;
        }

        public async Task<PatientProfileDto> GetByPhoneAndCodeAsync(string pCode, string pPhone)
        {
            var item = await _patientProfileRepository.GetAsync(x => x.PatientCode == pCode && x.PatientMobileNo == pPhone);//.WithDetailsAsync();
                                                                                                                            //var patient = item.Where(x => x.PatientCode == pCode && x.PatientMobileNo == pPhone).FirstOrDefault();
                                                                                                                            //if(patient!=null)
            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);

            //var item = await _patientProfileRepository.WithDetailsAsync();
            //var profile = item.FirstOrDefault(item => item.Id == id);
            //var result = profile != null ? ObjectMapper.Map<PatientProfile, PatientProfileDto>(profile) : null;

            //return null;
        }

        public async Task<PatientProfileDto> GetByUserNameAsync(string userName)
        {
            var item = await _patientProfileRepository.GetAsync(x => x.MobileNo == userName);

            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
        }

        public async Task<List<PatientProfileDto>> GetListAsync()
        {
            var profiles = await _patientProfileRepository.GetListAsync();
            return ObjectMapper.Map<List<PatientProfile>, List<PatientProfileDto>>(profiles);
        }

        public async Task<List<PatientProfileDto>> GetListPatientListByAdminAsync()
        {
            List<PatientProfileDto>? result = null;
            var allProfile = await _patientProfileRepository.GetListAsync();
            if (!allProfile.Any())
            {
                return result;
            }

            result = new List<PatientProfileDto>();
            foreach (var item in allProfile)
            {
                result.Add(new PatientProfileDto()
                {
                    Id = item.Id,
                    PatientName = item.PatientName,
                    PatientEmail = item.PatientEmail,
                    PatientMobileNo = item.PatientMobileNo,
                    PatientCode = item.PatientCode,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    BloodGroup = item.BloodGroup,
                    Address = item.Address,
                    ProfileRole = "Patient",

                }); ;
            }
            return result;
        }

        public async Task<PatientProfileDto> GetByUserIdAsync(Guid userId)
        {
            var item = await _patientProfileRepository.GetAsync(x => x.UserId == userId);
            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
        }

        public async Task<PatientProfileDto> UpdateAsync(PatientProfileInputDto input)
        {
            var result = new PatientProfileDto();
            try
            {
                var itemPatient = await _patientRepository.FindAsync(input.Id);
                if (itemPatient != null)
                {
                    itemPatient.FullName = input.FullName;
                    itemPatient.DateOfBirth = input.DateOfBirth;
                    itemPatient.Age = input.Age;
                    itemPatient.Gender = input.Gender;
                    itemPatient.BloodGroup = input.BloodGroup;
                    itemPatient.Address = input.Address;
                    itemPatient.City = input.City;
                    itemPatient.ZipCode = input.ZipCode;
                    itemPatient.Country = input.Country;
                    itemPatient.Email = input.Email;
                    itemPatient.PatientName = !string.IsNullOrEmpty(itemPatient.PatientName) ? itemPatient.PatientName : input.PatientName;
                    itemPatient.PatientMobileNo = !string.IsNullOrEmpty(itemPatient.PatientMobileNo) ? itemPatient.PatientMobileNo : input.PatientMobileNo;
                    itemPatient.PatientEmail = !string.IsNullOrEmpty(itemPatient.PatientEmail) ? itemPatient.PatientEmail : input.PatientEmail;

                    var item = await _patientRepository.UpdateAsync(itemPatient);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    result = ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }


        public async Task<List<PatientProfileDto>> GetPatientListByUserProfileIdAsync(long profileId, string role)
        {
            var profiles = await _patientProfileRepository.GetListAsync(p => p.CreatorEntityId == profileId && p.CreatorRole == role);
            return ObjectMapper.Map<List<PatientProfile>, List<PatientProfileDto>>(profiles);
        }

        public async Task<List<PatientProfileDto>> GetDoctorListFilterAsync(DataFilterModel? patientFilterModel, FilterModel filterModel)
        {
            List<PatientProfileDto> result = null;
            var profileWithDetails = await _patientProfileRepository.WithDetailsAsync();
            var profiles = profileWithDetails.ToList();
            var schedules = await _patientProfileRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<PatientProfileDto>();

            if (!string.IsNullOrEmpty(patientFilterModel?.name))
            {
                profiles = profiles.Where(p => p.FullName.ToLower().Contains(patientFilterModel.name.ToLower().Trim())).ToList();
            }

            profiles = profiles.Skip(filterModel.Offset)
                               .Take(filterModel.Limit).ToList();

            foreach (var item in profiles)
            {
                result.Add(new PatientProfileDto()
                {
                    Id = item.Id,
                    PatientName = item.PatientName,
                    PatientEmail = item.PatientEmail,
                    PatientMobileNo = item.PatientMobileNo,
                    PatientCode = item.PatientCode,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    BloodGroup = item.BloodGroup,
                    Address = item.Address,
                    ProfileRole = "Patient",
                });
            }
            return result;
        }

        public async Task<List<PatientProfileDto>> GetDoctorListByCreatorIdFilterAsync(long profileId, DataFilterModel? patientFilterModel, FilterModel filterModel)
        {
            List<PatientProfileDto> result = null;
            var profileWithDetails = await _patientProfileRepository.WithDetailsAsync();
            var profiles = profileWithDetails.Where(c => c.CreatorEntityId == profileId).ToList();
            var schedules = await _patientProfileRepository.WithDetailsAsync();
            //var scheduleCons = schedules.Where(s=>(s.ConsultancyType == consultType)
            if (!profileWithDetails.Any())
            {
                return result;
            }
            result = new List<PatientProfileDto>();

            if (!string.IsNullOrEmpty(patientFilterModel?.name))
            {
                profiles = profiles.Where(p => p.FullName.ToLower().Contains(patientFilterModel.name.ToLower().Trim())).ToList();
            }

            profiles = profiles.Skip(filterModel.Offset)
                               .Take(filterModel.Limit).ToList();

            foreach (var item in profiles)
            {
                result.Add(new PatientProfileDto()
                {
                    Id = item.Id,
                    PatientName = item.PatientName,
                    PatientEmail = item.PatientEmail,
                    PatientMobileNo = item.PatientMobileNo,
                    PatientCode = item.PatientCode,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    BloodGroup = item.BloodGroup,
                    Address = item.Address,
                    ProfileRole = "Patient",
                });
            }
            return result;
        }

        public async Task<List<PatientProfileDto>> GetPatientListBySearchUserProfileIdAsync(long profileId, string role, string name)
        {
            var profiles = await _patientProfileRepository.GetListAsync(p => p.CreatorEntityId == profileId && p.CreatorRole == role);
            if (!string.IsNullOrEmpty(name))
            {
                profiles = profiles.Where(p => p.FullName.Contains(name)).ToList();
            }
            return ObjectMapper.Map<List<PatientProfile>, List<PatientProfileDto>>(profiles);
        }
    }
}
