using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Match> Matches { get; set; }
    }
}
