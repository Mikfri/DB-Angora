﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class RefreshToken
    {


        public int Id { get; set; }
        public string UserId { get; set; } // ForeignKey til User
        public User User { get; set; } // Navigationsegenskab

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        public RefreshToken()
        {
            Expires = DateTime.UtcNow.Add(TokenDuration);
        }

        public static TimeSpan TokenDuration => TimeSpan.FromDays(7);

    }
}
