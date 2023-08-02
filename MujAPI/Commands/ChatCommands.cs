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
		public static int TotalVoteKicksNeeded = 20;
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

				//helper function to handle votes
				void HandleVotes(int votes)
				{
					// sends message to player and a uilog on the server
					player.Message($"Vote To Kick Player:({args[0]}) Registered. Total Votes: {votes}/{TotalVoteKicksNeeded}");
					GameServer.UILogOnServer($"Need ({TotalVoteKicksNeeded - votes}) to kick player:({args[0]})", 5f);
					
					//if there is 1 vote then an announcement is made
					if (votes == 1)
					{
						GameServer.AnnounceShort($"A Vote Kick on Player:({args[0]}) has been initiated. Needs {TotalVoteKicksNeeded} Votes to kick");
						return;
					}

					//if there are 20 or more votes then that player is kicked
					if (votes >= 20)
					{
						GameServer.Kick(targetPlayerSteamId, "Vote Kicked");
						GameServer.UILogOnServer($"({args[0]}) Has Been Kicked", 5f);
						SteamIDKickVotes.Remove(targetPlayerSteamId);
					}
				}

				if (!SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
				{
					//set to 1 if it doesnt exist
					SteamIDKickVotes[targetPlayerSteamId] = 1;
					SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
					HandleVotes(value);
					return;
				}
				if (SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
				{
					// increment the value of the target player
					SteamIDKickVotes[targetPlayerSteamId] += 1; 
					SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
					HandleVotes(value);
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

			//checks if player is mod or admin
			if (!player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
			{
				player.Message("Ur Not an admin");
				return;
			}
			//if trusted player is passed then kill command is issued 
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

			// looks for 1 argument
			if (args.Length == 1)
			{
				// flag that can only be enabled by admin or mod
				if (args[0] == "trollflagon" && player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
					IsMapVoteTrollFlagOn = !IsMapVoteTrollFlagOn;

				// returns a list of the maps available to vote
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

			//looks for 2 arguments
			if (args.Length == 2)
			{
				Maps MatchedMap = MujUtils.GetMapsEnumFromMapString(args[0]);
				MapDayNight MatchedMapDayNight = MujUtils.GetDayNightEnumFromString(args[1]);
				// sends error message to user if they dont input a valid map name
				if (MatchedMap == Maps.None)
				{
					player.Message("Not a valid map. Type !skipmap mapnames to get a list of the maps");
					return;
				}
				// kicks the player for choosing lonovo night
				if (IsMapVoteTrollFlagOn && MatchedMap == Maps.Lonovo && MatchedMapDayNight == MapDayNight.Night)
				{
					player.Kick("smh ╭∩╮(-_-)╭∩╮");
					return;
				}
				else
				{
					MapInfo MapInfo = new() { Map = MatchedMap, DayNight = MatchedMapDayNight };
					player.VotedMap = MapInfo;
					//adds the user to a votemaplist
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
			//if (!IsVoteMapSkipAnnounced)
			//{
			//	//accounces to the game a map skip has been initiated
			//	GameServer.AnnounceLong("A Skip Map Vote Has been initiated");
			//	IsVoteMapSkipAnnounced = true;
			//}
			else
			{
				player.Message("this is used to skip the current map Usage: !skipmap <mapname> <day/night>");
				return;
			}
		}
	}
}