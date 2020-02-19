using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenPoker.Models;

namespace OpenPoker.Controllers
{
    public class RoomListController : Controller
    {
        private readonly IServer _server;
        public RoomListController(IServer server)
        {
            _server = server;
        }

        private const int countOfRoomsOnPage = 30;

        public IActionResult Index(int page)
        {
            var rooms =  _server.rooms.Select(kvp => kvp.Value).ToList();
            int countOfPages = (int)Math.Ceiling((double)_server.rooms.Count / countOfRoomsOnPage);
            if (page <= 0 || page > Math.Ceiling((double)_server.rooms.Count / countOfRoomsOnPage))
                return View(new RoomsList(rooms.GetRange(0, countOfRoomsOnPage), countOfPages));
            page--;
            return View(new RoomsList(rooms.GetRange(page * countOfRoomsOnPage,
                (page + 1) * countOfRoomsOnPage <= _server.rooms.Count ?
                countOfRoomsOnPage : (_server.rooms.Count - (page) * countOfRoomsOnPage))
                , countOfPages));
        }
    }
}