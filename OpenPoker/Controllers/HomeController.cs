﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenPoker.Models;
using OpenPoker.GameEngine;
using OpenPoker.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace OpenPoker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServer _server;
        private readonly MatchMaker _matchMaker;
        public HomeController(ILogger<HomeController> logger, IServer server, MatchMaker matchMaker)
        {
            _server = server;
            _logger = logger;
            _matchMaker = matchMaker;
        }
        
        private const int countOfRoomsOnPage = 30;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Play()
        {
            int id = _matchMaker.CreateOrFindRoom();
            
            return RedirectToAction("Index", "Room", new { roomId = id });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
