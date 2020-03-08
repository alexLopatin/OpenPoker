using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models.Update
{
    public class EndGameUpdateModel
    {
        public string WinnerId;
        public int Cash;
        public string Final { get; set; }
        public EndGameUpdateModel(string final, string winnerId, int cash)
        {
            Cash = cash;
            Final = final;
            WinnerId = winnerId;
        }
        public EndGameUpdateModel(string final)
        {
            Final = final;
        }

    }
}
