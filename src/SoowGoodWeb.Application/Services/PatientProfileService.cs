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
            var totalPatients = await _patientProfileRepository.GetListAsync();
            var count = totalPatients.Count();
            input.PatientCode = "SG-P-" + (count + 1);
            var newEntity = ObjectMapper.Map<PatientProfileInputDto, PatientProfile>(input);

            var patientProfile = await _patientProfileRepository.InsertAsync(newEntity);

            //await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(patientProfile);
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
        public async Task<List<PatientProfileDto>> GetListByAdminAsync()
        {
            List<PatientProfileDto>? result = null;
            var allProfile = await _patientProfileRepository.GetListAsync(); ;
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
                    FullName = item.FullName,
                    Email = item.Email,
                    MobileNo = item.MobileNo,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    GenderName = item.Gender > 0 ? ((Gender)item.Gender).ToString() : "n/a",
                    BloodGroup = item.BloodGroup,
                    Address = item.Address,
                });
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
                itemPatient.IsSelf = input.IsSelf;
                itemPatient.PatientName = input.PatientName;
                itemPatient.PatientEmail = input.PatientEmail;
                itemPatient.PatientMobileNo = input.PatientMobileNo;
                
                //input.FullName = itemPatient.FullName;
                //input.DateOfBirth = itemPatient.DateOfBirth;
                //input.Gender = itemPatient.Gender;
                //input.Age = itemPatient.Age;
                //input.Email = itemPatient.Email;
                //input.Address = itemPatient.Address;
                //input.MobileNo = itemPatient.MobileNo;
                //input.BloodGroup = itemPatient.BloodGroup;
                //input.City = itemPatient.City;
                //input.Country = itemPatient.Country;
                //input.ZipCode = itemPatient.ZipCode;
                //input.CreatedBy = itemPatient.CreatedBy;
                //input.CratorCode = itemPatient.CratorCode;
                //input.CreatorEntityId = itemPatient.CreatorEntityId;
                //input.PatientCode = itemPatient.PatientCode;

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
    }
}
