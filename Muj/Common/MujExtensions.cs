using BattleBitAPI.Common.Enums;
using BattleBitAPI.Server;

namespace CommunityServerAPI.Muj.Common
{
	public class MujExtentions
	{
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
						player.GameServer.Kick(player.GameServer.FindSteamIdByName(args[0]), args[1]);
						break;
					case "kill":
						player.GameServer.Kill(player.GameServer.FindSteamIdByName(args[0]));
						break;
					case "votemap":
						if (args.Length < 2) 
						{
							player.GameServer.MessageToPlayer(player, "Please Specify Which Map you want to use");
						}
						else
						{
							Maps matchedMap = GetMappedEnum(args[0]);
							if (matchedMap == Maps.None)
							{
								player.GameServer.MessageToPlayer(player, "Not a valid map");
								break;
							}
							if (!player.votedMaps.ContainsKey(player))
							{
								player.votedMaps.Add(player, matchedMap);
								break;
							}
							break;
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

		public static Maps GetMappedEnum(string input)
		{
			string lowercaseInput = input.ToLower();

			var stringToEnumMap = new Dictionary<string, Maps>
			{
				{ "azagor", Maps.Azagor },
				{ "tensatown", Maps.TensaTown },
        };

			if (stringToEnumMap.TryGetValue(lowercaseInput, out Maps matchedMap))
			{
				return matchedMap;
			}

			return Maps.None;
		}
	}
}