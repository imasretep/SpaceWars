using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceWars.SignalRService
{
    public class SignalRServer
    {
        private readonly HubConnection _hubConnection;
        public event Action<PlayerInfo> PlayerJoined;
        public event Action<List<PlayerInfo>> PlayersOnServer;
        public event Action<PlayerInfo> LocationChanged;
        public event Action<MissileInfo> MissileFromServer;
        public event Action<PlayerInfo> RemovePlayerFromEnemyList;
        public PlayerShip enemyPlayer;

        public string connectionId => _hubConnection.ConnectionId;

        public SignalRServer()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7140/spacewars")
                .WithAutomaticReconnect().Build();

            _hubConnection.On<PlayerInfo>("PlayerConnectedServer", (playerInfo) => PlayerJoined?.Invoke(playerInfo));
            _hubConnection.On<List<PlayerInfo>>("PlayersConnectedToServer", (playerList) => PlayersOnServer?.Invoke(playerList));
            _hubConnection.On<PlayerInfo>("LocationChanged", (playerInfo) => LocationChanged?.Invoke(playerInfo));
            _hubConnection.On<MissileInfo>("MissileInServer", (missile) => MissileFromServer?.Invoke(missile));
            _hubConnection.On<PlayerInfo>("RemovePlayerFromEnemyList", (playerInfo) => RemovePlayerFromEnemyList?.Invoke(playerInfo));

        }

        public async Task ConnectToServer()
        {
            await _hubConnection.StartAsync();
        }

        public async Task AddPlayerToServer(PlayerInfo playerInfo)
        {
            await _hubConnection.InvokeAsync("AddPlayerToServer", playerInfo);
        }

        public async Task AddMissilesToServer(MissileInfo missile)
        {
            await _hubConnection.InvokeAsync("AddMissileToServer", missile);
        }

        public async Task GetPlayers()
        {
            await _hubConnection.InvokeAsync("PlayersOnServer");
        }

        public async Task SendMessage(string message)
        {
            await _hubConnection.InvokeAsync("SendMessage", message);
        }

        public PlayerShip UpdatePlayer(PlayerInfo playerInfo)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                enemyPlayer = new PlayerShip(playerInfo.UserName);
                enemyPlayer.PlayerInfo = playerInfo;

            });
            return enemyPlayer;
        }

        public void UpdatePlayer(PlayerShip playerShip, SignalRServer server)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                playerShip.PlayerInfo.ConnectionId = connectionId;
                server.AddPlayerToServer(playerShip.PlayerInfo);

            });

            GetPlayers();
        }
        public async void UpdatePlayerLocation(PlayerShip playerShip)
        {
            await _hubConnection.InvokeAsync("PlayerLocation", playerShip.PlayerInfo);
        }

        public async void RemovePlayer(PlayerShip playerShip)
        {
            await _hubConnection.InvokeAsync("RemovePlayerFromServer", playerShip.PlayerInfo);
        }

    }
}
