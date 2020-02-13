using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;

namespace OpenPoker.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IServer _server;
        public async Task JoinRoom(string roomId)
        {
            var room = _server.rooms.Find( p=> p.id == Int32.Parse(roomId));
            NetworkPlayer player = (NetworkPlayer)room.game.players.Find(p => {
                if (p is NetworkPlayer)
                    if (((NetworkPlayer)p).IsDisconnected)
                        return true;
                return false;
            });
            if (room.game.players.Count == 6 && player == null)
                await _server.Reject(Clients.Caller);
            else
            {
                lock (room.game.players)
                {
                    Context.Items["roomId"] = roomId;
                    if (player != null)
                    {
                        player.ConnectionId = Context.ConnectionId;
                        player.IsDisconnected = false;
                    }
                    else
                    {
                        IPlayer p = new NetworkPlayer(Context.ConnectionId);
                        p.bet = -1;
                        room.game.players.Add(p);
                    }
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
                await _server.SendUpdateData(Clients.Caller, Int32.Parse(roomId));
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string roomId =  Context.Items["roomId"] as string;
            if (roomId == null)
                await base.OnDisconnectedAsync(exception);
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            lock (room.game.players)
            {
                var player = room.game.players.Find(p => {
                    if (p is NetworkPlayer)
                        if (((NetworkPlayer)p).ConnectionId == Context.ConnectionId)
                            return true;
                    return false;
                });
                player.IsDisconnected = true;
                player.bet = -1;
            }
            await _server.SendUpdateData(Clients.Others, Int32.Parse(roomId));
            await base.OnDisconnectedAsync(exception);
        }
        public RoomHub(IServer server)
        {
            _server = server;
        }
    }
}
