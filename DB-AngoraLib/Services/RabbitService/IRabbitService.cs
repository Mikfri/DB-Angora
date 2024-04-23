﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RabbitService
{
    public interface IRabbitService
    {
        Task<List<Rabbit>> GetAllRabbitsAsync();
        Task<Rabbit> GetRabbitByEarTagsAsync(string rightEarId, string leftEarId);
        Task AddRabbit_ToCurrentUserAsync(User currentUser, Rabbit newRabbit);
        Task UpdateRabbitAsync(User currentUser, Rabbit rabbitId);
        Task DeleteRabbitAsync(User currentUser, Rabbit rabbitToDelete);
    }
}
