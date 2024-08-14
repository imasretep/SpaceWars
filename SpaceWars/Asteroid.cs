using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpaceWars
{
    public class Asteroid
    {
        public Rectangle AsteroidsGfx { get; set; }
        public double LocX { get; set; }
        public double LocY { get; set; }


        public Asteroid()
        {
            AsteroidsGfx = new Rectangle
            {
                Height = 40,
                Width = 40,
                Fill = Brushes.Gray
            };

            LocX = RandomLocationX();
            LocY = RandomLocationY();

        }


        private int RandomLocationX()
        {
            var random = new Random();
            var loc = random.Next(0, 800);
            return loc;
        }

        private int RandomLocationY()
        {
            var random = new Random();
            var loc = random.Next(0, 450);
            return loc;
        }


    }
}
