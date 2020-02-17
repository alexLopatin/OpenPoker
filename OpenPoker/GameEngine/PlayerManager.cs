using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker.GameEngine
{
    public class PlayerManager
    {
        private readonly IServer _server;
        public void SetPlayerBet(string connectionId, string roomId, int bet)
        {
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            var player = (NetworkPlayer)room.game.players.Find(p => {
                if (p is NetworkPlayer)
                    if (((NetworkPlayer)p).ConnectionId == connectionId)
                        return true;
                return false;
            });
            lock(player.GetPlayerBetTask)
                player.GetPlayerBetTask.MessageReceived(bet);
        }
        public async Task SetPlayerBetAsync(string connectionId, string roomId, int bet)
        {
            await Task.Run(() => { SetPlayerBet(connectionId, roomId, bet); });
        }
        public void SetPlayerDisconnected(string connectionId, string roomId)
        {
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            lock (room.game.players)
            {
                var player = (NetworkPlayer)room.game.players.Find(p => {
                    if (p is NetworkPlayer)
                        if (((NetworkPlayer)p).ConnectionId == connectionId)
                            return true;
                    return false;
                });
                if (player.GetPlayerBetTask != null)
                    if (player.GetPlayerBetTask.IsPending)
                        player.GetPlayerBetTask.MessageReceived(-1);
                player.IsDisconnected = true;
                player.bet = -1;
            }
        }
        public void Kick(int id, string roomId)
        {
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            lock (room.game.players)
            {
                var player =room.game.players.Find(p => {
                        if (p.Id == id)
                            return true;
                    return false;
                });
                //if (player. != null)
                //    if (player.GetPlayerBetTask.IsPending)
                //        player.GetPlayerBetTask.MessageReceived(-1);
                player.IsDisconnected = true;
                player.bet = -1;
            }
        }
        public int AddNewPlayer(string connectionId, string roomId, string playerName)
        {
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            NetworkPlayer player = (NetworkPlayer)room.game.players.Find(p =>
            {
                if (p is NetworkPlayer)
                    if (((NetworkPlayer)p).IsDisconnected)
                        return true;
                return false;
            });
            if (room.game.players.Count == 6 && player == null)
                return -1;
            else
            {
                int newId = 0;
                lock (room.game.players)
                {
                    newId = room.GetNewIdPlayer();
                    if (player != null)
                    {
                        player.ConnectionId = connectionId;
                        newId = player.Id;
                        player.IsDisconnected = false;
                    }
                    else
                    {
                        IPlayer p = new NetworkPlayer(_server, connectionId, newId, playerName);
                        if (room.game.state == Game.GameState.Lobby)
                            p.bet = 0;
                        else
                            p.bet = -1;
                        room.game.players.Add(p);
                        room.game.players.Sort((x, y) => x.Id.CompareTo(y.Id));
                    }
                    return newId;
                }
            }
        }
        public PlayerManager(IServer server)
        {
            _server = server;
        }
    }
}
