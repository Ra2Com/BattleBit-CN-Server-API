using MujAPI;

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