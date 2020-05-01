using Azul.Models.ViewModel;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Azul
{
    public class ChatHub : Hub
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

        public void StartGame()
        {
            var gameId = Clients.Caller.gameId as string;
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            if (gameVM.Start())
            {
                Clients.Group(gameId).showMessage($"{ name } started a game at { gameVM.Started.Value }.");
                Clients.Group(gameId).gameStarted(gameId);
            }
        }

        public Task Leave(string gameId)
        {
            var name = Clients.Caller.name as string;
            var gameVM = MemoryCache.Default[gameId] as GameVM;

            var player = gameVM.Players.First(q => q.Name.Equals(name));
            gameVM.Players.Remove(player);

            Clients.Group(gameId).showMessage($"{ name } has left the game");

            return Groups.Remove(Context.ConnectionId, gameId);
        }
    }
}