﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_ReceivedDTO
    {
        public int Id { get; init; }
        public RequestStatus Status { get; init; }

        [DataType(DataType.Date)]
        public DateOnly? DateAccepted { get; init; }

        public string? Rabbit_EarCombId { get; init; }
        public string? Rabbit_NickName { get; init; }

        public string? Issuer_BreederRegNo { get; init; }
        public string? Issuer_FirstName { get; init; }

        public int? Price { get; init; }
        public string? SaleConditions { get; init; }
    }
}
