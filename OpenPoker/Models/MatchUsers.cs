using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models
{
    public class MatchUsers
    {
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
