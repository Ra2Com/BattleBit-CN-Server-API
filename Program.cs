using MujAPI;
using log4net;
using log4net.Config;
using CommunityServerAPI.MujAPI;

class Program
{
	static void Main(string[] args)
	{
		Task.Run(() =>
		{
			Timer timer = new(MujUtils.SetConsoleTitleAsTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		});
		MujApi.Start();
		Thread.Sleep(-1);
	}
}