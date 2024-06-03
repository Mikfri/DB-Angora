﻿using DB_AngoraLib.MockData;
using DB_AngoraLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.EF_DbContext
{
    public class DB_AngoraContext : IdentityDbContext<User>
    {
        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region IdentityUser PK setup - syntaks
            ////------------------- PK SETUP -------------------
            //// Irellevant, da IdentityUser allerede selv sætter disse op
            //// Configure primary key for IdentityUser
            //modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => p.Id);
            //modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
            //modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            //modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            //modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(p => p.Id);
            #endregion

            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Add unique constraint for User's BreederRegNo
            modelBuilder.Entity<User>()
                .HasIndex(u => u.BreederRegNo)
                .IsUnique();

            // Konfigurer primær nøgle for Rabbit
            modelBuilder.Entity<Rabbit>()
                    //.HasKey(r => r.Id);
                    //.HasKey(r => new { r.RightEarId, r.LeftEarId });
                    .HasKey(r => r.EarCombId);

            //// Add unique constraint for Rabbit: Forbedrer ydeevnen ved DB søgning da de nu er indexerede
            //modelBuilder.Entity<Rabbit>()
            //    .HasIndex(r => new { r.RightEarId, r.LeftEarId })
            //    .IsUnique();

            //------------------- FK SETUP -------------------



            // Configure Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.User)        // En Rabbit har en User
                .WithMany(u => u.Rabbits)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId) // En Rabbit har en OwnerId
                .IsRequired(false);

            // Configure Foreign Key for Rabbit -> Rabbit (Father)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Father)
                .WithMany()
                .HasForeignKey(r => r.Father_EarCombId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure Foreign Key for Rabbit -> Rabbit (Mother)
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.Mother)
                .WithMany()
                .HasForeignKey(r => r.Mother_EarCombId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete



            //modelBuilder.Entity<Rabbit>()
            //    .HasOne(r => r.Father)
            //    .WithMany()
            //    .HasForeignKey(r => r.FatherId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            //// Configure Foreign Key for Rabbit -> Rabbit (Mother)
            //    modelBuilder.Entity<Rabbit>()
            //        .HasOne(r => r.Mother)
            //        .WithMany()
            //        .HasForeignKey(r => r.MotherId)
            //        .IsRequired(false)
            //        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


            //modelBuilder.Entity<Rating>()
            //    .HasOne(r => r.Rabbit)      // En Rating har en Rabbit
            //    .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
            //    .HasForeignKey(r => r.Id); // Opdateret til at bruge den nye Id property



        }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
