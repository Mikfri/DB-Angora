﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.UserService
{
    public class UserService : IUserService
    {
        //private readonly List<Rabbit> _rabbits = new List<Rabbit>();
        //public IReadOnlyList<Rabbit> Rabbits => _rabbits.AsReadOnly();

        private readonly IGRepository<User> _dbRepository;

        public UserService(IGRepository<User> dbRepository)
        {
            _dbRepository = dbRepository;
        }

        //------------------------- USER METHODS -------------------------


        /// <summary>
        /// Login er tiltænkt for at kunne verificere brugere, der logger ind under en Cookie session,
        /// og benyttes af andre metoder, der kræver brugerens verificering.
        //public async Task<User> Login(UserLoginDTO userLoginDto)
        //{
        //    var user = await _dbRepository.GetObjectAsync(u => u.Email == userLoginDto.Email);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
        //    {
        //        return null;
        //    }

        //    return user;
        //}

        /// <summary>
        /// Benytter <User> classen ICollection<Rabbit>.
        public async Task<List<Rabbit_PreviewDTO>> GetCurrentUsersRabbitCollection(string userId)
        {
            var currentUserCollection = await _dbRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.Rabbits)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (currentUserCollection?.Rabbits == null)
            {
                return new List<Rabbit_PreviewDTO>();
            }

            return currentUserCollection.Rabbits
                .Select(rabbit => new Rabbit_PreviewDTO
                {
                    RightEarId = rabbit.RightEarId,
                    LeftEarId = rabbit.LeftEarId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender
                })
                .ToList();
        }

        public async Task<List<Rabbit_PreviewDTO>> GetFilteredRabbitCollection(string userId, string rightEarId = null, string leftEarId = null, string nickName = null, Race? race = null, Color? color = null, Gender? gender = null)
        {
            var rabbitCollection = await GetCurrentUsersRabbitCollection(userId);

            return rabbitCollection
                .Where(rabbit =>
                       (rightEarId == null || rabbit.RightEarId == rightEarId)
                    && (leftEarId == null || rabbit.LeftEarId == leftEarId)
                    && (nickName == null || rabbit.NickName == nickName)
                    && (race == null || rabbit.Race == race)
                    && (color == null || rabbit.Color == color)
                    && (gender == null || rabbit.Gender == gender))
                .ToList();
        }


        public async Task UpdateUserAsync(User user)
        {
            await _dbRepository.UpdateObjectAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _dbRepository.DeleteObjectAsync(user);
        }

        //-------: ADMIN METHODS ONLY
        public async Task<List<User>> GetAllUsersAsync()
        {
            return (await _dbRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<User> GetUserByBreederRegNoAsync(User_KeyDTO userKeyDto)
        {
            return await _dbRepository.GetObjectAsync(u => u.Id == userKeyDto.BreederRegNo);
        }

        public async Task AddUserAsync(User newUser)
        {
            await _dbRepository.AddObjectAsync(newUser);
        }
    }
}
