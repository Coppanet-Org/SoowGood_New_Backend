﻿using SoowGoodWeb.InputDto;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SoowGoodWeb.Interfaces
{
    public interface IHubClient
    {
        Task BroadcastMessage(string message); //long meassageId
        //Task SendNotificationAsync(SendNotificationInputDto input);
    }
}
