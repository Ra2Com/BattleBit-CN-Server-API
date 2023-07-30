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
	}
}