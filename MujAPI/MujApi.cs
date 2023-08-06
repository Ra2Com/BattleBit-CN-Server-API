﻿using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.MujAPI.Common.Utils;
using MujAPI.Commands;
using MujAPI.Common;
using MujAPI.Common.Database;
using MujAPI.Common.GameRules;
using System.Net;

namespace MujAPI
{
    public class MujApi
	{
		private static ApiCommandHandler serverCommandProcessor;
		private static Dictionary<MujPlayer, bool> premiumPlayers = new Dictionary<MujPlayer, bool>();
		private static Dictionary<ulong, Roles> thePoliceMods = new Dictionary<ulong, Roles>();
		public static Dictionary<MujPlayer, MapInfo> VoteMapList = new Dictionary<MujPlayer, MapInfo>();
		public static Dictionary<string, GameServer> GameServerIdentifiers = new Dictionary<string, GameServer>();
		public static Dictionary<ulong, MujPlayer> BullyList = new Dictionary<ulong, MujPlayer>();

		//chat command handler
		private static ChatCommandHandler commandHandler = new ChatCommandHandler();


		public static MujGameRules Rules = new MujGameRules();

		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MujApi));

		//flags
		public static bool IsAcrossServerChatOn = false;
		public static ServerListener<MujPlayer> listener = new();

		// start the api
		public static void Start()
		{
			// TODO: init database things here
			MujUtils.RandomMOTD = MujDBConnection.DBGetMotds().Select(sv => sv.Motd).ToList(); // grab motds from server

			log.Info($"Logger Started");

			listener.OnPlayerTypedMessage += OnPlayerChat;
			listener.OnGameServerConnected += OnGameServerConnected;
			listener.OnGameServerConnecting += OnGameServerConnecting;
			listener.OnPlayerConnected += OnPlayerConnected;
			listener.OnPlayerSpawning += OnPlayerSpawning;
			listener.OnGetPlayerStats += OnGetPlayerStats;
			//listener.OnMatchEnding += OnMatchEnding;
			listener.Start(29294);//Port
			log.Info("Listener started");

			//register commands
			ChatCommands.RegisterMyCommands(commandHandler);
			log.Info("ChatCommands Registered");

			//command processor for api
			serverCommandProcessor = new ApiCommandHandler(listener);
			Task.Run(() => serverCommandProcessor.Start());
			log.Info("ApiCommands Listening");
		}

		//for testing map voting
		private static void TestUserVotes(ServerListener<MujPlayer> listener)
		{
			Random rnd = new Random();
			long min = (long)Math.Pow(10, 16);
			long max = (long)Math.Pow(10, 17) - 1;

			for (int i = 0; i < 20; i++)
			{
				ulong steamdid = (ulong)rnd.NextInt64(min, max);
				VoteMapList.Add(new MujPlayer(steamdid), new MapInfo((Maps)rnd.Next(1, 4), (MapDayNight)rnd.Next(0, 2)));
			}

			foreach (var keyValuePair in VoteMapList)
			{
				ulong SteamId = keyValuePair.Key.SteamID;
				string VotedMap = keyValuePair.Value.ToString();

				Console.WriteLine($"{SteamId} voted {VotedMap}");
			}

			var (totalOccurrencesCount, maxOccurrencesCount) = MujUtils.GetOccurances(VoteMapList);
			Console.WriteLine($"Total Occurrences Count: {totalOccurrencesCount}");
			Console.WriteLine($"Max Occurrences Count: {maxOccurrencesCount}");

			var HighestVotedMap = MujUtils.GetMapInfoWithHighestOccurrences(VoteMapList);
			Console.WriteLine($"Highest Vote: {HighestVotedMap}");

		}

		//callback hooks
		private static Task<PlayerStats> OnGetPlayerStats(ulong steamid, PlayerStats stats)
		{
			// TODO: 
			if (steamid == 76561198347766467)
			{
				Roles roles = Roles.Admin | Roles.Moderator;
				stats.Roles = roles;
				thePoliceMods.Add(steamid, roles);
				return Task.FromResult(stats);
			}
			return Task.FromResult(stats);
		}

		// TODO: get player stats from database
		private static async Task OnPlayerConnected(MujPlayer player)
		{
			thePoliceMods.TryGetValue(player.SteamID, out var roles);
			player.Stats.Roles = roles;

			premiumPlayers.TryGetValue(player, out var isPremium);
			player.IsPremium = isPremium;


			if (!isPremium)
				player.Kick("Not a premium player. pay $2 bux");
		}

		private static async Task<bool> OnGameServerConnecting(IPAddress address)
		{
			log.Info(address.ToString() + " is attempting to connect");
			return true;
		}

		// player chat event
		public static async Task OnPlayerChat(MujPlayer player, ChatChannel channel, string msg)
		{
			if (msg.StartsWith("!"))
			{
				string command = msg[1..];

				object[] optionalObjects = new object[] { player, channel };
				await Task.Run(() => commandHandler.ExecuteCommand(command, optionalObjects));
			}
			else if (IsAcrossServerChatOn && channel.HasFlag(ChatChannel.AllChat)) //experimental fr fr
			{
				string ServerIdentifier = GameServerIdentifiers.FirstOrDefault(kvp => kvp.Value == player.GameServer).Key;

				string[] ChatMessage = new string[2];
				ChatMessage[0] = "all";
				ChatMessage[1] = $"({ServerIdentifier}) {player.Name} : {msg}";

				await Task.Run(() => ApiCommands.SendChatMessageToAllServers(ChatMessage, new object[] {}));
			}
		}

		// when a gameserver connects to the api
		public static async Task OnGameServerConnected(GameServer server)
		{

			string ColouredIdentifier = await MujUtils.GetColoredIdentifierAsync(server.ServerName);

			GameServerIdentifiers.Add(ColouredIdentifier, server);

			log.Info($"{server} just connected");

			Timer timer = new(MujUtils.SendToServersMotd, server, TimeSpan.Zero, TimeSpan.FromMinutes(5));

		}

		// match ending
		private static async Task OnMatchEnding(GameServer server)
		{
			MapInfo MostVotedMap = MujUtils.GetMapInfoWithHighestOccurrences(VoteMapList);
			var (totalMapCount, maxMapCount) = MujUtils.GetOccurances(VoteMapList);

			//TODO switch to most voted map if skip vote is initiated

			log.Info($"{MostVotedMap}, {maxMapCount}, {totalMapCount}");
		}


		public static async Task<PlayerSpawnRequest> OnPlayerSpawning(MujPlayer player, PlayerSpawnRequest request)
		{
			if (BullyList.ContainsKey(player.SteamID)) 
			{
				player.GameServer.Kill(player);
				player.TimesBullied++;

				string spelling = player.TimesBullied == 1 ? "Time" : "Times";
				player.GameServer.SayToChat($"{player.Name} has been bullied {player.TimesBullied} {spelling}");
				return request;
			}

			Weapon WeaponPrimary = request.Loadout.PrimaryWeapon.Tool;
			Weapon WeaponSecondary = request.Loadout.SecondaryWeapon.Tool;
			Gadget HeavyGadget = request.Loadout.HeavyGadget;
			Gadget LightGadget = request.Loadout.LightGadget;
			PlayerWearings Wearings = request.Wearings;

			// TODO: make sure that this works
			if (Rules.weaponBans.IsBanned(WeaponPrimary)){
				player.Message($"{WeaponPrimary} is banned");
				WeaponPrimary = null;
			}
			if (Rules.weaponBans.IsBanned(WeaponSecondary))
			{
				player.Message($"{WeaponSecondary} is banned");
				WeaponSecondary = null;
			}
			if (Rules.gadgetBans.IsBanned(HeavyGadget))
			{
				player.Message($"{HeavyGadget} is banned");
				HeavyGadget = null;
			}
			if (Rules.gadgetBans.IsBanned(LightGadget))
			{
				player.Message($"{LightGadget} is banned");
				LightGadget = null;
			}
			var (isBanned, bannedItems) = await Rules.wearingsBans.IsBanned(Wearings);
			if (isBanned)
			{
				if (bannedItems.Count == 0)
				{
					player.Message("Bro ur entire fucking outfit is banned");
					foreach (var field in typeof(PlayerWearings).GetFields())
					{
						field.SetValue(player, null);
					}
				}
				else
				{
					foreach (var item in bannedItems)
					{
						switch (item)
						{
							case "Head":
								Wearings.Head = null;
								break;
							case "Chest":
								Wearings.Chest = null;
								break;
							case "Belt":
								Wearings.Belt = null;
								break;
							case "Backbag":
								Wearings.Backbag = null;
								break;
							case "Eye":
								Wearings.Eye = null;
								break;
							case "Face":
								Wearings.Face = null;
								break;
							case "Hair":
								Wearings.Hair = null;
								break;
							case "Skin":
								Wearings.Skin = null;
								break;
							case "Uniform":
								Wearings.Uniform = null;
								break;
							case "Camo":
								Wearings.Hair = null;
								break;
						}
					}
					player.Message($"The Following items are banned: |{string.Join("|", bannedItems)}|");
				}
			}


			return request;
		}

	}
}
