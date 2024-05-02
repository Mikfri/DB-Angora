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

        public DB_AngoraContext(DbContextOptions<DB_AngoraContext> options) : base(options)
        {
            Users = Set<User>();
            Rabbits = Set<Rabbit>();
            Ratings = Set<Rating>();
            RabbitParents = Set<RabbitParents>();
            Photos = Set<Photo>();
        }
                
        #region ConnectionString
        //public DB_AngoraContext() { }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer(
        //              @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DB-Angora_DB; Integrated Security=True; Connect Timeout=30; Encrypt=False");
        //    }
        //}
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Dette ændrer default navnet på tabellen fra AspNetUsers Id til BreederRegNo
            //modelBuilder.Entity<IdentityUser>(
            //    iu => iu.Property(c => c.Id).HasColumnName("BreederRegNo")
            //    );

            // Configure primary key for User
            modelBuilder.Entity<User>()
                .HasKey(u => u.BreederRegNo);
                        
            // Configure composite key for Rabbit
            modelBuilder.Entity<Rabbit>()
                .HasKey(r => new { r.RightEarId, r.LeftEarId });

            //------------------- FK SETUP -------------------
            // Configure Foreign Key for Rabbit -> User
            modelBuilder.Entity<Rabbit>()
                .HasOne(r => r.User)       // En Rabbit har en User
                .WithMany(u => u.Rabbits)   // En User har mange Rabbits
                .HasForeignKey(r => r.OwnerId) // En Rabbit har en OwnerId
                .IsRequired(false);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Rabbit) // En Rating har en Rabbit
                .WithMany(rb => rb.Ratings) // En Rabbit har mange Ratings
                .HasForeignKey(r => new { r.RightEarId, r.LeftEarId });

            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitMother)      // En RabbitParent har en RabbitMother
                .WithMany(r => r.MotheredChildren)
                .HasForeignKey(rp => new { rp.MotherRightEarId, rp.MotherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RabbitParents>()
                .HasOne(rp => rp.RabbitFather)
                .WithMany(r => r.FatheredChildren)
                .HasForeignKey(rp => new { rp.FatherRightEarId, rp.FatherLeftEarId })
                .OnDelete(DeleteBehavior.NoAction);


            // Tilføj mock data
            //var passwordHasher = new PasswordHasher<User>();
            //var mockUsers = new MockUsers(passwordHasher);

            //var users = mockUsers.GetMockUsers();
            //modelBuilder.Entity<User>().HasData(users);

            //var mockRabbits = MockRabbits.GetMockRabbits();
            //modelBuilder.Entity<Rabbit>().HasData(mockRabbits);

            //base.OnModelCreating(modelBuilder);
        }
        //public DbSet<User> Users { get; set; }
        public DbSet<Rabbit> Rabbits { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<RabbitParents> RabbitParents { get; set; }
        public DbSet<Photo> Photos { get; set; }


    }
}
