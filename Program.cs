using CommunityServerAPI.MujAPI.Common.Utils;
using MujAPI;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config",Watch = true)]

class Program
{
	static void Main(string[] args)
	{
		var timerDoneEvent = new ManualResetEvent(false);

		Task.Run(() =>
		{
			Timer timer = new Timer(state => 
			{
				MujUtils.SetConsoleTitle(MujApi.listener);
			}, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			timerDoneEvent.WaitOne();
		});
		MujApi.Start();
		Thread.Sleep(-1);
	}
}