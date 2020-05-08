using Azul.Helpers;
using Azul.Models.ViewModel;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Azul
{
    public class GameHub : Hub
    {
        public Task Join(string gameId)
        {
            return Groups.Add(Context.ConnectionId, gameId);
        }

        public void HasJoined()
        {
            var gameId = Clients.Caller.gameId as string;
            var name = Clients.Caller.name as string;

            Clients.Group(gameId).showMessage($"{ name } has joined the game");
        }

        public void SendChat(string msg)
        {
            var gameId = Clients.Caller.gameId as string;
            var name = Clients.Caller.name as string;

            Clients.Group(gameId).showMessage($"[ { name } ] says: { msg }");
        }

        public void StartGame()
        {
            var gameId = Clients.Caller.gameId as string;
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            if (gameVM.Start())
            {
                Clients.Group(gameId).showMessage($"{ name } started a game at { gameVM.Started.Value }.");
                Clients.Group(gameId).gameStarted();
                Clients.Group(gameId).refreshGameForAll(gameVM);
            }
        }

        public Task Leave(string gameId)
        {
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            if (!gameVM.Start())
            {
                var player = gameVM.Players.First(q => q.Name.Equals(name));
                gameVM.Players.Remove(player);

                Clients.Group(gameId).showMessage($"{ name } has left the game");

                return Groups.Remove(Context.ConnectionId, gameId);
            }

            return null;
        }

        public void TakeFromExpositor(string gameId, int expositorNumber, int rowNumber, TileType typeSelected)
        {
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            gameVM.TakeFromExpositor(name, expositorNumber, rowNumber, typeSelected);
            Clients.Group(gameId).showMessage($"{ name } -> <img src=\"/Content/images/{typeSelected}.png\" width=\"15\" /> from {expositorNumber} -> {rowNumber} row.");

            if (gameVM.Ended.HasValue)
            {
                var winner = gameVM.Players.OrderByDescending(o => o.VictoryPoints).First();
                Clients.Group(gameId).showMessage($"<span style='color:red'>GAME ENDED! The winnwe is { winner.Name }</span>");
            }

            Clients.Group(gameId).refreshGameForAll(gameVM);
        }

        public void TakeFromCenterTable(string gameId, int rowNumber, TileType typeSelected)
        {
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            gameVM.TakeFromCenterTable(name, rowNumber, typeSelected);
            Clients.Group(gameId).showMessage($"{ name } -> <img src=\"/Content/images/{typeSelected}.png\" width=\"15\" /> from Center -> {rowNumber} row.");

            if (gameVM.Ended.HasValue)
            {
                var winner = gameVM.Players.OrderByDescending(o => o.VictoryPoints).First();
                Clients.Group(gameId).showMessage($"<span style='color:red'>GAME ENDED! The winnwe is { winner.Name }</span>");
            }

            Clients.Group(gameId).refreshGameForAll(gameVM);
        }

    }
}