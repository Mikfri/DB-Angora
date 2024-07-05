﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record TransferRequest_PreviewDTO
    {
        public int Id { get; init; }
        public RequestStatus Status { get; init; }
        public DateOnly? DateAccepted { get; init; }

        public string EarCombId { get; init; }
        public string Issuer_BreederRegNo { get; init; }
        public string Recipent_BreederRegNo { get; init; }

        public int? Price { get; init; }
        public string? SaleConditions { get; init; }
    }
}
