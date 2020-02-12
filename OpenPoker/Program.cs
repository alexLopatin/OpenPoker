using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenPoker.GameEngine;

namespace OpenPoker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Server.CreateGame(new GameRoom("First one", 1));
            Server.CreateGame(new GameRoom("Second one", 2));
            Server.CreateGame(new GameRoom("Third one", 3));
            CreateHostBuilder(args).Build().Run();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
