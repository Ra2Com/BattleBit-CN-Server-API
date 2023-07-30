using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Common.Enums;
using BattleBitAPI.Server;
using MujAPI;
using System.Net;

class Program
{

    private static ServerCommandProcessor serverCommandProcessor = null;
	private static Dictionary<MyPlayer, bool> premiumPlayers = new Dictionary<MyPlayer, bool>();
	public static Dictionary<MyPlayer, MapInfo> VoteMapList = new Dictionary<MyPlayer, MapInfo>();

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
		//listener.OnMatchEnding += OnMatchEnding;
		listener.Start(12345);//Port

		Random random = new Random();

		Console.WriteLine("Skip Map Vote:");
		for (int i = 0; i <= 20; i++)
		{
			ulong steamID = (ulong)random.NextInt64();
			MyPlayer player = new MyPlayer(steamID);
			MapInfo mapInfo = new MapInfo((Maps)random.Next(1, 4), (MapDayNight)random.Next(0, 2));

			Console.WriteLine($"	MapInfo {mapInfo} voted by {player.SteamID}");

			VoteMapList.Add(player, mapInfo);
		}

		var (totalOccurrencesCount, maxOccurrencesCount) = MujExtentions.GetOccurances(VoteMapList);
		MapInfo MostVotedMap = MujExtentions.GetMapInfoWithHighestOccurrences(VoteMapList);
		Console.WriteLine($"Total Maps Occurrences Count: {totalOccurrencesCount}");
		Console.WriteLine($"Max Map Occurrences Count: {maxOccurrencesCount} Which is {MostVotedMap}");

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
        if (msg.StartsWith("!"))
		{
			MujExtentions.HandleChatCommand(player, msg);

			// will check if they already voted
			if (!VoteMapList.ContainsKey(player))
			{
				VoteMapList.Add(player, player.votedMap);
			}
			else
			{
				player.GameServer.MessageToPlayer(player, "Already Voted Cannot Vote Again");
			}
		}
		await Console.Out.WriteLineAsync(msg);
	}

	private static async Task OnGameServerConnected(GameServer server)
	{
        await Console.Out.WriteLineAsync($"{server} just connected");

        Timer timer = new(MujExtentions.SendMessageEveryFiveMinutes, server, TimeSpan.Zero, TimeSpan.FromMinutes(5));

	}

	private static async Task OnMatchEnding(GameServer server)
	{
		MapInfo MostVotedMap = MujExtentions.GetMapInfoWithHighestOccurrences(VoteMapList);
		var (totalMapCount, maxMapCount) = MujExtentions.GetOccurances(VoteMapList);

		//server.UILogOnServer($"{MostVotedMap} has been voted the most. {maxMapCount}/{totalMapCount}", 10f);

		Console.WriteLine($"{MostVotedMap}, {maxMapCount}, {totalMapCount}");

		//server.NextMap(MostVotedMap);

	}

}

// map info for map voting
public class MapInfo
{
	public Maps Map { get; set; }
	public MapDayNight DayNight { get; set; }

	public MapInfo(Maps map, MapDayNight dayNight)
	{
		Map = map;
		DayNight = dayNight;
	}

	public MapInfo()
	{
	}

	public override string ToString()
	{
		return this.Map.ToString() + " : " + this.DayNight.ToString();
	}
}

public class MyPlayer : Player
{
    public bool isPremium { get; set; }
	public MapInfo votedMap { get; set; }
    public PlayerStats Stats { get; set; }


	public MyPlayer(ulong steamID)
	{
		SteamID = steamID;
	}

}
