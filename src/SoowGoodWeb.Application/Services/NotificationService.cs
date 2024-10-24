﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SoowGoodWeb;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;

namespace SoowGoodWeb.Services
{
    public class NotificationService : SoowGoodWebAppService, INotificationService
    {
        //private readonly IIdentityUserRepository _identityUserRepository;
        //private readonly ILookupNormalizer _lookupNormalizer;
        //private readonly IDistributedEventBus _distributedEventBus;

        private readonly IRepository<Notification> _notificationRepository;

        //public readonly IDoctorProfileService _doctorProfileService;

        public NotificationService(IRepository<Notification> notificationRepository)//, ILookupNormalizer lookupNormalizer, IDistributedEventBus distributedEventBus, IDoctorProfileService doctorProfileService)
        {
            _notificationRepository = notificationRepository;
            //_lookupNormalizer = lookupNormalizer;
            //_distributedEventBus = distributedEventBus;
            //_doctorProfileService = doctorProfileService;
        }

        //public async Task SendNotificationAsync(SendNotificationInputDto input)
        //{
        //    //var targetId = (await _identityUserRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName(input.TargetUserName))).Id;
        //    var targetId = (await _identityUserRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName(input.TargetUserName))).Id;

        //    await _distributedEventBus.PublishAsync(new ReceivedNotificationDto(targetId, CurrentUser.UserName, input.Message));
        //}

        public async Task<NotificationDto> GetAsync(int id)
        {
            var item = await _notificationRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<Notification, NotificationDto>(item);
        }
        public async Task<List<NotificationDto>> GetListAsync()
        {
            var notifications = await _notificationRepository.GetListAsync();
            var notificationlist = notifications.OrderByDescending(x => x.Id).ToList();
            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notificationlist);
        }
        public async Task<List<NotificationDto>> GetListByUserIdAsync(long? userId, string? role)
        {
            var findNotification = new List<Notification>();
            try
            {
                var notifications = await _notificationRepository.WithDetailsAsync();
                
                if (userId>0 && !string.IsNullOrEmpty(role))
                {
                    findNotification = notifications.Where(c => c.NotifyToEntityId == userId && c.NotifyToRole==role).OrderByDescending(x => x.Id).ToList();
                }
                //else if (role == "patient" || role == "agent")
                //{
                //    findNotification = notifications.Where(c => c.CreatorEntityId == userId).OrderByDescending(x => x.Id).ToList();
                //}
                else
                {
                    findNotification = notifications.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception wx)
            {
                return null;
            }
            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(findNotification);
        }
        public async Task<int> GetCountByUserId(long userId, string? role)
        {
            var notifications = await _notificationRepository.GetListAsync();

            if (role == "doctor")
            {
                notifications = notifications.Where(c => c.NotifyToEntityId == userId || c.CreatorEntityId == userId).OrderByDescending(x => x.Id).ToList();
            }
            else if (role == "patient" || role == "agent")
            {
                notifications = notifications.Where(c => c.CreatorEntityId == userId || c.CreatorEntityId == userId).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                notifications = notifications.OrderByDescending(x => x.Id).ToList();
            }
            return notifications.Count;
        }
        public async Task<int> GetCount()
        {
            var notifications = await _notificationRepository.GetListAsync();
            return notifications.Count;
        }

        public async Task<int> GetByUserIdCount(long? userId, string? role)
        {
            int count = 0;
            if (role == "doctor")
            {
                var notifications = await _notificationRepository.GetListAsync(n => n.NotifyToEntityId == userId);
                count = notifications.Count;
            }
            else
            {
                var notifications = await _notificationRepository.GetListAsync(n => n.CreatorEntityId == userId);
                count = notifications.Count;
            }
            return count;
        }
    }
}
