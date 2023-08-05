using CommunityServerAPI.MujAPI.Common.Utils;
using Microsoft.Extensions.Configuration;
using MujAPI;
using MujAPI.Common;
using MujAPI.Common.Database;
using dotenv.net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

class Program
{
	static void Main(string[] args)
	{
		var timerDoneEvent = new ManualResetEvent(false);

		DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

		Task.Run(() =>
		{
			Timer timer = new Timer(state => 
			{
				MujUtils.SetConsoleTitle(MujApi.listener);
			}, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			timerDoneEvent.WaitOne();
		});

		MujDBConnection.Start();

		MujApi.Start();
		Thread.Sleep(-1);
	}
}