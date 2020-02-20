using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenPoker.Logging;
using OpenPoker.GameEngine;

namespace OpenPoker.Controllers
{
    public class ReplayController : Controller
    {
        public IActionResult Index(int id)
        {
            LogDeserialize log = new LogDeserialize("GameLogs/" + id.ToString() + ".log");
            if(log.Exists())
            {
                var deserialized = log.Deserialize<List<KeyValuePair<GameEngine.Action, TimeArguments>>>();
                return View(deserialized);
            }
            return View();
        }

    }
}