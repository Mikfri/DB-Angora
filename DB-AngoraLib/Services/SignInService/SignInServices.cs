﻿using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace DB_AngoraLib.Services.SigninService
{
    public class SigninServices : ISigninService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SigninServices(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //-----------------: ALMINDLIG LOGIN :-----------------

        /// <summary>
        /// IdentityUser SignInManager benyttes.. 
        /// En Users RoleClaims opdateres, når brugeren logger ind.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        public async Task<SignInResult> LoginAsync(string userName, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(userName);
                var roles = await _userManager.GetRolesAsync(user);

                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    var userClaims = await _userManager.GetClaimsAsync(user);

                    // Remove any old role claims from the user
                    foreach (var claim in userClaims)
                    {
                        if (roleClaims.Any(rc => rc.Type == claim.Type && rc.Value == claim.Value))
                        {
                            await _userManager.RemoveClaimAsync(user, claim);
                        }
                    }

                    // Add the updated role claims to the user
                    foreach (var claim in roleClaims)
                    {
                        await _userManager.AddClaimAsync(user, claim);
                    }
                }
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        //-----------------: EKSTERN LOGIN :-----------------


        /// <summary>
        /// Henter info såsom loginProvider og providerKey, som kan bruges til at logge brugeren ind.
        /// Metoden kan testes via en mockup - derfor pakker vi signInManageren ind i en service.
        /// </summary>
        /// <returns></returns>
        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }

        /// <summary>
        /// Forsøger at logge brugeren ind ved hjælp af den eksterne login information.
        /// Metoden kan testes via en mockup - derfor pakker vi signInManageren ind i en service.
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }


        public async Task<string> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Replace this with your actual error handling code
                throw new Exception("Error: No external login info available.");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                var user = new User { UserName = info.Principal.FindFirstValue(ClaimTypes.Email), Email = info.Principal.FindFirstValue(ClaimTypes.Email) };
                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    createResult = await _userManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
            }

            // Replace this with your actual error handling code
            throw new Exception("Error: Unable to create user account.");
        }


        private string RedirectToLocal(string returnUrl)
        {
            // Replace this with the URL of your home page
            string defaultUrl = "/home";

            // Check if the URL is local. If it's not, return the default URL.
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
            {
                return returnUrl;
            }
            else
            {
                return defaultUrl;
            }
        }
    }
}