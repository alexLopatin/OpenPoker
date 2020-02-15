using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class Action
    {
        public bool IsPersonal 
        {
            get
            {
                return (ConnectionId != null);
            }
         }
        //who receives
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public Action(string name, string connectionId = null)
        {
            Name = name;
            ConnectionId = connectionId;
        }
    }
}
