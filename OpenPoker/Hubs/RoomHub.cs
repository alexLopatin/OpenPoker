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
            if(room.game.players.Count == 6)
                await _server.Reject(Clients.Caller);
            else
            {
                lock (room.game.players)
                {
                    Context.Items["roomId"] = roomId;
                    IPlayer p = new NetworkPlayer();
                    p.bet = -1;
                    room.game.players.Add(p);
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, "/room/" + roomId);
                await _server.SendUpdateData(Clients.Caller, Int32.Parse(roomId));
            }
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string roomId =  Context.Items["roomId"] as string;
            if (roomId == null)
                return base.OnDisconnectedAsync(exception);
            var room = _server.rooms.Find(p => p.id == Int32.Parse(roomId));
            room.game.players.RemoveAll(p => { return false; });
            return base.OnDisconnectedAsync(exception);
        }
        public RoomHub(IServer server)
        {
            _server = server;
        }
    }
}
