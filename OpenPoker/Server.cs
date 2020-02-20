using Microsoft.AspNetCore.SignalR;
using OpenPoker.GameEngine;
using OpenPoker.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace OpenPoker
{
    public interface IServer
    {
        public Dictionary<int, GameRoom> rooms { get; }
        public void CreateGame(GameRoom room);
        public Task SendUpdateData(IClientProxy caller, int roomId, bool showCards);
        public Task Reject(IClientProxy caller, string reason);
        public Task SendBetQuery(string connectionId, int minBet);
        public Task SendSetupData(IClientProxy caller, int id);
        public Task RequireLogin(IClientProxy caller);
    }
    public class Server : IServer
    {
        private int maxCountOfRooms = int.MaxValue;
        public Dictionary<int, GameRoom> rooms { get; private set; } = new Dictionary<int, GameRoom>();
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
                rooms.Remove((sender as GameRoom).id);
        }
        public async Task RequireLogin(IClientProxy caller)
        {
            await caller.SendAsync("RedirectToLogin");
        }
        public async Task SendUpdateData(IClientProxy caller, int roomId, bool showCards = false)
        {
            var room = rooms[roomId];
            var args = room.game.GetUpdateData(showCards);

            foreach (KeyValuePair<GameEngine.Action, object> kvp in args.ActionArgumentsPairs)
            {
                if (kvp.Key.IsPersonal)
                {
                    await HubContext.Clients.Client(kvp.Key.ConnectionId).SendAsync(kvp.Key.Name, kvp.Value);
                }
                else
                    await caller
                        .SendAsync(kvp.Key.Name, kvp.Value);
            }
        }
        public async Task Reject(IClientProxy caller, string reason)
        {
            await caller.SendAsync("Reject", reason);
        }
        public void CreateGame(GameRoom room)
        {
            if (rooms.Count < maxCountOfRooms)
            {
                rooms[room.id] = room;
                room.OnGameUpdate += SendPlayerState;
                room.OnGameClose += GameRoomClose;
            }
            //else
            //    throw new Exception("Count of rooms reached its maximum");
            
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

        public void Configure()
        {
            XmlDocument settings = new XmlDocument();
            settings.Load("Server.xml");
            XmlElement xRoot = settings.DocumentElement;
            if (xRoot.Name == "Server.Configuration")
            {
                foreach (XmlNode xnode in xRoot)
                {
                    if (xnode.Name == "Room.Settings")
                    {
                        foreach (XmlNode roomSettingsNode in xnode)
                        {
                            if (roomSettingsNode.Name == "MaxCount")
                                maxCountOfRooms = int.Parse(roomSettingsNode.InnerText);
                        }
                    }
                }
            }
        }

        public Server(IHubContext<RoomHub> hubContext)
        {
            Configure();
            HubContext = hubContext;
            for(int i = 1; i <= 10; i++)
                CreateGame(new GameRoom("Room #" + i.ToString(), i, 6));
        }
    }
}
