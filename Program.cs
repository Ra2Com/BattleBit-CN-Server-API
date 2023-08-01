using MujAPI;
using log4net;
using log4net.Config;

class Program
{
<<<<<<< HEAD
	static void Main(string[] args)
    {
		Task.Run(() => { 
			Timer timer = new(MujUtils.SetConsoleTitleAsTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		});
		MujApi.Start();
		Thread.Sleep(-1);
    }

=======
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer>();
        listener.OnGameServerTick += OnGameServerTick;
        listener.Start(29294);//Port
        Thread.Sleep(-1);
    }

    private static async Task OnGameServerTick(GameServer server)
    {
        //server.Settings.SpectatorEnabled = !server.Settings.SpectatorEnabled;
        //server.MapRotation.AddToRotation("DustyDew");
        //server.MapRotation.AddToRotation("District");
        //server.GamemodeRotation.AddToRotation("CONQ");
        //server.ForceEndGame();
    }
}
class MyPlayer : Player
{
    
>>>>>>> ec935486deb92a1ce00e871e2b766d2d7340b826
}
