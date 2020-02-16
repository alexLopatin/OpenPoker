using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace OpenPoker.GameEngine
{
    public class NetworkPlayer : IPlayer
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        private readonly IServer _server;
        public NetworkPlayer(IServer server, string connectionId, int id, string name)
        {
            _server = server;
            ConnectionId = connectionId;
            Id = id;
            Name = name;
        }
        public string ConnectionId {get;set;}
        public bool IsDisconnected { get; set; } = false;
        public List<Card> cards { get; set; } = new List<Card>();
        public int bet { get; set; } = 0;
        public RequestResponseTask<int> GetPlayerBetTask;
        public async Task<int> DoBet(int minBet)
        {
            GetPlayerBetTask = new RequestResponseTask<int>(_server.SendBetQuery(ConnectionId, minBet));
            int nb = await GetPlayerBetTask.Run();
            if (nb == -1)
            {
                bet = -1;
            }
            else if (nb < minBet)
            {
                Console.WriteLine("Write correct bet.");
                return await DoBet(minBet);
            }
            else
            {
                int res = nb - bet;
                bet = nb;
                return res;
            }
            return 0;
        }
    }
}
