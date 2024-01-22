﻿using SoowGoodWeb.DtoModels;
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
            var totalAgentMaters = await _agentProfileRepository.GetListAsync();
            var count = totalAgentMaters.Count();
            input.AgentCode = "SGAG00" + (count + 1);
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
            var result = new List<AgentProfileDto>();
            try 
            {
                var profiles = await _agentProfileRepository.WithDetailsAsync(m => m.AgentMaster, s => s.AgentSupervisor);
                var item = profiles.ToList();
                if (item.Any())
                {
                    foreach (var profile in item)
                    {
                        result.Add(new AgentProfileDto()
                        {
                            Id = profile.Id,
                            AgentCode = profile.AgentCode,
                            FullName = profile.FullName,
                            MobileNo = profile.MobileNo,
                            Email = profile.Email,
                            OrganizationName = profile.OrganizationName,
                            City = profile.City,
                            Address = profile.Address,
                            AgentMasterName = profile?.AgentMaster?.AgentMasterOrgName,
                            AgentSupervisorName = profile?.AgentSupervisor?.AgentSupervisorOrgName

                        });
                    }
                }
            }
            catch(Exception e) { }

            return result;//ObjectMapper.Map<List<AgentProfile>, List<AgentProfileDto>>(item);
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

        public async Task<AgentProfileDto> GetlByUserNameAsync(string userName)
        {
            var item = await _agentProfileRepository.GetAsync(x => x.MobileNo == userName);

            return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        }

    }
}
