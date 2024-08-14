using SpaceWars.SignalRService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpaceWars
{
    public class PlayerShip 
    {
        public PlayerInfo PlayerInfo { get; set; }
        public Polygon PlayerGfx { get; set; }
        public Missile missile { get; set; }
        public bool wPressed, aPressed, sPressed, dPressed, ePressed, thrusting;
        public double Speed { get; set; }
        public double MaxPositiveSpeed { get; set; }
        public double MaxNegativeSpeed { get; set; }
        public bool Engaged { get; set; }
        public bool isRotated { get; set; }

        public PlayerShip()
        {
            PlayerInfo = new PlayerInfo();
            PlayerInfo.HealthPoints = 10;
            Speed = 0;
            MaxPositiveSpeed = 3;
            MaxNegativeSpeed = 0;
            Engaged = false;


            DrawShip();
        }


        private void DrawShip()
        {
            PlayerGfx = new Polygon
            {
                Height = 20,
                Width = 20,
                Points = new PointCollection
                {
                new Point(20,10),
                new Point(0,0),
                new Point(0,0),
                new Point(5,10),
                new Point(0,20),
                },
                Stroke = Brushes.White,
                Fill = Brushes.Transparent,

            };
        }

        public bool TakeDamage(int damage)
        {
            if(PlayerInfo.HealthPoints > 0)
            {
                PlayerInfo.HealthPoints -= damage;
                if(PlayerInfo.HealthPoints == 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                Debug.WriteLine("Game Over!");
                return true;
            }
        }

        public int Shoot(int counter, Canvas cnvGameField, List<Missile> missiles, SignalRServer server)
        {
            if (counter == 0)
            {
                missile = new Missile();
                missile.Info.Angle = this.PlayerInfo.Angle;
                missile.Info.PlayerConnectionId = this.PlayerInfo.ConnectionId;

                // Rotates the missile to right directions
                missile.MissileGfx.RenderTransformOrigin = new Point(0.5, 0.5);
                RotateTransform rotateMissile = new RotateTransform(missile.Info.Angle);
                missile.MissileGfx.RenderTransform = rotateMissile;

                double playerX = Canvas.GetLeft(this.PlayerGfx) + this.PlayerGfx.Width / 2;
                double playerY = Canvas.GetTop(this.PlayerGfx) + this.PlayerGfx.Height / 2;
                

                playerX = playerX - missile.MissileGfx.Width / 2;
                playerY = playerY - missile.MissileGfx.Height / 2;
                

                // For client
                Canvas.SetTop(missile.MissileGfx, playerY);
                Canvas.SetLeft(missile.MissileGfx, playerX );

                cnvGameField.Children.Add(missile.MissileGfx);

                // For server
                missile.Info.LocX = playerX;
                missile.Info.LocY = playerY;

                server.AddMissilesToServer(missile.Info);

                missiles.Add(missile);
                counter = 20;
            }
            return counter;
        }

        public void ActionDown(Key key)
        {
            switch (key)
            {
                case Key.W:
                    wPressed = true;
                    thrusting = true;
                    break;
                case Key.S:
                    sPressed = true;
                    break;
                case Key.A:
                    aPressed = true;
                    break;
                case Key.D:
                    dPressed = true;
                    break;
                case Key.E:
                    ePressed = true;
                    break;
            }
        }
        public void ActionUp(Key key)
        {
            switch (key)
            {
                case Key.W:
                    wPressed = false;
                    thrusting = false;

                    //this.Speed = 0;

                    break;
                case Key.S:
                    sPressed = false;
                    break;
                case Key.A:
                    aPressed = false;
                    break;
                case Key.D:
                    dPressed = false;
                    break;
                case Key.E:
                    ePressed = false;
                    break;
            }
        }

        public void RotateShip(Key key)
        {

            switch (key)
            {
                case Key.A:
                    this.PlayerInfo.Angle -= 2.5;
                    Rotate();
                    break;
                case Key.D:
                    this.PlayerInfo.Angle += 2.5;
                    Rotate();
                    break;
            }
        }

        private void Rotate()
        {
            this.PlayerGfx.RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotateShip = new RotateTransform(this.PlayerInfo.Angle);
            this.PlayerGfx.RenderTransform = rotateShip;
        }

        public void Rotate(PlayerInfo playerAngle)
        {
            this.PlayerGfx.RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotateShip = new RotateTransform(playerAngle.Angle);
            this.PlayerGfx.RenderTransform = rotateShip;
        }

        public void Accelerate()
        {
            // Speed up
            if (this.Speed < this.MaxPositiveSpeed)
            {
                this.Speed += 0.2;
            }
        }

        public void Decelerate()
        {

            if (this.Speed > this.MaxNegativeSpeed)
            {
                this.Speed += -0.2;
            }

        }

    }
}
