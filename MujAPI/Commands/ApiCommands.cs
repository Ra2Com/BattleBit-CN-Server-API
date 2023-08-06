using BattleBitAPI.Server;
using MujAPI.Common.Database;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace MujAPI.Commands
{
	public class ApiCommands
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ChatCommands));

		private static readonly object lockObject = new();

		private static ServerListener<MujPlayer> listener;

		public static void RegisterApiCommands(ApiCommandHandler apiCommandHandler, ServerListener<MujPlayer> serverListener)
		{
			listener = serverListener ?? throw new ArgumentNullException(nameof(serverListener));

			apiCommandHandler.AddCommand("clear", ClearConsoleCommand);
			apiCommandHandler.AddCommand("help", PrintHelp);
			apiCommandHandler.AddCommand("shutdown", ShutdownServer);
			apiCommandHandler.AddCommand("say", SayToServers);
			apiCommandHandler.AddCommand("listall", ListAllServers);
			apiCommandHandler.AddCommand("desc", DescribeServer);
			apiCommandHandler.AddCommand("exit", ShutdownAPI);
			apiCommandHandler.AddCommand("addservertest", TestGameServerConn);
			apiCommandHandler.AddCommand("addtestplayers", AddTestPlayers);
			apiCommandHandler.AddCommand("listplayer", ListAllPlayers);
			apiCommandHandler.AddCommand("dbtest", DBTest);
			apiCommandHandler.AddCommand("crosschat", EnableCrossServerChat);
		}

		// add test server callback
		private static void AddTestPlayers(string[] args, object[] objects)
		{
			int TotalPlayersAdded = 0;
			switch (args.Length)
			{
				case 1:
					{
						if (!int.TryParse(args[0], out int NumberOfPlayers) || NumberOfPlayers == 254 * 16)
						{
							log.Error("Too Many Players\n");
							break;
						}
						else
						{
							int CurrentPlayerCount;
							int MaxPlayerCount;

							foreach (var server in listener.GetGameServers())
							{

								PropertyInfo currentPlayerProperty = server.GetType().GetProperty("CurrentPlayers", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

								if (currentPlayerProperty != null)
								{
									CurrentPlayerCount = server.CurrentPlayers;
									MaxPlayerCount = server.MaxPlayers;

									int AvailableSlots = MaxPlayerCount - CurrentPlayerCount;

									int PlayersToAdd = Math.Min(NumberOfPlayers - TotalPlayersAdded, AvailableSlots);

									currentPlayerProperty.SetValue(server, CurrentPlayerCount += PlayersToAdd);

									TotalPlayersAdded += PlayersToAdd;

									if (TotalPlayersAdded >= NumberOfPlayers)
										break;
								}
								else
								{
									log.Error("couldnt get CurrentPlayers");
									break;
								}
							}
						}
					}
					break;
				default:
					log.Error("Invalid Usage");
					break;
			}
		}

		// dbtest callback
		private static async void DBTest(string[] args, object[] objects)
		{
			var ServerIP = IPAddress.Parse("234.123.24.54");
			try
			{
				var AddGameServerResult = MujDBConnection.DBAddGameServer("TestServer", ServerIP.ToString(), 20000);
				if (AddGameServerResult != null)
				{
					var AddGameServerDesc = AddGameServerResult
						.Select(sv => $"DBID={sv.GameServerId}:{sv.ServerName}, {sv.IPAddress}, {sv.Port}, {sv.CreatedAt}: STATUS={sv.Status}")
						.ToList();

					log.Info($"GameServer Added To DB: {string.Join(", ", AddGameServerDesc)}");
				}
				else
				{
					log.Error("Failed to add the game server.");
				}

				// Changing Server Status
				var ChangeStatusResult = MujDBConnection.DBUpdateServerStatus(ServerIP.ToString(), 20000, "Maintenance");
				if (ChangeStatusResult != null)
				{
					var StatusDesc = ChangeStatusResult
						.Select(s => $"Current Status:{s.Status} For: {s.ServerName}:{s.IPAddress}:{s.Port}")
						.ToList();

					log.Info(string.Join(", ", StatusDesc));
				}
				else
				{
					log.Error("Failed to update server status.");
				}
			}
			catch (Exception ex)
			{
				log.Error($"An error occurred: {ex.Message}");
			}

		}

		// shutdown servers callback
		public static void ShutdownServer(string[] args, object[] objects)
		{
			if (args.Length == 1)
			{
				string whichServers = args[0];

				if (whichServers == "all")
				{
					ShutdownAllServers();
				}
				else if (int.TryParse(whichServers, out int portNumber))
				{
					ShutdownServersByPort(portNumber);
				}
				else
				{
					log.Error("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n" +
						"Use 'listall' to get all the servers\n");
				}
			}
			else
			{
				log.Error("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n" +
					"Use 'listall' to get all the servers\n");
			}
		}

		// shutdown all servers
		public static void ShutdownAllServers()
		{
			if (listener.GetGameServers().Length == 0)
			{
				log.Error("No Servers Found :(\n");
				return;
			}
			else
			{
				foreach (GameServer gameServer in listener.GetGameServers())
				{
					log.Info(gameServer + " is shutting down");
					gameServer.StopServer();
				}
				log.Info("all servers shutdown\n");
			}
		}

		// shutdown by port number
		public static void ShutdownServersByPort(int portNumber)
		{
			if (portNumber < IPEndPoint.MinPort || portNumber > IPEndPoint.MaxPort)
			{
				log.Error("Invalid port number\n");
				return;
			}

			bool foundServer = false;
			foreach (GameServer gameServer in listener.GetGameServers())
			{
				if (gameServer.GamePort == portNumber)
				{
					gameServer.StopServer();
					foundServer = true;
				}
			}

			if (foundServer)
			{
				log.Info($"Closed server on port:{portNumber}\n");
			}
			else
			{
				log.Error($"Could not find server with port:{portNumber}\n");
			}
		}

		// list all servers call back
		public static void ListAllServers(string[] args, object[] objects)
		{
			switch (listener.GetGameServers().Length)
			{
				// if theres no game servers connected
				case 0:
					log.Error("No Servers Found :(\n");
					return;
				default:
					{
						foreach (var gameServer in listener.GetGameServers())
						{
							log.Info(gameServer);
						}
						break;
					}
			}
		}

		// describe server callback
		private static void DescribeServer(string[] args, object[] objects)
		{
			switch (args.Length)
			{
				//check first argument
				case 1:
					{
						if (!int.TryParse(args[0], out int PortNumber) || PortNumber == 0)
						{
							log.Error("Invalid usage. Type 'desc <portnumber>' to get a description of a server.\n" +
								"To get a list of your servers type 'listall'.\n");
							break;
						}
						else
						{

							switch (PortNumber)
							{
								// validate port min/max
								case < IPEndPoint.MinPort:
								case > IPEndPoint.MaxPort:
									log.Info("Invalid port number\n");
									return;
							}

							switch (listener.GetGameServers().Length)
							{
								// if there are no game servers connected
								case 0:
									log.Info("No Servers Found :(\n");
									return;
								default:
									{
										// write description of chosen server
										foreach (var gameServer in listener.GetGameServers()
											.Where(gameServer => PortNumber == gameServer.GamePort))
										{
											StringBuilder stringBuilder = new();
											stringBuilder.AppendLine($" Server Info:{gameServer}");
											stringBuilder.AppendLine($" Map:{gameServer.Map}");
											stringBuilder.AppendLine($" MapDayNight:{gameServer.DayNight}");
											stringBuilder.AppendLine($" GameMode:{gameServer.Gamemode}");
											stringBuilder.AppendLine($" Players:{gameServer.CurrentPlayers}");
											stringBuilder.AppendLine($" Players In Queue:{gameServer.InQueuePlayers}");
											log.Info(stringBuilder);
											return;
										}
										break;
									}
							}
						}
						break;
					}
				default:
					log.Error("Invalid usage. Type 'desc <portnumber>' to get a description of a server.\n" +
						"To get a list of your servers type 'listall'.\n");
					return;
			}
		}

		// shutdown api callback
		private static void ShutdownAPI(string[] args, object[] objects)
		{
			log.Info("Listener is being closed");
			listener.Stop();

			log.Info("Closing program");
			Environment.Exit(0);
		}

		// test servers and command callback
		private static async void TestGameServerConn(string[] args, object[] objects)
		{
			//creates server 1
			TcpClient tcpClient1 = new();
			GameServer.mInternalResources mInternalResources1 = new();
			GameServer server1 = new(tcpClient1, mInternalResources1, null, IPAddress.Loopback,
				30000, true, "EU#1", "CONQ", "azagor", BattleBitAPI.Common.MapSize._127vs127,
				BattleBitAPI.Common.MapDayNight.Day, 20, 2, 254, null, null);

			//creates server 2
			TcpClient tcpClient2 = new();
			GameServer.mInternalResources mInternalResources2 = new();
			GameServer server2 = new(tcpClient2, mInternalResources2, null, IPAddress.Loopback,
				30022, true, "EU#2", "INFECTED", "tensatown", BattleBitAPI.Common.MapSize._127vs127,
				BattleBitAPI.Common.MapDayNight.Day, 20, 2, 254, null, null);

			//adds the servers to the mActiveConnections list
			listener.AddGameServer(server1);
			listener.AddGameServer(server2);

			// makes the ongameserverconnected run
			await MujApi.OnGameServerConnected(server1);
			await MujApi.OnGameServerConnected(server2);


			//creates first player and gives admin and mod
			MujPlayer mujPlayer1 = new(ulong.Parse("783264326438"));
			mujPlayer1.Name = "Muj";
			mujPlayer1.GameServer = server1;
			mujPlayer1.Stats = new BattleBitAPI.Common.PlayerStats
			{
				Roles = BattleBitAPI.Common.Roles.Admin | BattleBitAPI.Common.Roles.Moderator
			};
			log.Info(mujPlayer1);

			//creates second player
			MujPlayer mujPlayer2 = new(ulong.Parse("324983274987"));
			mujPlayer2.Name = "Test";
			mujPlayer2.GameServer = server1;
			log.Info(mujPlayer2);

			//adds the players to the internal resources
			mInternalResources1.AddPlayer(mujPlayer1);
			mInternalResources1.AddPlayer(mujPlayer2);

			// tests each command
			var commands = new[]
			{
				"!testcommand",
				"!votekick",
				"!votekick Test",
				"!kill",
				"!kill Test",
				"!skipmap",
				"!skipmap mapnames",
				"!skipmap trollflagon",
				"!skipmap azagor",
				"!skipmap azagor day",
				"!skipmap lonovo night",
				"!bully nouser",
				"!bully 2376438746",
				"!bully help",
				"!update",
				"!update map",
				"!update gamemode",
				"!update map dustydew", //add
				"!update map dustydew", //remove
				"!update map tensatown",
				"!update map valley",
				"!update map wakistanasdghasjhdg",
				"!update gamemode dom",
				"!update gamemode dom",
				"!update gamemode domination",
				"!gamerule banweapon m4a1",
				"!gamerule banweapon mp9",
				"!gamerule unbanweapon m4a1",
				"!gamerule unbanweapon",
				"!gamerule",
				"!gamerule banwearings",
				"!gamerule unbanwearings",
			};

			foreach (var command in commands)
			{
				// simulates a chat event
				//await Task.Delay(1000);
				await MujApi.OnPlayerChat(mujPlayer1, BattleBitAPI.Common.ChatChannel.AllChat, command);
			}

			await MujApi.OnPlayerChat(mujPlayer2, BattleBitAPI.Common.ChatChannel.AllChat, "!votekick Test");

		}

		// cross chat callback
		private static void EnableCrossServerChat(string[] args, object[] objects)
		{
			MujApi.IsAcrossServerChatOn = !MujApi.IsAcrossServerChatOn;
			var state = MujApi.IsAcrossServerChatOn ? "Enabled" : "Disabled";
			log.Info($"Cross Server Chat {state}");
		}

		// list all players callback
		private static void ListAllPlayers(string[] args, object[] objects)
		{
			if (listener.GetGameServers().Length == 0)
			{
				log.Error("no servers");
				return;
			}

			foreach (var server in listener.GetGameServers())
			{
				var allplayers = server.GetAllPlayers();
				foreach (var player in allplayers)
				{
					if (player != null)
					{
						log.Info($"{player} : {server.ServerName}");
					}
				}
			}
		}

		// print help callback
		private static void PrintHelp(string[] args, object[] objects)
		{
			log.Info("The following commands are available:");

			var commands = new[]
			{
				("shutdown <all|portnumber>", "shutdown all servers or specific server"),
				("say <all|portnumber> <message>", "send a message to all servers or specific server"),
				("listall", "lists all the servers connected to the api"),
				("listplayers", "lists every player connected to each gameserver that connected to this api"),
				("desc <serverport>", "shows a description of that server"),
				("clear", "clears the console"),
				("exit", "shuts down the api"),
				("crosschat", "Toggle Cross Server Chat"),
				("addservertest", "DONOTUSE - used to test gameserver objects being added to server"),
				("addtestplayers","DONOTUSE - used to add test players to gameserver objects")
			};

			foreach (var (command, description) in commands)
			{
				log.Info($" {command,-32} - {description}");
			}
		}

		// clear console callback
		private static void ClearConsoleCommand(string[] args, object[] objects)
		{
			Console.Clear();
		}

		// say to servers callback
		public static void SayToServers(string[] args, object[] objects)
		{
			switch (args.Length)
			{
				// more than or equal to 2 args
				case >= 2:
					{
						string whichServers = args.Length >= 1 ? args[0].Trim() : string.Empty;
						string message = string.Join(" ", args, 1, args.Length - 1);

						switch (whichServers)
						{
							// second argument segment all or port
							case "all":
								{
									foreach (GameServer gameServer in listener.GetGameServers())
									{
										if (gameServer == null) continue;
										gameServer.SayToChat(message);
									}
									log.Info("Message Sent to servers: " + message + "\n");
									break;
								}

							default:
								if (!int.TryParse(whichServers, out int PortNumber))
								{
									break;
								}
								switch (PortNumber)
								{
									// check if port passes min and max checks
									case < IPEndPoint.MinPort:
									case > IPEndPoint.MaxPort:
										log.Error("Invalid port number\n");
										return;
								}

								//look for server with port number
								GameServer ChosenServer = listener.GetGameServers()
									.FirstOrDefault(server => server.GamePort == PortNumber);
								switch (ChosenServer)
								{
									case null:
										log.Error($"Could not find server with port:{PortNumber}\n");
										break;
									default:
										ChosenServer.SayToChat(message);
										log.Info($"({message}) Sent to server: {ChosenServer} \n");
										break;
								}
								Console.ResetColor();
								break;
						}
						break;
					}
				default:
					log.Error("Invalid usage. Type 'say <all|portnumber> <message>' to send a message.\n");
					break;
			}
		}

		// send chat to all servers callback
		public static void SendChatMessageToAllServers(string[] args, object[] objects)
		{
			lock (lockObject)
			{
				if (args.Length >= 2)
				{
					// remove multiple spaces like say  all      hello world
					string commandString = string.Join(" ", args).Trim();
					args = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
					string whichServers = args[0];
					string message = string.Join(" ", args, 1, args.Length - 1);

					if (whichServers == "all")
					{
						foreach (GameServer gameServer in listener.GetGameServers())
						{
							if (gameServer == null) continue;
							gameServer.SayToChat(message);
						}
						log.Info("Message Sent to servers: " + message + "\n");
					}
					else
					{
						log.Error("An Error with cross server chat occured");
						return;
					}
				}
			}
		}
	}
}
