using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using SpaceWars.SignalRService;
using System.Windows.Media;


namespace SpaceWars
{
    /// <summary>
    /// Interaction logic for BattlePage.xaml
    /// </summary>
    public partial class BattlePage : Page
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        public PlayerShip playerShip;
        private List<Missile> missiles = new List<Missile>();
        private List<Asteroid> asteroids = new List<Asteroid>();
        private List<PlayerShip> enemyPlayerShips = new List<PlayerShip>();
        private List<Missile> enemyMissiles = new List<Missile>();
        private Asteroid asteroid;
        private int counter = 0;
        private SignalRServer server;
        private bool isDestroyed;

        double distanceX = 0;
        double distanceY = 0;

        MainWindow mainWindow;

        public BattlePage(SignalRServer server, MainWindow mainWindow)
        {
            InitializeComponent();
            this.server = server;
            this.mainWindow = mainWindow;
            server.PlayerJoined += Server_PlayerJoined;
            server.PlayersOnServer += Server_PlayersOnServer;
            server.LocationChanged += Server_LocationChanged;
            server.MissileFromServer += Server_MissileFromServer;
            server.RemovePlayerFromEnemyList += Server_RemovePlayerFromEnemyList;
            InitializePlayer();

            Loaded += Page_Loaded;

        }

        private void Server_RemovePlayerFromEnemyList(PlayerInfo playerInfo)
        {
            var playersToRemove = new List<PlayerShip>();

            foreach (var player in enemyPlayerShips)
            {
                if (playerInfo.ConnectionId == player.PlayerInfo.ConnectionId)
                {
                    Dispatcher.Invoke(() =>
                    {
                        cnvGameField.Children.Remove(player.PlayerGfx);
                        playersToRemove.Add(player);
                    });

                }
            }

            foreach (var playerToRemove in playersToRemove)
            {
                enemyPlayerShips.Remove(playerToRemove);
            }

        }

        private void Server_MissileFromServer(MissileInfo missileInfo)
        {
            if (missileInfo.PlayerConnectionId != this.playerShip.PlayerInfo.ConnectionId)
            {
                var missile = new Missile();
                missile.Info = missileInfo;

                Dispatcher.Invoke(() =>
                {

                    missile.MissileGfx.RenderTransformOrigin = new Point(0.5, 0.5);
                    RotateTransform rotateMissile = new RotateTransform(missile.Info.Angle);
                    missile.MissileGfx.RenderTransform = rotateMissile;

                    Canvas.SetLeft(missile.MissileGfx, missile.Info.LocX * missile.MissileGfx.Width);
                    Canvas.SetTop(missile.MissileGfx, missile.Info.LocY * missile.MissileGfx.Height);


                    enemyMissiles.Add(missile);
                    cnvGameField.Children.Add(missile.MissileGfx);
                });
            }

        }

