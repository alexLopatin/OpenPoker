using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenPoker.Controllers
{
    public class RoomController : Controller
    {
        // GET: Room
        public ActionResult Index(int roomId)
        {
            return View(Server.rooms.Find(x=>x.id == roomId));
        }

        public ActionResult Data(int roomId)
        {
            return PartialView(Server.rooms.Find(x => x.id == roomId));
        }
    }
}