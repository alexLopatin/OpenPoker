using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace OpenPoker.GameEngine
{
    public class NetworkPlayer : IPlayer
    {
        private readonly IServer _server;
        public NetworkPlayer(IServer server, string connectionId)
        {
            _server = server;
            ConnectionId = connectionId;
        }
        public string ConnectionId {get;set;}
        public bool IsDisconnected { get; set; } = false;
        public List<Card> cards { get; set; } = new List<Card>();
        public int bet { get; set; } = 0;
        TaskCompletionSource<int> tcs = null;
        public void MessageReceived(int bet)
        {
            tcs?.TrySetResult(bet);
        }
        public async Task<int> DoBet(int minBet)
        {
            await _server.SendBetQuery(ConnectionId, minBet);
            tcs = new TaskCompletionSource<int>();
            int nb = await tcs.Task;
            tcs = null;
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
