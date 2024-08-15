﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.AccountService
{
    public interface IAccountService
    {
        // User related methods
        Task<Register_ResponseDTO> Register_BasicUserAsync(Register_CreateBasicUserDTO newUserDto);
        Task<User?> Get_UserByUserNameOrEmail(string userNameOrEmail);
        Task<User?> Get_UserById(string userId);
        Task<List<User>> GetAll_Users();
        Task<User_ProfileDTO> Get_User_Profile(string userId, string userProfileId, IList<Claim> userClaims);

        // Breeder related methods
        Task<List<Breeder>> GetAll_Breeders();
        Task<Breeder> Get_BreederById(string breederId);
        Task<Breeder?> Get_BreederByBreederRegNo(string breederRegNo);
        Task<Breeder?> Get_BreederByBreederRegNo_IncludingCollections(string breederRegNo);

        Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsOwned_Filtered(
          string userId, Rabbit_FilteredRequestDTO filter);

        // Email related methods
        Task Send_EmailConfirm_ToUser(string userId, string token);
        Task Send_PWResetRequest(string userEmail);
        Task Formular_ResetPW(string userId, string token, string newPassword);

        // Account settings related methods
        Task<IdentityResult> User_ChangePassword(User_ChangePasswordDTO userPwConfig);
    }
}
