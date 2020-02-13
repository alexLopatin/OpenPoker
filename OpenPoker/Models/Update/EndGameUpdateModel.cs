using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models.Update
{
    public class EndGameUpdateModel
    {
        public string Final { get; set; }
        public EndGameUpdateModel(string final)
        {
            Final = final;
        }
    }
}
