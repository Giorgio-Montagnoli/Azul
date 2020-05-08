using Azul.Helpers;
using Azul.Models;
using Azul.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Azul.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            var cookieGameId = Request.Cookies.Get(Utility.COOKIE_GAME_ID);

            if (cookieGameId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var gameVM = MemoryCache.Default[cookieGameId.Value] as GameVM;

            if (gameVM == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_game", gameVM);
            }

            return View(gameVM);
        }

        public ActionResult StartANewGame()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartANewGame(string name)
        {
            var gameVM = new GameVM();

            MemoryCache.Default.Add(gameVM.Id, gameVM, new CacheItemPolicy());

            var gamesListVM = GetAvailableGames();

            gamesListVM.Add(new GameListVM { Id = gameVM.Id, Players = new List<string> { name } });

            SaveAvailableGames(gamesListVM);

            return JoinAndWait(gameVM, name);
        }

        public ActionResult Join(string gameId)
        {
            return View((object)gameId);
        }

        [HttpPost]
        public ActionResult Join(string name, string gameId)
        {
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            var gamesListVM = GetAvailableGames();

            var gameListVM = gamesListVM.FirstOrDefault(q => q.Id.Equals(gameId));

            if (gameVM == null)
            {
                gamesListVM.Remove(gameListVM);

                SaveAvailableGames(gamesListVM);

                return RedirectToAction("GameNotFound");
            }
            else if (gameVM.Started.HasValue)
            {
                return RedirectToAction("GameStarted");
            }
            else if (gameVM.Players.Count > 3)
            {
                return RedirectToAction("GameFull");
            }
            else if (gameVM.Players.Any(q => q.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return RedirectToAction("UsernameAlreadyUsed");
            }

            gameListVM.Players.Add(name);

            if (gameVM.Players.Count.Equals(4))
            {
                gamesListVM.Remove(gameListVM);
            }

            SaveAvailableGames(gamesListVM);

            return JoinAndWait(gameVM, name);
        }

        public ActionResult GameNotFound()
        {
            ViewBag.ErrorMsg = "Game not found. Try to join/create another one.";
            return View("ErrorPage");
        }

        public ActionResult GameFull()
        {
            ViewBag.ErrorMsg = "Game contains already 4 players. Try to join/create another one.";
            return View("ErrorPage");
        }

        public ActionResult UsernameAlreadyUsed()
        {
            ViewBag.ErrorMsg = "The username you've chosen is already in use. Try with another one.";
            return View("ErrorPage");
        }

        public void GameStarted(string gameId)
        {
            var gamesListVM = GetAvailableGames();

            var gameListVM = gamesListVM.FirstOrDefault(q => q.Id.Equals(gameId));
            gamesListVM.Remove(gameListVM);

            SaveAvailableGames(gamesListVM);
        }

        #region Metodi private

        private ActionResult JoinAndWait(GameVM gameVM, string name)
        {
            gameVM.Players.Add(new Player { Name = name });

            Response.Cookies.Add(
                new HttpCookie(Utility.COOKIE_GAME_ID)
                {
                    Value = gameVM.Id,
                    Expires = DateTime.Now.AddDays(1)
                });

            Response.Cookies.Add(
                new HttpCookie(Utility.COOKIE_GAME_NAME)
                {
                    Value = name,
                    Expires = DateTime.Now.AddDays(1)
                });

            return RedirectToAction("Index");
        }

        private List<GameListVM> GetAvailableGames()
        {
            var dirPath = Server.MapPath(@"~/_db/");

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var filePath = Server.MapPath(@"~/_db/ActiveGames.json");

            if (!System.IO.File.Exists(filePath))
            {
                return new List<GameListVM>();
            }

            return JsonConvert.DeserializeObject<List<GameListVM>>(System.IO.File.ReadAllText(filePath));
        }

        private void SaveAvailableGames(List<GameListVM> gamesListVM)
        {
            var filePath = Server.MapPath(@"~/_db/ActiveGames.json");

            System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(gamesListVM));
        }

        #endregion
    }
}