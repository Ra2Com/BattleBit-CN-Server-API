using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using MujAPI;
using System.Net;
using BattleBitAPI.Storage;
using System.Numerics;
using BattleBitAPI.Common.Enums;

class Program
{
    private static ServerCommandProcessor serverCommandProcessor = null;
	private static Dictionary<MyPlayer, bool> premiumPlayers = new Dictionary<MyPlayer, bool>();
	private static Dictionary<ulong, Roles> thePoliceMods = new Dictionary<ulong, Roles>();
	public static Dictionary<MyPlayer, MapInfo> VoteMapList = new Dictionary<MyPlayer, MapInfo>();
	public static bool IsAcrossServerChatOn = false;

	static void Main(string[] args)
    {
		Task.Run(() => { 
			Timer timer = new(MujUtils.SetConsoleTitleAsTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
		}); 
		var listener = new ServerListener<MyPlayer>();
        listener.OnPlayerTypedMessage += OnPlayerChat;
        listener.OnGameServerConnected += OnGameServerConnected;
        listener.OnGameServerConnecting += OnGameServerConnecting;
        listener.OnPlayerConnected += OnPlayerConnected;
		listener.OnPlayerSpawning += OnPlayerSpawning;
		listener.OnGetPlayerStats += OnGetPlayerStats;
		//listener.OnMatchEnding += OnMatchEnding;
		listener.Start(12345);//Port

		serverCommandProcessor = new ServerCommandProcessor(listener);
		Task.Run(() => serverCommandProcessor.Start()); //start server command processor

		Thread.Sleep(-1);
    }


	//callback hooks
	private static Task<PlayerStats> OnGetPlayerStats(ulong steamid, PlayerStats stats)
	{
		if (steamid == 76561198347766467)
		{
			Roles roles = Roles.Admin | Roles.Moderator;
			stats.Roles = roles;
			thePoliceMods.Add(steamid, roles);
			return Task.FromResult(stats);
		}
		return Task.FromResult(stats);
	}


	private static async Task OnPlayerConnected(MyPlayer player)
	{
		thePoliceMods.TryGetValue(player.SteamID, out var roles);
		player.stats.Roles = roles;

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
			MujUtils.HandleChatCommand(player, channel, msg);
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

        Timer timer = new(MujUtils.SendMessageEveryFiveMinutes, server, TimeSpan.Zero, TimeSpan.FromMinutes(5));

	}

	private static async Task OnMatchEnding(GameServer server)
	{
		MapInfo MostVotedMap = MujUtils.GetMapInfoWithHighestOccurrences(VoteMapList);
		var (totalMapCount, maxMapCount) = MujUtils.GetOccurances(VoteMapList);

		//server.UILogOnServer($"{MostVotedMap} has been voted the most. {maxMapCount}/{totalMapCount}", 10f);

		Console.WriteLine($"{MostVotedMap}, {maxMapCount}, {totalMapCount}");

		//server.NextMap(MostVotedMap);

	}


    private static async Task<PlayerSpawnRequest> OnPlayerSpawning(MyPlayer player, PlayerSpawnRequest request)
    {
        if (request.Loadout.PrimaryWeapon.Tool == Weapons.M4A1)
        {
            //Don't allow M4A1
            request.Loadout.PrimaryWeapon.Tool = null;
        }
        else if (request.Loadout.PrimaryWeapon.Tool.WeaponType == WeaponType.SniperRifle)
        {
            //Force 6x if weapon is sniper.
            request.Loadout.PrimaryWeapon.MainSight = Attachments._6xScope;
        }

        //Override pistol with deagle
        request.Loadout.SecondaryWeapon.Tool = Weapons.DesertEagle;

        //Force everyone to use RPG
        request.Loadout.LightGadget = Gadgets.Rpg7HeatExplosive;

        //Don't allow C4s
        if (request.Loadout.HeavyGadget == Gadgets.C4)
            request.Loadout.HeavyGadget = null;

        //Spawn player 2 meter above than the original position.
        request.SpawnPosition.Y += 2f;

        //Remove spawn protection
        request.SpawnProtection = 0f;

        //Remove chest armor
        request.Wearings.Chest = null;

        //Give extra 10 more magazines on primary
        request.Loadout.PrimaryExtraMagazines += 10;

        //Give extra 5 more throwables 
        request.Loadout.ThrowableExtra += 5;

        return request;
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
	public PlayerStats stats { get; set; }

	public MyPlayer(ulong steamID)
	{
		SteamID = steamID;
	}
}
