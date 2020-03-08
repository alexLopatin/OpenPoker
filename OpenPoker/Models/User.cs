﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OpenPoker.Models
{
    public class User : IdentityUser
    {
        public int YearBorn { get; set; }
        public ICollection<MatchUsers> matches { get; set; }
        
    }
}
