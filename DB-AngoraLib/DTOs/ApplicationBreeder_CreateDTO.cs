﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.DTOs
{
    public record ApplicationBreeder_CreateDTO 
    {
        public string RequestedBreederRegNo { get; init; }
        public string DocumentationPath { get; init; }
    }
}
