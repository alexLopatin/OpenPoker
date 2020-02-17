using OpenPoker.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.Models
{
    public class RoomsList
    {
        public IEnumerable<GameRoom> Rooms { get; set; }
        public int CountOfPages { get; set; }
        public RoomsList(IEnumerable<GameRoom> rooms, int countOfPages)
        {
            CountOfPages = countOfPages;
            Rooms = rooms;
        }
    }
}
