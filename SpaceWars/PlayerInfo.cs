using System;
using System.ComponentModel;

namespace SpaceWars
{
    public class PlayerInfo : INotifyPropertyChanged
    {
        private int healthPoints;
        public string ConnectionId { get; set; }
        public string PlayerName { get; set; }
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double Angle { get; set; }
        public int HealthPoints
        {
            get { return healthPoints; }
            set
            {
                if (HealthPoints != value)
                {
                    healthPoints = value;
                    OnPropertyChanged(nameof(HealthPoints));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public PlayerInfo()
        {
            LocX = RandomLocationX();
            LocY = RandomLocationY();
            Angle = 0;
        }


        private double RandomLocationX()
        {
            var random = new Random();
            var loc = random.Next(0, 800);
            return loc;
        }

        private double RandomLocationY()
        {
            var random = new Random();
            var loc = random.Next(0, 450);
            return loc;
        }
    }
}
