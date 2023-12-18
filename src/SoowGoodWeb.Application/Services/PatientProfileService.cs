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
        public PatientProfileService(IRepository<PatientProfile> patientProfileRepository, IRepository<PatientProfile, long> patientRepository

                                    , IUnitOfWorkManager unitOfWorkManager)
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
            try
            {
                var itemPatient = await _patientRepository.FindAsync(input.Id);
               // itemPatient.IsSelf = input.IsSelf;
                itemPatient.FullName = !string.IsNullOrEmpty(itemPatient.FullName) ? itemPatient.FullName : input.FullName;
                itemPatient.PatientName = !string.IsNullOrEmpty(itemPatient.PatientName) ? itemPatient.PatientName : input.PatientName;
                itemPatient.PatientEmail = !string.IsNullOrEmpty(itemPatient.PatientEmail) ? itemPatient.PatientEmail : input.PatientEmail;
                itemPatient.PatientMobileNo = !string.IsNullOrEmpty(itemPatient.PatientMobileNo) ? itemPatient.PatientMobileNo : input.PatientMobileNo;
                itemPatient.BloodGroup = !string.IsNullOrEmpty(itemPatient.BloodGroup) ? itemPatient.BloodGroup : input.BloodGroup;
                itemPatient.Age = itemPatient.Age > 0 ? itemPatient.Age : input.Age;
                itemPatient.DateOfBirth = !string.IsNullOrEmpty(itemPatient.DateOfBirth.ToString()) ? itemPatient.DateOfBirth : input.DateOfBirth;
                itemPatient.Gender = itemPatient.Gender > 0 ? itemPatient.Gender : input.Gender;                
                itemPatient.City = !string.IsNullOrEmpty(itemPatient.City) ? itemPatient.City : input.City;
                itemPatient.Country = !string.IsNullOrEmpty(itemPatient.Country) ? itemPatient.Country : input.Country;
                itemPatient.ZipCode = !string.IsNullOrEmpty(itemPatient.ZipCode) ? itemPatient.ZipCode : input.ZipCode;
                //input.CreatedBy = itemPatient.CreatedBy;
                //input.CratorCode = itemPatient.CratorCode;
                //input.CreatorEntityId = itemPatient.CreatorEntityId;
                itemPatient.PatientCode = !string.IsNullOrEmpty(itemPatient.ZipCode) ? itemPatient.ZipCode : input.PatientCode;

                //var updateItem = ObjectMapper.Map<PatientProfileInputDto, PatientProfile>(input);

                var item = await _patientRepository.UpdateAsync(itemPatient);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<PatientProfileDto>> GetPatientListByUserProfileIdAsync(long profileId)
        {
            var profiles = await _patientProfileRepository.GetListAsync(p => p.CreatorEntityId == profileId);
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
    }
}
