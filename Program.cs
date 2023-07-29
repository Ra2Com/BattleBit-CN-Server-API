using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Common.Enums;
using BattleBitAPI.Server;
using System.Net;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer>();
        listener.OnPlayerTypedMessage += OnPlayerChat;
        listener.OnGameServerConnected += OnGameServerConnected;
        listener.OnGameServerConnecting += OnGameServerConnecting;
        listener.Start(12345);//Port
        Thread.Sleep(-1);
    }

	private static async Task<bool> OnGameServerConnecting(IPAddress address)
	{
        await Console.Out.WriteLineAsync(address.ToString() + " is attempting to connect");
        return true;
	}

	private static Task OnPlayerChat(MyPlayer player, ChatChannel channel, string msg)
	{
        if (player.Name == "muj_2498") 
        {
            if (!string.IsNullOrEmpty(msg))
            {
                return Task.CompletedTask;
            }
			else if (msg.StartsWith("scale help"))
			{
				player.GameServer.MessageToPlayer(player, "USAGE: scale [8vs8/16vs16/32vs32/64vs64/127vs127] (Starts new round if server is waiting for players). EXAMPLE: scale 8vs8");
			}
			else if (msg.StartsWith("scale"))
            {
                player.GameServer.ChangeScale(msg.Remove(0, 5));
            }
        }
        return Task.CompletedTask;
	}

	private static async Task OnGameServerConnected(GameServer server)
	{
        await Console.Out.WriteLineAsync($"{server} just connected");
	}
}
class MyPlayer : Player
{
}
