using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SoowGoodWeb;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;

namespace SignalRTieredDemo
{
    public class NotificationService: SoowGoodWebAppService, INotificationAppService
    {
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IDistributedEventBus _distributedEventBus;

        public readonly IDoctorProfileService _doctorProfileService;

        public NotificationService(IIdentityUserRepository identityUserRepository, ILookupNormalizer lookupNormalizer, IDistributedEventBus distributedEventBus, IDoctorProfileService doctorProfileService)
        {
            _identityUserRepository = identityUserRepository;
            _lookupNormalizer = lookupNormalizer;
            _distributedEventBus = distributedEventBus;
            _doctorProfileService = doctorProfileService;
        }

        public async Task SendNotificationAsync(SendNotificationInputDto input)
        {
            //var targetId = (await _identityUserRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName(input.TargetUserName))).Id;
            var targetId = (await _identityUserRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName(input.TargetUserName))).Id;

            await _distributedEventBus.PublishAsync(new ReceivedNotificationDto(targetId, CurrentUser.UserName, input.Message));
        }
    }
}
