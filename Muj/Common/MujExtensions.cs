using BattleBitAPI.Common.Enums;
using BattleBitAPI.Common;
using BattleBitAPI.Server;

namespace CommunityServerAPI.Muj.Common
{
	public class MujExtentions
	{
		public static bool IsVoteSkipAnnounced = false;

		public static void HandleChatCommand(MyPlayer player, string msg)
		{
			string[] commandParts = msg.Trim().ToLower().Split(" ");
			if (commandParts.Length > 0)
			{
				string command = commandParts[0].Substring(1);
				string[] args = commandParts.Skip(1).ToArray();
				switch (command)
				{
					case "votekick":
						if (args.Length == 1)
						{
							player.GameServer.MessageToPlayer(player, "Usage: !votekick <playername>");
							break;
						}
						else
						{ 
							player.GameServer.Kick(player.GameServer.FindSteamIdByName(args[0], player.GameServer), "votekicked!");
							break;
						}
					case "kill":
						if (args.Length == 1)
						{
							player.GameServer.MessageToPlayer(player, "Usage !kill <playername>");
							break;
						}
						if (!player.Stats.Roles.HasFlag(Roles.Admin | Roles.Moderator))
						{
							player.GameServer.MessageToPlayer(player, "Ur Not an admin");
							break;
						}
						else
						{
							ulong steamid = player.GameServer.FindSteamIdByName((string)args[0], player.GameServer);
							player.GameServer.Kill(steamid);
							player.GameServer.AnnounceShort($"<color=red>{args[0]}</color> Has been Smited!!");
							break;
						}
					case "skipmap":
						if (args.Length == 2) 
						{
							player.GameServer.MessageToPlayer(player, "this is used to skip the current map Usage: !skipmap <mapname> <day/night>");
							break;
						}
						if (!IsVoteSkipAnnounced)
						{
							player.GameServer.AnnounceLong("A Skip Map Vote Has been initiated");
							IsVoteSkipAnnounced = true;
						}
						else
						{
							Maps MatchedMap = GetMapsEnum(args[0]);
							MapDayNight MatchedMapDayNight = GetDayNightEnum(args[1]);
							if (MatchedMap == Maps.None)
							{
								player.GameServer.MessageToPlayer(player, "Not a valid map");
								break;
							}
							if (MatchedMap == Maps.Lonovo && MatchedMapDayNight == MapDayNight.Night)
							{
								player.Kick("smh ╭∩╮(-_-)╭∩╮");
								break;
							}
							else
							{
								MapInfo MapInfo = new() { Map = MatchedMap, DayNight = MatchedMapDayNight};
								player.votedMap = MapInfo;
								break;
							}
						}
						break;

					default:
						player.GameServer.MessageToPlayer(player, "Invalid Usage of commands");
						break;
				}
			}
		}

		public static async void SendMessageEveryFiveMinutes(object state)
		{
			GameServer server = (GameServer)state;

			if (server == null)
			{
				await Console.Out.WriteLineAsync("uh oh");
			}

			server.SayToChat(
				"Use <b><color=green>!votemap [mapnamehere]</b></color> to vote for maps!\n" +
				"Use <b><color=green>!votekick [personnamehere]</b></color> to vote kick!");
		}

		//console title task
		public static void SetConsoleTitleAsTime(object state)
		{
			Console.Title = DateTime.UtcNow.ToString();
		}

		/// <summary>
		/// returns a map enum based on the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Maps GetMapsEnum(string input)
		{
			string lowercaseInput = input.ToLower();

			var stringToEnumMap = new Dictionary<string, Maps>
			{
				{ "azagor", Maps.Azagor },
				{ "tensatown", Maps.TensaTown },
				{ "lonovo", Maps.Lonovo }
        };

			if (stringToEnumMap.TryGetValue(lowercaseInput, out Maps matchedMap))
			{
				return matchedMap;
			}

			return Maps.None;
		}

		/// <summary>
		/// returns a day/night enum based on the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static MapDayNight GetDayNightEnum(string input)
		{
			string lowercaseInput = input.ToLower();

			var stringToEnumMap = new Dictionary<string, MapDayNight>
			{
				{ "day", MapDayNight.Day },
				{ "night", MapDayNight.Night },
		};

			if (stringToEnumMap.TryGetValue(lowercaseInput, out MapDayNight matchedDayNight))
			{
				return matchedDayNight;
			}

			return MapDayNight.Day;
		}

		public static MapInfo GetMapInfoWithHighestOccurrences(Dictionary<MyPlayer, MapInfo> VoteMapList)
		{
			var groupedMapInfos = VoteMapList.GroupBy(kv => kv.Value)
											 .Select(group => new { MapInfo = group.Key, Occurrences = group.Count() });

			var mapInfoWithMaxOccurrences = groupedMapInfos.OrderByDescending(group => group.Occurrences)
														  .FirstOrDefault();

			return mapInfoWithMaxOccurrences?.MapInfo;
		}


		/// <summary>
		/// used to grab the max and total occurances of voted maps
		/// </summary>
		/// <param name="VoteMapList"></param>
		/// <returns>totalOccurances, maxOccurrences</returns>
		public static (int TotalOccurances, int MaxOccurances) GetOccurances(Dictionary<MyPlayer, MapInfo> VoteMapList)
		{
			var groupedMapInfos= VoteMapList.GroupBy(kv => kv.Value)
											 .Select(group => new { Occurrences = group.Count() });

			int totalOccurances = groupedMapInfos.Sum(group => group.Occurrences);

			int maxOccurrences = groupedMapInfos.Max(group => group.Occurrences);

			return (totalOccurances, maxOccurrences);
		}

	}
}