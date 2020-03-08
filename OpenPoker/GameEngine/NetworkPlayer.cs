using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.Models;

namespace OpenPoker.GameEngine
{
    public class NetworkPlayer : IPlayer
    {
        public string Name { get
            {
                return User.UserName;
            } }
        public int Id { get; private set; }
        private readonly IServer _server;
        public User User { get; set; }
        public NetworkPlayer(IServer server, string connectionId, int id, User user)
        {
            _server = server;
            ConnectionId = connectionId;
            Id = id;
            User = user;
        }
        public string ConnectionId {get;set;}
        public bool IsDisconnected { get; set; } = false;
        public bool IsPlaying { get; set; } = false;
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
