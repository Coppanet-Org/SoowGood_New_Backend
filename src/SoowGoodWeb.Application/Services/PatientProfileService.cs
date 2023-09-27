using SoowGoodWeb.DtoModels;
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
        //private readonly IRepository<AgentDegree> _agentDegreeRepository;
        //private readonly IRepository<AgentSpecialization> _agentSpecializationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PatientProfileService(IRepository<PatientProfile> patientProfileRepository
                                    , IUnitOfWorkManager unitOfWorkManager)
        {
            _patientProfileRepository = patientProfileRepository;
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
        public async Task<PatientProfileDto> GetByUserIdAsync(Guid userId)
        {
            var item = await _patientProfileRepository.GetAsync(x => x.UserId == userId);
            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
        }

        public async Task<PatientProfileDto> UpdateAsync(PatientProfileInputDto input)
        {
            
            var itemP = GetAsync(input.Id);

            var upItemP = ObjectMapper.Map(itemP, input);

            //input.FullName = itemP.FullName;
            //input.IsSelf = itemP.IsSelf;
            //input.



            var updateItem = ObjectMapper.Map<PatientProfileInputDto, PatientProfile>(upItemP);

            var item = await _patientProfileRepository.UpdateAsync(updateItem);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<PatientProfile, PatientProfileDto>(item);
        }

        public async Task<List<PatientProfileDto>> GetPatientListByUserProfileIdAsync(long profileId)
        {
            var profiles = await _patientProfileRepository.GetListAsync(p => p.CreatorEntityId == profileId);
            return ObjectMapper.Map<List<PatientProfile>, List<PatientProfileDto>>(profiles);
        }
    }
}
