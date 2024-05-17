﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.MockData
{
    public class MockRoleClaims
    {
        private static List<IdentityRoleClaim<string>> _roleClaimsList = new List<IdentityRoleClaim<string>>()
        {
            // Claims for the Admin role
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Admin").Id,
                ClaimType = "Permission",
                ClaimValue = "CRUD_All_Rabbits"
            },

            // Claims for the Moderator role
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Moderator").Id,
                ClaimType = "Permission",
                ClaimValue = "CRUD_All_Rabbits"
            },

            // Claims for the Breeder role
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Breeder").Id,
                ClaimType = "Permission",
                ClaimValue = "Read_Own_Rabbits"
            },
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Breeder").Id,
                ClaimType = "Permission",
                ClaimValue = "Read_Public_Rabbits"
            },
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Breeder").Id,
                ClaimType = "Permission",
                ClaimValue = "Create_Own_Rabbits"
            },
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Breeder").Id,
                ClaimType = "Permission",
                ClaimValue = "Update_Own_Rabbits"
            },
            new IdentityRoleClaim<string>
            {
                RoleId = MockRoles.GetMockRoles().First(r => r.Name == "Breeder").Id,
                ClaimType = "Permission",
                ClaimValue = "Delete_Own_Rabbits"
            }
        };

        public static List<IdentityRoleClaim<string>> GetMockRoleClaims()
        {
            return _roleClaimsList;
        }
    }
}
