using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class MatchMaker
    {
        private readonly IServer _server;
        public MatchMaker(IServer server)
        {
            _server = server;
        }
        public int CreateOrFindRoom()
        {
            int lastNull = -1;
            lock(_server.rooms)
            {
                for (int i = 1; i <= _server.rooms.Max(kvp =>kvp.Key); i++)
                {
                    if (!_server.rooms.ContainsKey(i))
                        lastNull = i;
                    else if (_server.rooms[i].CountOfPlayers() < 6)
                        return i;
                }
                if (lastNull == -1)
                    lastNull = _server.rooms.Max(kvp => kvp.Key) + 1;
                _server.CreateGame(new GameRoom("Room #" + lastNull.ToString(), lastNull));
                return lastNull;
            }
        }
    }
}
