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
	public static bool IsAcrossServerChatOn = false;

	static void Main(string[] args)
    {
		Task.Run(() => { 
			Timer timer = new(MujExtentions.SetConsoleTitleAsTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}); 
		var listener = new ServerListener<MyPlayer>();
        listener.OnPlayerTypedMessage += OnPlayerChat;
        listener.OnGameServerConnected += OnGameServerConnected;
        listener.OnGameServerConnecting += OnGameServerConnecting;
        listener.OnPlayerConnected += OnPlayerConnected;
        listener.OnGetPlayerStats += OnGetPlayerStats;
		//listener.OnMatchEnding += OnMatchEnding;
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
        if (msg.StartsWith("!"))
		{
			MujExtentions.HandleChatCommand(player, channel, msg);
			await Console.Out.WriteLineAsync(msg);
			// will check if they already voted
			if (!VoteMapList.ContainsKey(player))
			{
				VoteMapList.Add(player, player.votedMap);
				return;
			}
			else
			{
				player.GameServer.MessageToPlayer(player, "Already Voted Cannot Vote Again");
				return;
			}
		}
		else if (IsAcrossServerChatOn && channel.HasFlag(ChatChannel.AllChat))
		{
			string[] ChatMessage = new string[2];
			ChatMessage[0] = "all";
			ChatMessage[1] = $"GLOBAL: {player.Name} : {msg}";
			
			serverCommandProcessor.SendChatMessageToAllServers(ChatMessage);
		}

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
