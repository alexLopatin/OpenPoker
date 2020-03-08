using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenPoker.Logging;
using OpenPoker.GameEngine;
using OpenPoker.Infrastructure;

namespace OpenPoker.Controllers
{
    public class ReplayController : Controller
    {
        ApplicationContext db;
        public ReplayController(ApplicationContext applicationContext)
        {
            db = applicationContext;
        }
        public IActionResult Index(int id)
        {
            

            LogDeserialize log = new LogDeserialize("GameLogs/" + id.ToString() + ".log");
            if(log.Exists())
            {
                //var deserialized = log.Deserialize<List<KeyValuePair<GameEngine.Action, TimeArguments>>>();
                var rawJson = log.GetRawData();
                return View((object)rawJson);
            }
            return View();
        }

    }
}