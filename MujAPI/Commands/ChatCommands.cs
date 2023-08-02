using BattleBitAPI;
using BattleBitAPI.Common;
using MujAPI;
using System.Numerics;
using System.Text;

namespace MujAPI.Commands
{
	public class ChatCommands
	{

		public static bool IsVoteMapSkipAnnounced = false;
		public static bool IsMapVoteTrollFlagOn = false;
		public static int TotalVoteKicksNeeded = 0;
		public static Dictionary<ulong, int> SteamIDKickVotes = new Dictionary<ulong, int>();


		public static void RegisterMyCommands(ChatCommandHandler commandHandler)
		{
			//register commands
			commandHandler.AddCommand("votekick", ChatCommands.VoteKickCommand);
			commandHandler.AddCommand("kill", ChatCommands.KillCommand);
			commandHandler.AddCommand("skipmap", ChatCommands.SkipMapCommand);
		}


		// !votekick Callback
		public static void VoteKickCommand(string[] args, object[] optionalObjects)
		{
			if (args.Length == 1)
			{
				var player = (MujPlayer)optionalObjects[0];
				var GameServer = player.GameServer;


				ulong targetPlayerSteamId = GameServer.FindSteamIdByName(args[0], GameServer);
				if (!SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
					SteamIDKickVotes[targetPlayerSteamId] = 1; //set to 1 if it doesnt exist
				if (SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
				{
					SteamIDKickVotes[targetPlayerSteamId] += 1; // increment the value of the target player
					SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
					player.Message($"Vote To Kick {args[0]} Registered. Total Votes: {value}/{TotalVoteKicksNeeded}");
					GameServer.UILogOnServer($"Need {TotalVoteKicksNeeded - value} to kick {args[0]}", 5f);
					if (value == 1)
						GameServer.AnnounceShort($"A Vote Kick on {args[0]} has been initiated. Needs {TotalVoteKicksNeeded} Votes to kick");
					if (value >= 20)
					{
						GameServer.Kick(targetPlayerSteamId, "Vote Kicked");
						GameServer.UILogOnServer($"{args[0]} Has Been Kicked", 5f);
						SteamIDKickVotes.Remove(targetPlayerSteamId);
					}
				}
			}
			else
			{
				var player = (MujPlayer)optionalObjects[0];
				player.Message("Usage: !votekick <playername>");
			}
		}

		// !kill Callback
		public static void KillCommand(string[] args, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var GameServer = player.GameServer;

			if (!player.Stats.Roles.HasFlag(Roles.Moderator))
			{
				player.Message("Ur Not an admin");
				return;
			}

			if (args.Length == 1)
			{
				ulong steamid = GameServer.FindSteamIdByName(args[0], GameServer);
				GameServer.Kill(steamid);
				GameServer.AnnounceShort($"<color=red>{args[0]}</color> Has been Smited!!");
				return;
			}
			else
			{
				player.Message("Usage !kill <playername>");
			}
		}

		// !skipmap Callback
		public static void SkipMapCommand(string[] args, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var GameServer = player.GameServer;

			if (args.Length == 1)
			{
				if (args[0] == "trollflagon" && player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
					IsMapVoteTrollFlagOn = !IsMapVoteTrollFlagOn;

				if (args[0] == "mapnames")
				{
					StringBuilder sb = new StringBuilder();
					int count = 0;
					foreach (var mapName in MujUtils.stringToEnumMap.Keys)
					{
						if (count > 0 && count % 5 == 0)
							sb.Append(Environment.NewLine);

						sb.Append($"|{mapName}|");
						count++;
					}
					player.Message($"<b><i><color=red>Here is a list of the maps:</color></i></b>\n" + sb);
					return;
				}
			}
			if (args.Length == 2)
			{
				Maps MatchedMap = MujUtils.GetMapsEnumFromMapString(args[0]);
				MapDayNight MatchedMapDayNight = MujUtils.GetDayNightEnumFromString(args[1]);
				if (MatchedMap == Maps.None)
				{
					player.Message("Not a valid map. Type !skipmap mapnames to get a list of the maps");
					return;
				}
				if (IsMapVoteTrollFlagOn && MatchedMap == Maps.Lonovo && MatchedMapDayNight == MapDayNight.Night)
				{
					player.Kick("smh ╭∩╮(-_-)╭∩╮");
					return;
				}
				else
				{
					MapInfo MapInfo = new() { Map = MatchedMap, DayNight = MatchedMapDayNight };
					player.VotedMap = MapInfo;

					if (!MujApi.VoteMapList.ContainsKey(player))
					{
						MujApi.VoteMapList.Add(player, MapInfo);
						return;
					}
					else
					{
						player.Message("You Have Already Voted");
						return;
					}
				}
			}
			if (!IsVoteMapSkipAnnounced)
			{
				GameServer.AnnounceLong("A Skip Map Vote Has been initiated");
				IsVoteMapSkipAnnounced = true;
			}
			else
			{
				player.Message("this is used to skip the current map Usage: !skipmap <mapname> <day/night>");
				return;
			}
		}
	}
}