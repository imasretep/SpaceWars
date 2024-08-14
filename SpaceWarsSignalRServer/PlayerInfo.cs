namespace SpaceWarsSignalRServer.Hubs
{
    public class PlayerInfo
    {
        public string ConnectionId { get; set; }
        public string PlayerName { get; set; }
        public double LocX { get; set; }
        public double LocY { get; set; }
        public double Angle { get; set; }
        public int HealthPoints { get; set; }

    }
}
