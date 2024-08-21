using Microsoft.AspNetCore.SignalR;

namespace SpaceWarsSignalRServer.Hubs
{
    public class SpaceWarsHub : Hub
    {
        private static List<PlayerInfo> connectedPlayers = new List<PlayerInfo>();
        public async Task SendMessage(string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("Recieve message", message);
        }

        public async Task AddPlayerToServer(PlayerInfo playerInfo)
        {
            Console.WriteLine($"A player {playerInfo.UserName} connected to server");

            connectedPlayers.Add(playerInfo);
            await Clients.All.SendAsync("PlayerConnectedServer", playerInfo);
        }

        public async Task PlayersOnServer()
        {
            Console.WriteLine("Jeejee");
            await Clients.Caller.SendAsync("PlayersConnectedToServer", connectedPlayers);
        }

        public async Task PlayerLocation(PlayerInfo playerLocation)
        {
            //Console.WriteLine("PlayerID: " + playerLocation.ConnectionId + " Loc x: " + playerLocation.LocX + " Loc y: " + playerLocation.LocY + " Angle: " + playerLocation.Angle);
            await Clients.All.SendAsync("LocationChanged", playerLocation);
        }

        public async Task AddMissileToServer(MissileInfo missile)
        {
            //Console.WriteLine("Pew pew!");
            await Clients.All.SendAsync("MissileInServer", missile);
        }

        public async Task RemovePlayerFromServer(PlayerInfo playerInfo)
        {

            for (int i = connectedPlayers.Count - 1; i >= 0; i--)
            {
                var player = connectedPlayers[i];
                if (playerInfo.ConnectionId == player.ConnectionId)
                {
                    connectedPlayers.RemoveAt(i);
                }
            }

            await Clients.All.SendAsync("RemovePlayerFromEnemyList", playerInfo);
        }

    }
}
