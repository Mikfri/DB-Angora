﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.RabbitService;
using DB_AngoraLib.Services.UserService;
using DB_AngoraLib.Services.ValidationService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraMST.Services_InMemTest
{
    [TestClass]
    public class UserService_MST
    {
        //private RabbitService _rabbitService;
        private UserService _userService;
        private DB_AngoraContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DB_AngoraContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new DB_AngoraContext(options);
            _context.Database.EnsureCreated();

            // todo: få mine mockusers og mockrabbits til at virke i mine MST Services efter korregering af DB_AngoraContext

            // Add mock data to in-memory database
            //var mockUsers = MockUsers.GetMockUsers();
            //_context.Users.AddRange(mockUsers);
            var mockRabbits = MockRabbits.GetMockRabbits();
            _context.Rabbits.AddRange(mockRabbits);
            _context.SaveChanges();

            var userRepository = new GRepository<User>(_context);
            _userService = new UserService(userRepository);

            var rabbitRepository = new GRepository<Rabbit>(_context);
            var validatorService = new RabbitValidator();
            //_rabbitService = new RabbitService(rabbitRepository, _userService, validatorService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetCurrentUsersRabbitCollection_WithoutProperties_TEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var userKeyDto = new User_KeyDTO { BreederRegNo = currentUser.Id };

            // Act
            var result = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto, null, null, null, null, null, null, null, null, null, null);

            // Debug: Print the names of the returned rabbits
            foreach (var rabbit in result)
            {
                Console.WriteLine(rabbit.NickName);
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count); // Assuming the first user has 5 rabbits
        }

        [TestMethod]
        public async Task GetCurrentUsersRabbitCollection_WithPropertiesTEST()
        {
            // Arrange
            var currentUser = _context.Users.First();
            var userKeyDto = new User_KeyDTO { BreederRegNo = currentUser.Id };
            var race = Race.Angora;
            var color = Color.Blå;
            var gender = Gender.Hun;
            var isPublic = IsPublic.No;
            var rightEarId = "5095";
            var leftEarId = "002";
            var nickName = "Sov";
            var isJuvenile = (bool?)null;
            var dateOfBirth = (DateOnly?)null;
            var dateOfDeath = (DateOnly?)null;

            // Act
            var result = await _userService.GetCurrentUsersRabbitCollection_ByProperties(userKeyDto, rightEarId, leftEarId, nickName, race, color, gender, isPublic, isJuvenile, dateOfBirth, dateOfDeath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var rabbit = result.First();
            Assert.AreEqual(rightEarId, rabbit.RightEarId);
            Assert.AreEqual(leftEarId, rabbit.LeftEarId);
            Assert.AreEqual(nickName, rabbit.NickName);
            Assert.AreEqual(race, rabbit.Race);
            Assert.AreEqual(color, rabbit.Color);
            Assert.AreEqual(gender, rabbit.Gender);
            //Assert.AreEqual(isPublic, rabbit.IsPublic);
        }
    }
}
