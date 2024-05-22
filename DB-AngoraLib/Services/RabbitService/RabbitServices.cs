﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.HelperService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;


namespace DB_AngoraLib.Services.RabbitService
{
    //using IRabbitRepository = IGRepository<Rabbit>;
    //using RabbitList = List<Rabbit>;

    public class RabbitServices : IRabbitService
    {
        private readonly IGRepository<Rabbit> _dbRepository;
        private readonly IUserService _userService;
        private readonly RabbitValidator _validatorService;

        public RabbitServices(IGRepository<Rabbit> dbRepository, IUserService userService, RabbitValidator validatorService)
        {
            _dbRepository = dbRepository;
            _userService = userService;
            _validatorService = validatorService;
        }

        public RabbitServices() { }
                

        //---------------------: GET METODER
        //-------: ADMIN METODER

        public async Task<List<Rabbit>> GetAllRabbitsAsync()     // reeeally though? Skal vi bruge denne metode, udover test?
        {
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.ToList();
        }

        public async Task<List<Rabbit>> GetAllRabbits_ByBreederRegAsync(string breederRegNo)
        {
            var user = await _userService.GetUserByBreederRegNoAsync(breederRegNo);
            if (user == null)
            {
                return null;
            }
            var rabbits = await _dbRepository.GetAllObjectsAsync();
            return rabbits.Where(rabbit => rabbit.OwnerId == user.Id).ToList();
        }


        //-------: GET BY EAR TAGs METODER
        /// <summary>
        /// Finder en Rabbit ud fra dens øremærkerne kombinationen
        /// </summary>
        /// <param name="rightEarId"></param>
        /// <param name="leftEarId"></param>
        /// <returns></returns>
        public async Task<Rabbit> GetRabbit_ByEarTagsAsync(string rightEarId, string leftEarId)
        {
            Expression<Func<Rabbit, bool>> filter = r => r.RightEarId == rightEarId && r.LeftEarId == leftEarId;

            //Console.WriteLine($"Checking for existing Rabbit with ear tags: RightEarId: {rightEarId}, LeftEarId: {leftEarId}");

            var rabbit = await _dbRepository.GetObjectAsync(filter);

            //if (rabbit != null)
            //{
            //    Console.WriteLine($"A Rabbit with this ear-tag already exists!\nRabbitName: {rabbit.NickName}, OwnerId: {rabbit.OwnerId}, OwnerName: {rabbit.User.FirstName} {rabbit.User.LastName}");
            //}
            //else
            //{
            //    Console.WriteLine("No rabbit found with the given ear tags.");
            //}

            return rabbit;
        }

        //---------------------: ADD

        /// <summary>
        /// Tilføjer en Rabbit til den nuværende bruger og tilføjer den til Collectionen
        /// </summary>
        /// <param name="userId">GUID fra brugeren</param>
        /// <param name="newRabbitDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Rabbit_ProfileDTO> AddRabbit_ToMyCollectionAsync(string currentUser, Rabbit_CreateDTO newRabbitDto)
        {
            //Console.WriteLine($"Trying to add rabbit with RightEarId: {newRabbitDto.RightEarId}, LeftEarId: {newRabbitDto.LeftEarId}");

            // Convert RabbitDto to Rabbit
            var newRabbit = new Rabbit
            {
                RightEarId = newRabbitDto.RightEarId,
                LeftEarId = newRabbitDto.LeftEarId,
                NickName = newRabbitDto.NickName,
                Race = newRabbitDto.Race,
                Color = newRabbitDto.Color,
                DateOfBirth = newRabbitDto.DateOfBirth,
                DateOfDeath = newRabbitDto.DateOfDeath,
                //IsJuvenile = newRabbitDto.IsJuvenile, // TODO: Kigg på denne opsætning
                Gender = newRabbitDto.Gender,
                IsPublic = newRabbitDto.IsPublic
                // ... evt flere..
            };

            _validatorService.ValidateRabbit(newRabbit);

            var existingRabbit = await GetRabbit_ByEarTagsAsync(newRabbit.RightEarId, newRabbit.LeftEarId);
            if (existingRabbit != null)
            {
                throw new InvalidOperationException("En kanin med samme øremærke kombination eksistere allerede");
            }

            var thisUser = await _userService.GetUserByIdAsync(currentUser);

            if (thisUser == null)
            {
                throw new InvalidOperationException("No user found with the given GUID");
            }

            newRabbit.OwnerId = thisUser.Id;
            await _dbRepository.AddObjectAsync(newRabbit);

            // Create a new RabbitDTO and copy properties from newRabbit
            var newRabbitDtoToReturn = new Rabbit_ProfileDTO
            {
                RightEarId = newRabbit.RightEarId,
                LeftEarId = newRabbit.LeftEarId,
                NickName = newRabbit.NickName,
                Race = newRabbit.Race,
                Color = newRabbit.Color,
                DateOfBirth = newRabbit.DateOfBirth,
                DateOfDeath = newRabbit.DateOfDeath,
                Gender = newRabbit.Gender,
                IsPublic = newRabbit.IsPublic
            };

            return newRabbitDtoToReturn;
        }


