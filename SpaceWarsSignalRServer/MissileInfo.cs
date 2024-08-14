namespace SpaceWarsSignalRServer.Hubs
{
    public class MissileInfo
    {
        public string PlayerConnectionId { get; set; }
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double Angle { get; set; }

    }
}
