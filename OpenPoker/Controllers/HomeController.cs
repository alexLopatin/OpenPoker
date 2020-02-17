using System;
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
        public HomeController(ILogger<HomeController> logger, IServer server)
        {
            _server = server;
            _logger = logger;
        }
        
        private const int countOfRoomsOnPage = 30;

        public IActionResult Index(int page)
        {
            
            int countOfPages = (int)Math.Ceiling((double)_server.rooms.Count / countOfRoomsOnPage);
            if (page <= 0 || page > Math.Ceiling((double) _server.rooms.Count/countOfRoomsOnPage))
                return View( new RoomsList( _server.rooms.GetRange(0, countOfRoomsOnPage), countOfPages));
            page--;
            return View( new RoomsList( _server.rooms.GetRange(page * countOfRoomsOnPage,
                (page + 1)*countOfRoomsOnPage <= _server.rooms.Count ?
                countOfRoomsOnPage : (_server.rooms.Count - (page) * countOfRoomsOnPage))
                , countOfPages));
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
