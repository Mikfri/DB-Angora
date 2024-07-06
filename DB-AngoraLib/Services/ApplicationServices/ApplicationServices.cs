﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.EF_DbContext;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using DB_AngoraLib.Services.AccountService;
using DB_AngoraLib.Services.RabbitService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.ApplicationServices
{
    public class ApplicationServices : IApplicationService
    {
        private readonly IGRepository<ApplicationBreeder> _dbRepository;
        private readonly IRabbitService _rabbitServices;
        private readonly IAccountService _accountServices;
        private readonly UserManager<User> _userManager;

        public ApplicationServices(IGRepository<ApplicationBreeder> breederAppRepository, IRabbitService rabbitService, IAccountService accountService, UserManager<User> userManager)
        {
            _dbRepository = breederAppRepository;
            _rabbitServices = rabbitService;
            _accountServices = accountService;
            _userManager = userManager;
        }


        //---------------------------------: CREATE/POST :---------------------------------
        //----------------: Create application
        public async Task Apply_ApplicationBreeder(string userId, ApplicationBreeder_CreateDTO applicationDTO)
        {
            // Brug GetUserByIdAsync fra IAccountService til at hente den aktuelt loggede ind brugers oplysninger
            var user = await _accountServices.Get_UserByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (user.BreederRegNo is not null)
            {
                throw new Exception("Du er allerede registreret som avler");
            }

            var existingApplication = await _dbRepository.GetObject_ByFilterAsync(ba => ba.UserApplicantId == userId && ba.Status == RequestStatus.Pending);
            if (existingApplication != null)
            {
                throw new Exception("Du har allerede en afventende ansøgning om optagelse som avler! Vent venligst");
            }

            var application = new ApplicationBreeder
            {
                UserApplicantId = userId,
                RequestedBreederRegNo = applicationDTO.RequestedBreederRegNo,
                DocumentationPath = applicationDTO.DocumentationPath,
                Status = RequestStatus.Pending
            };

            await _dbRepository.AddObjectAsync(application);
        }


        // TODO: Se om den pågældende bruger skal modtage en notifikation/email om afvisning/success
        public async Task Respond_ApplicationBreeder(int applicationId, ApplicationBreeder_ResponseDTO responseDTO)
        {
            var application = await _dbRepository.GetObject_ByIntKEYAsync(applicationId);
            if (application == null) throw new Exception("Ansøgning ikke fundet");

            if (responseDTO.IsApproved)
            {
                application.Status = RequestStatus.Approved;

                var user = await _accountServices.Get_UserByIdAsync(application.UserApplicantId);
                if (user == null) throw new Exception("Bruger ikke fundet");

                var addToRoleResult = await _userManager.AddToRoleAsync(user, "Breeder");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception("Brugerollen 'Breeder' kunne ikke tildeles");
                }

                user.BreederRegNo = application.RequestedBreederRegNo;
                await _rabbitServices.LinkRabbits_ToNewBreederAsync(user.Id, user.BreederRegNo);
            }
            else
            {
                application.Status = RequestStatus.Rejected;
                application.RejectionReason = responseDTO.RejectionReason ?? "Venligst kontakt DB-Angoras service på yyy@yyy.dk, for hjælp";
            }

            await _dbRepository.UpdateObjectAsync(application);
        }



        //---------------------------------: READ/GET :---------------------------------
        //----------------: Get pending applications

        // TODO: Skal returnere en DTO i stedet for modellen
        public async Task<IEnumerable<ApplicationBreeder>> GetAll_ApplicationBreeder_Pending()
        {
            var pendingApplications = await _dbRepository.GetAllObjectsAsync();
            return pendingApplications.Where(a => a.Status == RequestStatus.Pending);
        }

    }
}
