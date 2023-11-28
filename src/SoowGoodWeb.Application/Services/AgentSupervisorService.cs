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
    public class AgentSupervisorService : SoowGoodWebAppService, IAgentSupervisorService
    {
        private readonly IRepository<AgentSupervisor> _agentSupervisorRepository;
        //private readonly IRepository<AgentDegree> _agentDegreeRepository;
        //private readonly IRepository<AgentSpecialization> _agentSpecializationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public AgentSupervisorService(IRepository<AgentSupervisor> agentSupervisorRepository
                                    , IUnitOfWorkManager unitOfWorkManager)
        {
            _agentSupervisorRepository = agentSupervisorRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<AgentSupervisorDto> CreateAsync(AgentSupervisorInputDto input)
        {
            var totalAgentSupervisors = await _agentSupervisorRepository.GetListAsync();
            var count = totalAgentSupervisors.Count();
            input.AgentSupervisorCode = "SG-AS-" + (count + 1);
            var newEntity = ObjectMapper.Map<AgentSupervisorInputDto, AgentSupervisor>(input);

            var agentSupervisor = await _agentSupervisorRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<AgentSupervisor, AgentSupervisorDto>(agentSupervisor);
        }

        public async Task<List<AgentSupervisorDto>> GetListAsync()
        {
            //var agentSupervisors = await _agentSupervisorRepository.GetListAsync();
            List<AgentSupervisorDto>? result = null;
            var allsupervisorwithDetails = await _agentSupervisorRepository.WithDetailsAsync(s=>s.AgentMaster);
            //var list = allsupervisorwithDetails.ToList();
            
            if (!allsupervisorwithDetails.Any())
            {
                return result;
            }
            result = new List<AgentSupervisorDto>();
            foreach(var item in allsupervisorwithDetails)
            {
                result.Add(new AgentSupervisorDto()
                {
                    AgentMasterId = item.AgentMasterId,
                    AgentMasterName = item.AgentMasterId > 0 ? item.AgentMaster.AgentMasterOrgName:""
                    //SupervisorName = item.SupervisorName,
                    //AgentSupervisorOrgName = item.AgentSupervisorOrgName,
                    //AgentSupervisorCode = item.AgentSupervisorCode,
                    //SupervisorIdentityNumber = item.SupervisorIdentityNumber,
                    //SupervisorMobileNo = item.SupervisorMobileNo,
                    //Address = item.Address,
                    //City = item.City,
                    //ZipCode = item.ZipCode,
                    //Country = item.Country,
                    //PhoneNo = item.PhoneNo,
                    //Email = item.Email,
                    //EmergencyContact = item.EmergencyContact,
                    //AgentSupervisorDocNumber = item.AgentSupervisorDocNumber,
                    //AgentSupervisorDocExpireDate = item.AgentSupervisorDocExpireDate,
                    //IsActive = item.IsActive,
                    //IsDeleted = item.IsDeleted,
                }); 
            }
            return result;
            //return ObjectMapper.Map<List<AgentSupervisor>, List<AgentSupervisorDto>>(agentSupervisors);
        }
        //public async Task<AgentProfileDto> CreateAsync(AgentProfileInputDto input)
        //{
        //    var newEntity = ObjectMapper.Map<AgentProfileInputDto, AgentProfile>(input);

        //    var agentProfile = await _agentProfileRepository.InsertAsync(newEntity);

        //    //await _unitOfWorkManager.Current.SaveChangesAsync();

        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(agentProfile);
        //}

        //public async Task<AgentProfileDto> GetAsync(int id)
        //{
        //    var item = await _agentProfileRepository.GetAsync(x => x.Id == id);

        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);

        //    //var item = await _agentProfileRepository.WithDetailsAsync();
        //    //var profile = item.FirstOrDefault(item => item.Id == id);
        //    //var result = profile != null ? ObjectMapper.Map<AgentProfile, AgentProfileDto>(profile) : null;

        //    //return result;
        //}

        //public async Task<AgentProfileDto> GetByUserNameAsync(string userName)
        //{
        //    var item = await _agentProfileRepository.GetAsync(x => x.MobileNo == userName);

        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        //}
        //public async Task<List<AgentProfileDto>> GetListAsync()
        //{
        //    var profiles = await _agentProfileRepository.GetListAsync();
        //    return ObjectMapper.Map<List<AgentProfile>, List<AgentProfileDto>>(profiles);
        //}
        //public async Task<AgentProfileDto> GetByUserIdAsync(Guid userId)
        //{
        //    var item = await _agentProfileRepository.GetAsync(x => x.UserId == userId);
        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        //}

        //public async Task<AgentProfileDto> UpdateAsync(AgentProfileInputDto input)
        //{
        //    var updateItem = ObjectMapper.Map<AgentProfileInputDto, AgentProfile>(input);

        //    var item = await _agentProfileRepository.UpdateAsync(updateItem);
        //    await _unitOfWorkManager.Current.SaveChangesAsync();                        
        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        //}

        //public async Task<AgentProfileDto> GetlByUserNameAsync(string userName)
        //{
        //    var item = await _agentProfileRepository.GetAsync(x => x.MobileNo == userName);

        //    return ObjectMapper.Map<AgentProfile, AgentProfileDto>(item);
        //}

    }
}
