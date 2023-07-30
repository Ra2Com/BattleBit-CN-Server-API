using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Common.Enums;
using BattleBitAPI.Server;
using CommunityServerAPI.Muj.Common;
using System.Net;

class Program
{

    private static ServerCommandProcessor serverCommandProcessor = null;
	private static Dictionary<MyPlayer, bool> premiumPlayers = new Dictionary<MyPlayer, bool>();
	public static Dictionary<MyPlayer, Maps> VoteMapList = new Dictionary<MyPlayer, Maps>();
	private static MujExtentions MujExtentions = new MujExtentions();

	static void Main(string[] args)
    {
		Task.Run(() => { 
			Timer timer = new(MujExtentions.SetConsoleTitleAsTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
		}); 
		Console.WriteLine("   ___     ___     ___              ___      ___    ___   \r\n  | _ )   | _ )   | _ \\     o O O  /   \\    | _ \\  |_ _|  \r\n  | _ \\   | _ \\   |   /    o       | - |    |  _/   | |   \r\n  |___/   |___/   |_|_\\   TS__[O]  |_|_|   _|_|_   |___|  \r\n_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"| {======|_|\"\"\"\"\"|_| \"\"\" |_|\"\"\"\"\"| \r\n\"`-0-0-'\"`-0-0-'\"`-0-0-'./o--000'\"`-0-0-'\"`-0-0-'\"`-0-0-' \r\n");
		var listener = new ServerListener<MyPlayer>();
        listener.OnPlayerTypedMessage += OnPlayerChat;
        listener.OnGameServerConnected += OnGameServerConnected;
        listener.OnGameServerConnecting += OnGameServerConnecting;
        listener.OnPlayerConnected += OnPlayerConnected;
        listener.OnGetPlayerStats += OnGetPlayerStats;
		listener.Start(12345);//Port

        serverCommandProcessor = new ServerCommandProcessor(listener);
		Task.Run(() => serverCommandProcessor.Start()); //start server command processor

		Thread.Sleep(-1);
    }

	//callback hooks
	private static Task<PlayerStats> OnGetPlayerStats(ulong steamid)
	{
        PlayerStats stats = new PlayerStats();
        stats.Roles = Roles.Admin | Roles.Moderator | Roles.Special;
		return Task.FromResult(stats);
	}

	private static async Task OnPlayerConnected(MyPlayer player)
	{
        premiumPlayers.TryGetValue(player, out var isPremium);
        player.isPremium = isPremium;


		if (!isPremium)
            player.Kick("Not a premium player. pay $2 bux");
	}

	private static async Task<bool> OnGameServerConnecting(IPAddress address)
	{
        await Console.Out.WriteLineAsync(address.ToString() + " is attempting to connect");
        return true;
	}

	private static async Task OnPlayerChat(MyPlayer player, ChatChannel channel, string msg)
	{
        if (player.Stats.Roles.HasFlag(Roles.Admin) && msg.StartsWith("!"))
		{
			MujExtentions.HandleChatCommand(player, msg);
		}
	}

	private static async Task OnGameServerConnected(GameServer server)
	{
        await Console.Out.WriteLineAsync($"{server} just connected");

        Timer timer = new(MujExtentions.SendMessageEveryFiveMinutes, server, TimeSpan.Zero, TimeSpan.FromMinutes(5));

	}
}
public class MyPlayer : Player
{
    public bool isPremium { get; set; }
    public PlayerStats Stats { get; set; }
}
