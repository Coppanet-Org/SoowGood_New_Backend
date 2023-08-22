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
    public class AgentProfileService : SoowGoodWebAppService, IAgentProfileService
    {
        private readonly IRepository<AgentProfile> _agentProfileRepository;
        //private readonly IRepository<AgentDegree> _agentDegreeRepository;
        //private readonly IRepository<AgentSpecialization> _agentSpecializationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public AgentProfileService(IRepository<AgentProfile> agentProfileRepository
                                    , IUnitOfWorkManager unitOfWorkManager)
        {
            _agentProfileRepository = agentProfileRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<AgentProfileDto> CreateAsync(AgentProfileInputDto input)
        {
            var newEntity = ObjectMapper.Map<AgentProfileInputDto, AgentProfile>(input);

            var agentProfile = await _agentProfileRepository.InsertAsync(newEntity);

            //await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(agentProfile);
        }

        public async Task<AgentProfileDto> GetAsync(int id)
        {
            var item = await _agentProfileRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);

            //var item = await _agentProfileRepository.WithDetailsAsync();
            //var profile = item.FirstOrDefault(item => item.Id == id);
            //var result = profile != null ? ObjectMapper.Map<AgentProfile, AgentProfileDto>(profile) : null;

            //return result;
        }

        public async Task<AgentProfileDto> GetByUserNameAsync(string userName)
        {
            var item = await _agentProfileRepository.GetAsync(x => x.MobileNo == userName);

            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        }
        public async Task<List<AgentProfileDto>> GetListAsync()
        {
            var profiles = await _agentProfileRepository.GetListAsync();
            return ObjectMapper.Map<List<AgentProfile>, List<AgentProfileDto>>(profiles);
        }
        public async Task<AgentProfileDto> GetByUserIdAsync(Guid userId)
        {
            var item = await _agentProfileRepository.GetAsync(x => x.UserId == userId);
            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        }

        public async Task<AgentProfileDto> UpdateAsync(AgentProfileInputDto input)
        {
            var updateItem = ObjectMapper.Map<AgentProfileInputDto, AgentProfile>(input);

            var item = await _agentProfileRepository.UpdateAsync(updateItem);
            await _unitOfWorkManager.Current.SaveChangesAsync();                        
            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        }
        
    }
}