        private void Server_LocationChanged(PlayerInfo playerLocationInfo)
        {
            foreach (var enemyPlayer in enemyPlayerShips)
            {
                if (enemyPlayer.PlayerInfo.ConnectionId == playerLocationInfo.ConnectionId)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        enemyPlayer.Rotate(playerLocationInfo);

                        Canvas.SetLeft(enemyPlayer.PlayerGfx, playerLocationInfo.LocX);
                        Canvas.SetTop(enemyPlayer.PlayerGfx, playerLocationInfo.LocY);

                    });
                }
            }
        }

        private void Server_PlayersOnServer(List<PlayerInfo> playerList)
        {
            InitializePlayers(playerList);
        }

        private void Server_PlayerJoined(PlayerInfo player)
        {
            if (player.ConnectionId != this.playerShip.PlayerInfo.ConnectionId)
            {
                var enemyShip = server.UpdatePlayer(player);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    enemyPlayerShips.Add(enemyShip);
                    Canvas.SetLeft(enemyShip.PlayerGfx, enemyShip.PlayerInfo.LocX);
                    Canvas.SetTop(enemyShip.PlayerGfx, enemyShip.PlayerInfo.LocY);
                    cnvGameField.Children.Add(enemyShip.PlayerGfx);
                });
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTimer();
            this.Focus();
        }

        private void InitializePlayers(List<PlayerInfo> playerList)
        {
            if (playerList != null)
            {
                foreach (var player in playerList)
                {
                    if (player.ConnectionId != this.playerShip.PlayerInfo.ConnectionId)
                    {
                        var enemyShip = server.UpdatePlayer(player);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            enemyPlayerShips.Add(enemyShip);
                            Canvas.SetLeft(enemyShip.PlayerGfx, enemyShip.PlayerInfo.LocX);
                            Canvas.SetTop(enemyShip.PlayerGfx, enemyShip.PlayerInfo.LocY);
                            cnvGameField.Children.Add(enemyShip.PlayerGfx);
                        });
                    }
                }
            }
        }

        private async void InitializePlayer()
        {
            playerShip = new PlayerShip();
            Canvas.SetLeft(playerShip.PlayerGfx, playerShip.PlayerInfo.LocX);
            Canvas.SetTop(playerShip.PlayerGfx, playerShip.PlayerInfo.LocY);
            cnvGameField.Children.Add(playerShip.PlayerGfx);
            server.UpdatePlayer(playerShip, server);

            mainWindow.DataContext = this.playerShip.PlayerInfo;

        }


        private void InitializeAsteroids()
        {
            asteroid = new Asteroid();
            Canvas.SetLeft(asteroid.AsteroidsGfx, asteroid.LocX);
            Canvas.SetTop(asteroid.AsteroidsGfx, asteroid.LocY);
            cnvGameField.Children.Add(asteroid.AsteroidsGfx);

            asteroids.Add(asteroid);

        }

        private void InitializeTimer()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(8);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (playerShip.wPressed) playerShip.Accelerate();
            if (playerShip.sPressed) playerShip.Decelerate();
            if (playerShip.aPressed) playerShip.RotateShip(Key.A);
            if (playerShip.dPressed) playerShip.RotateShip(Key.D);
            if (playerShip.ePressed)
            {
                counter = playerShip.Shoot(counter, cnvGameField, missiles, server);

            };
            MoveShip();
            MoveMissile();
            MoveAsteroid();
            if (counter > 0)
            {
                counter--;
            }
            else
            {
                counter = 0;
            }
        }

        private void CollisionDetection()
        {
            List<Missile> missilesToRemove = new List<Missile>();
            List<Missile> enemyMissilesToRemove = new List<Missile>();
            foreach (var missile in missiles)
            {
                Rect missileBounds = new Rect(
                    Canvas.GetLeft(missile.MissileGfx),
                    Canvas.GetTop(missile.MissileGfx),
                    missile.MissileGfx.ActualWidth,
                    missile.MissileGfx.ActualHeight);

                foreach (var enemy in enemyPlayerShips)
                {
                    Rect enemyBounds = new Rect(
                        Canvas.GetLeft(enemy.PlayerGfx),
                        Canvas.GetTop(enemy.PlayerGfx),
                        enemy.PlayerGfx.ActualWidth,
                        enemy.PlayerGfx.ActualHeight);

                    if (missileBounds.IntersectsWith(enemyBounds))
                    {
                        Debug.WriteLine("Missile hit enemy!");
                        // Add items to the removal lists
                        missilesToRemove.Add(missile);
                        break;
                    }
                }
            }

            foreach (var missile in enemyMissiles)
            {
                Rect missileBounds = new Rect(
                    Canvas.GetLeft(missile.MissileGfx),
                    Canvas.GetTop(missile.MissileGfx),
                    missile.MissileGfx.ActualWidth,
                    missile.MissileGfx.ActualHeight);

                Rect playerBounds = new Rect(
                    Canvas.GetLeft(this.playerShip.PlayerGfx),
                    Canvas.GetTop(this.playerShip.PlayerGfx),
                    this.playerShip.PlayerGfx.ActualWidth,
                    this.playerShip.PlayerGfx.ActualHeight
                    );

                if (missileBounds.IntersectsWith(playerBounds))
                {
                    Debug.WriteLine("Missile hit you!");


                    isDestroyed = this.playerShip.TakeDamage(1);
                    if (isDestroyed)
                    {
                        // Game Over
                        // Send server message (this.playerShip.PlayerInfo -> Remove from other connected clients enemylists)
                        server.RemovePlayer(this.playerShip);
                        cnvGameField.Children.Remove(this.playerShip.PlayerGfx);
                        playerShip = null;
                        // Do something...

                        InitializePlayer(); // Placeholder
                    }

                    // Add items to the removal lists
                    enemyMissilesToRemove.Add(missile);
                    break;
                }

            }

            foreach (var enemy in enemyPlayerShips)
            {
                Rect playerBounds = new Rect(
                    Canvas.GetLeft(this.playerShip.PlayerGfx),
                    Canvas.GetTop(this.playerShip.PlayerGfx),
                    this.playerShip.PlayerGfx.ActualWidth,
                    this.playerShip.PlayerGfx.ActualHeight
                    );

                Rect enemyBounds = new Rect(
                    Canvas.GetLeft(enemy.PlayerGfx),
                    Canvas.GetTop(enemy.PlayerGfx),
                    enemy.PlayerGfx.ActualWidth,
                    enemy.PlayerGfx.ActualHeight);

                if (playerBounds.IntersectsWith(enemyBounds))
                {
                    // Do something...
                }

            }

            if (missilesToRemove.Count > 0)
            {
                foreach (var missileToRemove in missilesToRemove)
                {
                    missiles.Remove(missileToRemove);
                    cnvGameField.Children.Remove(missileToRemove.MissileGfx);
                }
            }

            if (enemyMissiles.Count > 0)
            {
                foreach (var missileToRemove in enemyMissilesToRemove)
                {
                    enemyMissiles.Remove(missileToRemove);
                    cnvGameField.Children.Remove(missileToRemove.MissileGfx);
                }
            }

        }


        private void MoveShip()
        {
            double newLocX = playerShip.PlayerInfo.LocX;
            double newLocY = playerShip.PlayerInfo.LocY;


            if (playerShip.thrusting || playerShip.sPressed)
            {
                if (playerShip.Engaged == false)
                {
                    playerShip.Engaged = true;
                }

                distanceX = Math.Cos(Math.PI * playerShip.PlayerInfo.Angle / 180.0) * playerShip.Speed;
                distanceY = Math.Sin(Math.PI * playerShip.PlayerInfo.Angle / 180.0) * playerShip.Speed;

            }
            if (playerShip.Engaged)
            {
                newLocX = Canvas.GetLeft(playerShip.PlayerGfx) + distanceX;
                newLocY = Canvas.GetTop(playerShip.PlayerGfx) + distanceY;
            }



            // Check boundaries
            if (newLocX < 0)
            {
                newLocX = cnvGameField.ActualWidth - playerShip.PlayerGfx.ActualWidth;
            }
            else if (newLocX > cnvGameField.ActualWidth - playerShip.PlayerGfx.ActualWidth)
            {
                newLocX = 0;
            }

            if (newLocY < 0)
            {
                newLocY = cnvGameField.ActualHeight - playerShip.PlayerGfx.ActualHeight;
            }
            else if (newLocY > cnvGameField.ActualHeight - playerShip.PlayerGfx.ActualHeight)
            {
                newLocY = 0;
            }

            Canvas.SetLeft(playerShip.PlayerGfx, newLocX);
            Canvas.SetTop(playerShip.PlayerGfx, newLocY);

            playerShip.PlayerInfo.LocX = Canvas.GetLeft(playerShip.PlayerGfx);
            playerShip.PlayerInfo.LocY = Canvas.GetTop(playerShip.PlayerGfx);

            server.UpdatePlayerLocation(this.playerShip);
            CollisionDetection();

        }

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            playerShip.ActionUp(e.Key);
        }

        private void MoveMissile()
        {
            List<Missile> missilesToRemove = new List<Missile>();

            // Player
            for (int i = 0; i < missiles.Count; i++)
            {
                var missile = missiles[i];

                // Get the current position of the missile
                missile.Info.LocY += Math.Sin(Math.PI * missile.Info.Angle / 180.0) * 10;
                missile.Info.LocX += Math.Cos(Math.PI * missile.Info.Angle / 180.0) * 10;

                // Set the new position of the missile
                Canvas.SetTop(missile.MissileGfx, missile.Info.LocY);
                Canvas.SetLeft(missile.MissileGfx, missile.Info.LocX);
                CollisionDetection();

                // Remove missiles that are out of bounds
                if (missile.Info.LocY < 0 || missile.Info.LocY > cnvGameField.ActualHeight || missile.Info.LocX < 0 || missile.Info.LocX > cnvGameField.ActualWidth)
                {
                    missilesToRemove.Add(missile);
                }
                foreach (var missileToRemove in missilesToRemove)
                {
                    missiles.Remove(missileToRemove);
                    cnvGameField.Children.Remove(missileToRemove.MissileGfx);
                }
            }


            // Enemy
            for (int i = 0; i < enemyMissiles.Count; i++)
            {
                var missile = enemyMissiles[i];

                // Get the current position of the missile
                missile.Info.LocY += Math.Sin(Math.PI * missile.Info.Angle / 180.0) * 10;
                missile.Info.LocX += Math.Cos(Math.PI * missile.Info.Angle / 180.0) * 10;

                // Set the new position of the missile
                Canvas.SetTop(missile.MissileGfx, missile.Info.LocY);
                Canvas.SetLeft(missile.MissileGfx, missile.Info.LocX);
                CollisionDetection();

                // Remove missiles that are out of bounds
                if (missile.Info.LocY < 0 || missile.Info.LocY > cnvGameField.ActualHeight || missile.Info.LocX < 0 || missile.Info.LocX > cnvGameField.ActualWidth)
                {
                    missilesToRemove.Add(missile);
                }
                foreach (var missileToRemove in missilesToRemove)
                {
                    enemyMissiles.Remove(missileToRemove);
                    cnvGameField.Children.Remove(missileToRemove.MissileGfx);
                }

            }
        }

        private void MoveAsteroid()
        {


        }
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            playerShip.ActionDown(e.Key);
        }
    }
}
