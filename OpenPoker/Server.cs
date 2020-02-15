using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;
using OpenPoker.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPoker
{
    public interface IServer
    {
        public List<GameRoom> rooms { get; }
        public void CreateGame(GameRoom room);
        public Task SendUpdateData(IClientProxy caller, int roomId);
        public Task Reject(IClientProxy caller);
        public Task SendBetQuery(string connectionId, int minBet);
        public Task SendSetupData(IClientProxy caller, int id);
    }
    public class Server : IServer
    {
        public List<GameRoom> rooms { get; private set; } = new List<GameRoom>();
        private IHubContext<RoomHub> HubContext { get; set; }
        public void SendPlayerState(object sender, GameUpdateArgs args)
        {
            var room = (GameRoom)sender;
            foreach(KeyValuePair<GameEngine.Action, object> kvp in args.ActionArgumentsPairs)
            {
                if (kvp.Key.IsPersonal)
                {
                    HubContext.Clients.Client(kvp.Key.ConnectionId).SendAsync(kvp.Key.Name, kvp.Value);
                }
                else
                    HubContext.Clients.Group("/room/" + room.id.ToString())
                        .SendAsync(kvp.Key.Name, kvp.Value);
            }
        }

        public void GameRoomClose(object sender, GameUpdateArgs args)
        {
            lock (rooms)
                rooms.Remove(sender as GameRoom);
        }
        public async Task SendUpdateData(IClientProxy caller, int roomId)
        {
            var room = rooms.Find(p => p.id == roomId);
            var args = room.game.GetUpdateData();
            foreach (KeyValuePair<GameEngine.Action, object> kvp in args.ActionArgumentsPairs)
                await caller.SendAsync(kvp.Key.Name, kvp.Value);
        }
        public async Task Reject(IClientProxy caller)
        {
            await caller.SendAsync("Reject", "Room is full!");
        }
        public void CreateGame(GameRoom room)
        {
            rooms.Add(room);
            room.OnGameUpdate += SendPlayerState;
            room.OnGameClose += GameRoomClose;
        }
        public async Task SendBetQuery(string connectionId, int minBet)
        {
            var client = HubContext.Clients.Client(connectionId);
            await client.SendAsync("DoBet", minBet);
        }

        public async Task SendSetupData(IClientProxy caller, int id)
        {
            await caller.SendAsync("SetupData", id);
        }
        public Server(IHubContext<RoomHub> hubContext)
        {
            
            HubContext = hubContext;
            //test rooms
            //CreateGame(new GameRoom("First one", 1));
            //CreateGame(new GameRoom("Second one", 2));
            //CreateGame(new GameRoom("Third one", 3));
            for(int i = 1; i <= 1000; i++)
                CreateGame(new GameRoom("Room #" + i.ToString(), i));
        }
    }
}
