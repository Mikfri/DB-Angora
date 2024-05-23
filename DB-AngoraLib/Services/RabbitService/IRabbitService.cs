﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.RabbitService
{
    public interface IRabbitService
    {
        Task<List<Rabbit>> GetAllRabbitsAsync();
        Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo);
        Task<Rabbit> GetRabbit_ByEarTagsAsync(string rightEarId, string leftEarId);
        Task<Rabbit_ProfileDTO> GetRabbit_ProfileAsync(string currentUser, string rightEarId, string leftEarId, IList<Claim> userClaims);
        Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollectionAsync(string currentUser, Rabbit_CreateDTO newRabbit);
        Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, Rabbit_UpdateDTO updatedRabbit, IList<Claim> userClaims);
        Task DeleteRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, IList<Claim> userClaims);
    }
}