        //---------------------: UPDATE
        public async Task<Rabbit_ProfileDTO> UpdateRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, Rabbit_UpdateDTO updatedRabbit, IList<Claim> userClaims)
        {
            var hasPermissionToUpdateOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "CRUD_My_Rabbits");
            var hasPermissionToUpdateAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "CRUD_All_Rabbits");

            var rabbitToUpdate = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            if (rabbitToUpdate == null)
            {
                return null; // No rabbit found with the given ear tags, so we return null
            }           

            if (hasPermissionToUpdateAll || (hasPermissionToUpdateOwn && currentUser.Id == rabbitToUpdate.OwnerId))
            {
                // Copy all non-null properties from updatedRabbit to rabbitToUpdate
                HelperServices.CopyPropertiesTo(updatedRabbit, rabbitToUpdate);

                _validatorService.ValidateRabbit(rabbitToUpdate);
                await _dbRepository.UpdateObjectAsync(rabbitToUpdate);
            }

            // Create a new Rabbit_ProfileDTO and copy properties from rabbitToUpdate
            var updatedRabbitDTO = new Rabbit_ProfileDTO();
            HelperServices.CopyPropertiesTo(rabbitToUpdate, updatedRabbitDTO);

            return updatedRabbitDTO; // Return the updated rabbit as a Rabbit_ProfileDTO
        }



        //---------------------: DELETE
        public async Task DeleteRabbit_RBAC_Async(User currentUser, string rightEarId, string leftEarId, IList<Claim> userClaims)
        {
            var hasPermissionToDeleteOwn = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "CRUD_My_Rabbits");
            var hasPermissionToDeleteAll = userClaims.Any(c => c.Type == "RolePermission" && c.Value == "CRUD_All_Rabbits");

            var rabbitToDelete = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            if (rabbitToDelete == null)
            {
                throw new InvalidOperationException("Ingen kanin med angivede ørermærker eksistere");
            }

            if (!hasPermissionToDeleteAll && (!hasPermissionToDeleteOwn || currentUser.Id != rabbitToDelete.OwnerId))
            {
                throw new InvalidOperationException("Du har ikke tilladelse til denne handling");
            }

            await _dbRepository.DeleteObjectAsync(rabbitToDelete);
        }


        //---------------------: TRANSFER
        public async Task RequestRabbitTransfer(string rightEarId, string leftEarId, string breederRegNo)
        {
            // Check if the new owner exists in the database
            var newOwner = await _userService.GetUserByBreederRegNoAsync(breederRegNo);
            if (newOwner == null)
            {
                throw new Exception("New owner not found");
            }

            // Find the rabbit to transfer
            var rabbit = await GetRabbit_ByEarTagsAsync(rightEarId, leftEarId);
            if (rabbit == null)
            {
                throw new Exception("Rabbit not found");
            }

            // Create a new transfer request
            var transferRequest = new RabbitTransferRequest
            {
                RabbitRightEarId = rightEarId,
                RabbitLeftEarId = leftEarId,
                CurrentOwnerId = rabbit.OwnerId,
                NewOwnerId = breederRegNo,
                IsAccepted = false
            };

            // Save the transfer request to the database
            //await _transferRequestRepository.AddObjectAsync(transferRequest);
        }
                    
    }
}
