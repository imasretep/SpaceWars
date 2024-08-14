using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpaceWars
{
    public class Missile
    {
        public Rectangle MissileGfx { get; set; }
        public MissileInfo Info { get; set; }
        //public double Angle { get; set; }
        //public double locX { get; set; }
        //public double locY { get; set; }


        public Missile()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Info = new MissileInfo();
                MissileGfx = new Rectangle
                {
                    Width = 10,
                    Height = 2,
                    Fill = Brushes.White

                };
            });
        }


    }
}
