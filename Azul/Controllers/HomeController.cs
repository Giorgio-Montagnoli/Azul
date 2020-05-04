using Azul.Models.ViewModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Azul.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var dirPath = Server.MapPath(@"~/_db/");

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var filePath = Server.MapPath(@"~/_db/ActiveGames.json");

            if (!System.IO.File.Exists(filePath))
            {
                return View();
            }

            return View(JsonConvert.DeserializeObject<List<GameListVM>>(System.IO.File.ReadAllText(filePath)));
        }
    }
}