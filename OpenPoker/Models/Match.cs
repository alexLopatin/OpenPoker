using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }
        public ICollection<MatchUsers> Users { get; set; } = new List<MatchUsers>();
        public int cash { get; set; }
        public string Winner { get; set; }
        public DateTime Date { get; set; }
    }
}
