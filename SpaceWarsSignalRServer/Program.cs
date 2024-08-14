using Microsoft.AspNetCore.ResponseCompression;
using SpaceWarsSignalRServer.Hubs;

namespace SpaceWarsSignalRServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.MapHub<SpaceWarsHub>("/spacewars");

            app.Run();
        }
    }
}
