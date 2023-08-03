using BattleBitAPI.Common;
using BattleBitAPI.Server;
using MujAPI.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace MujAPI
{
    public class MujUtils
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MujUtils));

		private static Random random = new Random();

		//lowercase names to enums
		public static Dictionary<string, MujAPI.GameMaps> stringToEnumMap = new Dictionary<string, MujAPI.GameMaps>
		{
			{ "azagor", GameMaps.Azagor },
			{ "basra", GameMaps.Basra },
			{ "construction", GameMaps.Contruction },
			{ "district", GameMaps.District },
			{ "dustydew", GameMaps.DustyDew },
			{ "equardovo", GameMaps.Equardovo },
			{ "frugis", GameMaps.Frugis },
			{ "isle", GameMaps.Isle },
			{ "lonovo", GameMaps.Lonovo },
			{ "multuislands", GameMaps.MultuIslands },
			{ "namak", GameMaps.Namak },
			{ "oildunes", GameMaps.OilDunes },
			{ "river", GameMaps.River },
			{ "salhan", GameMaps.Salhan },
			{ "sandysunset", GameMaps.SandySunset },
			{ "tensatown", GameMaps.TensaTown },
			{ "valley", GameMaps.Valley },
			{ "wakistan", GameMaps.Wakistan },
			{ "wineparadise", GameMaps.WineParadise },
			{ "voxelland", GameMaps.VoxelLand },
		};
		public static Dictionary<string, GameMode> stringToEnumGameMode = new Dictionary<string, GameMode>
		{
			{ "tdm", GameMode.TDM },
			{ "aas", GameMode.AAS },
			{ "rush", GameMode.RUSH },
			{ "conquest", GameMode.CONQ },
			{ "domination", GameMode.DOMI },
			{ "elimination", GameMode.ELI },
			{ "infconq", GameMode.INFCONQ },
			{ "frontline", GameMode.FRONTLINE },
			{ "gungameffa", GameMode.GunGameFFA },
			{ "ffa", GameMode.FFA },
			{ "gungameteam", GameMode.GunGameTeam },
			{ "suiciderush", GameMode.SuicideRush },
			{ "catchgame", GameMode.CatchGame },
			{ "infected", GameMode.Infected },
			{ "cashrun", GameMode.CashRun },
			{ "voxelfortify", GameMode.VoxelFortify },
			{ "voxeltrench", GameMode.VoxelTrench },
			{ "ctf", GameMode.CTF },
		};
		public static Dictionary<string, MapDayNight> stringToEnumDayNight = new Dictionary<string, MapDayNight>
		{
			{ "day", MapDayNight.Day },
			{ "night", MapDayNight.Night },
		};
		public static List<string> RandomMOTD = new()
		{
			"Use <b><color=green>!votemap [mapnamehere]</b></color> to vote for maps!",
			"Use <b><color=green>!votekick [personnamehere]</b></color> to vote kick!",
			"Snipers Are banned on this server",
			"Dont forget to favourite this server!!",
		};



		/// <summary>
		/// used to make colour tags for server identifiers. maybe could be used for colourful text 🤔.
		/// </summary>
		/// <returns></returns>
		public static async Task<string> GetRandomColorAsync()
		{
			return await Task.Run(() =>
			{
				var random = new Random();
				return string.Format("#{0:X6}", random.Next(0x1000000));
			});
		}

		/// <summary>
		/// used to get the identifier of the server in paters XX#1++. <br/>1++ meaning infinite amount of numbers
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string GetServerIdentifier(GameServer server) 
		{
			return MujApi.GameServerIdentifiers.FirstOrDefault(kvp => kvp.Value == server).Key;
		}

		/// <summary>
		/// motd - used for tips etc in chat
		/// </summary>
		/// <param name="state"></param>
		public static async void SendMOTD(object state)
		{
			GameServer server = (GameServer)state;

			if (server == null)
			{
				log.Info("uh oh");
			}


			int randomIndex = random.Next(0, RandomMOTD.Count);

			string randomMOTD = RandomMOTD[randomIndex];

			server.SayToChat(randomMOTD);
		}

		/// <summary>
		/// displays the current time on the console title
		/// </summary>
		/// <param name="listener"></param>
		public static void SetConsoleTitle(ServerListener<MujPlayer> listener)
		{
			StringBuilder sb = new();

			int totalPlayers = listener.GetGameServers().Sum(server => server.CurrentPlayers);

			sb.Append($"{DateTime.UtcNow} | {listener.GetGameServers().Length} Servers Connected | Total Players Connected: {totalPlayers}");

			Console.Title = sb.ToString();
		}

		/// <summary>
		/// returns a map enum based on the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static GameMaps GetMapsEnumFromMapString(string input)
		{
			string lowercaseInput = input.ToLower();

			if (stringToEnumMap.TryGetValue(lowercaseInput, out GameMaps matchedMap))
			{
				return matchedMap;
			}

			return GameMaps.None;
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
		/// checks if the string of the map name exists as a enum
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsExistInMaps(string input)
		{
			string lowercaseInput = input.ToLower();

			if(stringToEnumMap.ContainsKey(input))
				return true;
			return false;
		}

		/// <summary>
		/// check if a string of a gamemode name exists as a enum
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsExistInGameModes(string input)
		{
			string lowercaseInput = input.ToLower();

			if (stringToEnumGameMode.ContainsKey(input))
				return true;
			return false;
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
			var groupedMapInfos = VoteMapList.GroupBy(kv => kv.Value).Select(group => new { Occurrences = group.Count() });

			int totalOccurances = groupedMapInfos.Sum(group => group.Occurrences);

			int maxOccurrences = groupedMapInfos.Max(group => group.Occurrences);

			return (totalOccurances, maxOccurrences);
		}

		/// <summary>
		/// used to create colored tags for cross server chat
		/// </summary>
		/// <param name="serverName"></param>
		/// <returns></returns>
		public static async Task<string> GetColoredIdentifierAsync(string serverName)
		{
			var ServerNameRegex = new Regex(@"[A-Z]{2}#\d+");

			string ServerIdentifier = ServerNameRegex.Match(serverName).Value;
			string RandomColour = await MujUtils.GetRandomColorAsync();

			return $"<color={RandomColour}>{ServerIdentifier}</color>";
		}

	}
}