﻿using DB_AngoraLib.DTOs;
using DB_AngoraLib.Models;
using DB_AngoraLib.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.BreederService
{
    public class BreederServices : IBreederService
    {
        private readonly IGRepository<Breeder> _breederRepository;

        public BreederServices(IGRepository<Breeder> breederRepository)
        {
            _breederRepository = breederRepository;
        }

        //----------------------------: GET :----------------------------
        public async Task<List<Breeder>> GetAll_Breeders()
        {
            return (await _breederRepository.GetAllObjectsAsync()).ToList();
        }

        public async Task<Breeder?> Get_BreederById(string userId)
        {
            return await _breederRepository.GetObject_ByStringKEYAsync(userId);
        }

        public async Task<Breeder?> Get_BreederByBreederRegNo(string breederRegNo)
        {
            return await _breederRepository.GetDbSet()
                .FirstOrDefaultAsync(u => u.BreederRegNo == breederRegNo);
        }


        private async Task<Breeder?> Get_BreederByBreederRegNo_IncludingCollections(string breederRegNo)
        {
            return await _breederRepository.GetDbSet()
                .Include(b => b.RabbitTransfers_Issued)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .Include(b => b.RabbitTransfers_Received)
                    .ThenInclude(rt => rt.Status == TransferStatus.Pending)
                .FirstOrDefaultAsync(b => b.BreederRegNo == breederRegNo);
        }

        //-----------: BREEDER ICOLLECTIONs
        public async Task<List<Rabbit_PreviewDTO>> GetAll_RabbitsOwned_Filtered(
          string userId, Rabbit_FilteredRequestDTO filter)
        {

            var query = _breederRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOwner) // Inkluder UserOwner relationen
                .Include(u => u.RabbitsOwned)
                    .ThenInclude(rabbit => rabbit.UserOrigin) // Inkluder UserOrigin relationen
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitsOwned)
                .AsQueryable();



            // Filtrer baseret på død/levende status
            if (filter.OnlyDeceased.HasValue)
            {
                if (filter.OnlyDeceased.Value)
                {
                    // Vis kun døde kaniner
                    query = query.Where(r => r.DateOfDeath != null);
                }
                else
                {
                    // Vis kun levende kaniner
                    query = query.Where(r => r.DateOfDeath == null);
                }
            }

            // Anvend filtrering baseret på Rabbit_FilteredRequestDTO
            if (filter.RightEarId != null)
                query = query.Where(r => EF.Functions.Like(r.RightEarId, $"%{filter.RightEarId}%"));
            if (filter.LeftEarId != null)
                query = query.Where(r => EF.Functions.Like(r.LeftEarId, $"%{filter.LeftEarId}%"));
            if (filter.NickName != null)
                query = query.Where(r => EF.Functions.Like(r.NickName, $"%{filter.NickName}%"));
            if (filter.Race.HasValue)
                query = query.Where(r => r.Race == filter.Race.Value);
            if (filter.Color.HasValue)
                query = query.Where(r => r.Color == filter.Color.Value);
            if (filter.Gender.HasValue)
                query = query.Where(r => r.Gender == filter.Gender.Value);
            if (filter.FromDateOfBirth.HasValue)
                query = query.Where(r => r.DateOfBirth > filter.FromDateOfBirth.Value);
            if (filter.FromDateOfDeath.HasValue)
                query = query.Where(r => r.DateOfDeath.HasValue && r.DateOfDeath > filter.FromDateOfDeath.Value);

            // Håndter IsJuvenile
            if (filter.IsJuvenile.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var juvenileStart = today.AddDays(-14 * 7); // Starten af juvenile alderen (14 uger tilbage)
                var juvenileEnd = today.AddDays(-8 * 7); // Slutningen af juvenile alderen (8 uger tilbage)

                if (filter.IsJuvenile.Value)
                {
                    query = query.Where(r => r.DateOfBirth >= juvenileStart && r.DateOfBirth <= juvenileEnd);
                }
                else
                {
                    query = query.Where(r => r.DateOfBirth < juvenileStart || r.DateOfBirth > juvenileEnd);
                }
            }

            if (filter.ApprovedRaceColorCombination.HasValue)
                query = query.Where(r => r.ApprovedRaceColorCombination == filter.ApprovedRaceColorCombination.Value);

            if (filter.ForSale.HasValue)
                query = query.Where(r => r.ForSale == filter.ForSale.Value);
            if (filter.ForBreeding.HasValue)
                query = query.Where(r => r.ForBreeding == filter.ForBreeding.Value);

            // Filtrering baseret på FatherId_Placeholder og MotherId_Placeholder
            if (filter.FatherId_Placeholder != null)
                query = query.Where(r => EF.Functions.Like(r.FatherId_Placeholder, $"%{filter.FatherId_Placeholder}%"));
            if (filter.MotherId_Placeholder != null)
                query = query.Where(r => EF.Functions.Like(r.MotherId_Placeholder, $"%{filter.MotherId_Placeholder}%"));

            var queryRabbitsList = await query.ToListAsync();

            var rabbitPreviewDTOsList = queryRabbitsList.Select(rabbit => new Rabbit_PreviewDTO
            {
                EarCombId = rabbit.EarCombId,
                NickName = rabbit.NickName,
                Race = rabbit.Race,
                Color = rabbit.Color,
                Gender = rabbit.Gender,
                UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
            }).ToList();

            return rabbitPreviewDTOsList;
        }

        public async Task<List<Rabbit_PreviewDTO>> GetAll_Rabbits_FromMyFold(string userId)
        {
            var userWithRabbitsLinked = await _breederRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitsLinked)
                    .ThenInclude(rabbit => rabbit.UserOwner)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (userWithRabbitsLinked == null)
            {
                Console.WriteLine("User not found");
                return new List<Rabbit_PreviewDTO>();
            }

            if (userWithRabbitsLinked.RabbitsLinked.Count < 1)
            {
                Console.WriteLine("No linked rabbits found in collection");
                return new List<Rabbit_PreviewDTO>();
            }

            return userWithRabbitsLinked.RabbitsLinked
                .Select(rabbit => new Rabbit_PreviewDTO
                {
                    EarCombId = rabbit.EarCombId,
                    NickName = rabbit.NickName,
                    Race = rabbit.Race,
                    Color = rabbit.Color,
                    Gender = rabbit.Gender,
                    UserOwner = rabbit.UserOwner != null ? $"{rabbit.UserOwner.FirstName} {rabbit.UserOwner.LastName}" : null,
                    UserOrigin = rabbit.UserOrigin != null ? $"{rabbit.UserOrigin.FirstName} {rabbit.UserOrigin.LastName}" : null,
                })
                .ToList();
        }

        //-----------: TransferRequests
        public async Task<List<TransferRequest_ReceivedDTO>> GetAll_TransferRequests_Received(
            string userId, TransferRequest_ReceivedFilterDTO filter)
        {
            var query = _breederRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitTransfers_Received)
                    .ThenInclude(transfer => transfer.UserIssuer)
                .Include(u => u.RabbitTransfers_Received)
                    .ThenInclude(transfer => transfer.Rabbit)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitTransfers_Received)
                .AsQueryable();

            if (filter.Status.HasValue)
            {
                query = query.Where(transfer => transfer.Status == filter.Status.Value);
            }

            if (filter.Rabbit_EarCombId != null)
                query = query.Where(t => EF.Functions.Like(t.RabbitId, $"%{filter.Rabbit_EarCombId}%"));

            if (filter.Rabbit_NickName != null)
                query = query.Where(t => EF.Functions.Like(t.Rabbit.NickName, $"%{filter.Rabbit_NickName}%"));

            if (filter.Issuer_BreederRegNo != null)
                query = query.Where(t => EF.Functions.Like(t.UserIssuer.BreederRegNo, $"%{filter.Issuer_BreederRegNo}%"));

            if (filter.Issuer_FirstName != null)
                query = query.Where(t => EF.Functions.Like(t.UserIssuer.FirstName, $"%{filter.Issuer_FirstName}%"));

            if (filter.From_DateAccepted.HasValue)
            {
                query = query.Where(transfer => transfer.DateAccepted >= filter.From_DateAccepted.Value);
            }

            var transferRequests = await query.Select(transfer => new TransferRequest_ReceivedDTO
            {
                Id = transfer.Id,
                Status = transfer.Status,
                DateAccepted = transfer.DateAccepted,
                Rabbit_EarCombId = transfer.Rabbit.EarCombId,
                Rabbit_NickName = transfer.Rabbit.NickName,
                Issuer_BreederRegNo = transfer.UserIssuer.BreederRegNo,
                Issuer_FirstName = transfer.UserIssuer.FirstName,
                Price = transfer.Price,
                SaleConditions = transfer.SaleConditions,
            }).ToListAsync();

            return transferRequests;
        }

        public async Task<List<TransferRequest_SentDTO>> GetAll_TransferRequests_Sent(
            string userId, TransferRequest_SentFilterDTO filter)
        {
            var query = _breederRepository.GetDbSet()
                .AsNoTracking()
                .Include(u => u.RabbitTransfers_Issued)
                    .ThenInclude(transfer => transfer.UserIssuer)
                .Include(u => u.RabbitTransfers_Issued)
                    .ThenInclude(transfer => transfer.Rabbit)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.RabbitTransfers_Issued)
                .AsQueryable();


            if (filter.Status.HasValue)
            {
                query = query.Where(transfer => transfer.Status == filter.Status.Value);
            }

            if (filter.Rabbit_EarCombId != null)
                query = query.Where(t => EF.Functions.Like(t.RabbitId, $"%{filter.Rabbit_EarCombId}%"));

            if (filter.Rabbit_NickName != null)
                query = query.Where(t => EF.Functions.Like(t.Rabbit.NickName, $"%{filter.Rabbit_NickName}%"));

            if (filter.Recipent_BreederRegNo != null)
                query = query.Where(t => EF.Functions.Like(t.UserRecipent.BreederRegNo, $"%{filter.Recipent_BreederRegNo}%"));

            if (filter.Recipent_FirstName != null)
                query = query.Where(t => EF.Functions.Like(t.UserRecipent.FirstName, $"%{filter.Recipent_FirstName}%"));

            if (filter.From_DateAccepted.HasValue)
            {
                query = query.Where(transfer => transfer.DateAccepted >= filter.From_DateAccepted.Value);
            }

            // Konverter til TransferRequest_SentDTO
            var result = await query.Select(t => new TransferRequest_SentDTO
            {
                Id = t.Id,
                Status = t.Status,
                DateAccepted = t.DateAccepted,
                Rabbit_EarCombId = t.Rabbit.EarCombId,
                Rabbit_NickName = t.Rabbit.NickName,
                Recipent_BreederRegNo = t.UserRecipent.BreederRegNo,
                Recipent_FirstName = t.UserRecipent.FirstName,
                Price = t.Price,
                SaleConditions = t.SaleConditions
            }).ToListAsync();

            return result;
        }
    }
}
