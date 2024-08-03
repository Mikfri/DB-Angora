﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record Rabbit_FilteredRequestDTO
    {
        public bool? OnlyDeceased { get; init; }

        public string? RightEarId { get; init; }
        public string? LeftEarId { get; init; }
        public string? NickName { get; init; }
        public Race? Race { get; init; }
        public Color? Color { get; init; }
        public Gender? Gender { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? FromDateOfBirth { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? FromDateOfDeath { get; init; }

        public string? FatherId_Placeholder { get; init; }
        public string? MotherId_Placeholder { get; init; }
        public bool? IsJuvenile { get; init; }
        public bool? ApprovedRaceColorCombination { get; init; }
        public IsPublic? ForSale { get; init; }
        public IsPublic? ForBreeding { get; init; }
    }
}
