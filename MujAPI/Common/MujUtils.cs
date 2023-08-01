using BattleBitAPI.Common;
using BattleBitAPI.Server;
using MujAPI.Commands;

namespace MujAPI
{
    public class MujUtils : ChatCommands
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CommandProcessor));

		//lowercase names to enums
		public static Dictionary<string, Maps> stringToEnumMap = new Dictionary<string, Maps>
			{
				{ "azagor", Maps.Azagor },
				{ "basra", Maps.Basra },
				{ "construction", Maps.Contruction },
				{ "district", Maps.District },
				{ "dustydew", Maps.DustyDew },
				{ "equardovo", Maps.Equardovo },
				{ "frugis", Maps.Frugis },
				{ "isle", Maps.Isle },
				{ "lonovo", Maps.Lonovo },
				{ "multuislands", Maps.MultuIslands },
				{ "namak", Maps.Namak },
				{ "oildunes", Maps.OilDunes },
				{ "river", Maps.River },
				{ "salhan", Maps.Salhan },
				{ "sandysunset", Maps.SandySunset },
				{ "tensatown", Maps.TensaTown },
				{ "valley", Maps.Valley },
				{ "wakistan", Maps.Wakistan },
				{ "wineparadise", Maps.WineParadise },
				{ "voxelland", Maps.VoxelLand },
			};
		public static Dictionary<string, MapDayNight> stringToEnumDayNight = new Dictionary<string, MapDayNight>
			{
				{ "day", MapDayNight.Day },
				{ "night", MapDayNight.Night },
			};


		/// <summary>
		/// motd
		/// </summary>
		/// <param name="state"></param>
		public static async void SendMessageEveryFiveMinutes(object state)
		{
			GameServer server = (GameServer)state;

			if (server == null)
			{
				log.Info("uh oh");
			}

			server.SayToChat(
				"Use <b><color=green>!votemap [mapnamehere]</b></color> to vote for maps!\n" +
				"Use <b><color=green>!votekick [personnamehere]</b></color> to vote kick!");
		}

		/// <summary>
		/// displays the current time on the console title
		/// </summary>
		/// <param name="state"></param>
		public static void SetConsoleTitleAsTime(object state)
		{
			Console.Title = DateTime.UtcNow.ToString();
		}

		/// <summary>
		/// returns a map enum based on the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Maps GetMapsEnumFromMapString(string input)
		{
			string lowercaseInput = input.ToLower();

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
		public static MapDayNight GetDayNightEnumFromString(string input)
		{
			string lowercaseInput = input.ToLower();

			if (stringToEnumDayNight.TryGetValue(lowercaseInput, out MapDayNight matchedDayNight))
			{
				return matchedDayNight;
			}

			return MapDayNight.Day;
		}

		/// <summary>
		/// gets the map with the highest ammount of votes
		/// </summary>
		/// <param name="VoteMapList"></param>
		/// <returns>MapInfo</returns>
		public static MapInfo GetMapInfoWithHighestOccurrences(Dictionary<MujPlayer, MapInfo> VoteMapList)
		{
			var groupedMapInfos = VoteMapList.GroupBy(kv => kv.Value).Select(group => new { MapInfo = group.Key, Occurrences = group.Count() });

			var mapInfoWithMaxOccurrences = groupedMapInfos.OrderByDescending(group => group.Occurrences).FirstOrDefault();

			return mapInfoWithMaxOccurrences?.MapInfo;
		}


		/// <summary>
		/// used to grab the max and total occurances of voted maps
		/// </summary>
		/// <param name="VoteMapList"></param>
		/// <returns>totalOccurances, maxOccurrences</returns>
		public static (int TotalOccurances, int MaxOccurances) GetOccurances(Dictionary<MujPlayer, MapInfo> VoteMapList)
		{
			var groupedMapInfos= VoteMapList.GroupBy(kv => kv.Value).Select(group => new { Occurrences = group.Count() });

			int totalOccurances = groupedMapInfos.Sum(group => group.Occurrences);

			int maxOccurrences = groupedMapInfos.Max(group => group.Occurrences);

			return (totalOccurances, maxOccurrences);
		}

	}
}