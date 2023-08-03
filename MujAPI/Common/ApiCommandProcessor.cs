using BattleBitAPI.Server;
using System.Net;
using System.Text;
using log4net.Config;
using System.Net.Sockets;
using System.Reflection;

namespace MujAPI
{
	public class ApiCommandProcessor
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ApiCommandProcessor));


		private readonly ServerListener<MujPlayer> listener;
		private readonly object lockObject = new();

		/// <summary>
		/// this process user console input and checks for commands
		/// </summary>
		/// <remarks>
		/// <c>(MujPlayer)</c> class needs to be public <br/>
		/// </remarks>
		/// <param name="listener">the api listener</param>
		public ApiCommandProcessor(ServerListener<MujPlayer> listener)
		{
			this.listener = listener;
		}

		/// <summary>
		/// start the server command proccessor
		/// </summary>
		/// <remarks>
		/// <c>(mActiveConnections)</c> in <seealso cref="ServerListener{TPlayer}"/> needs to be public for the server commands to work <br/>
		/// </remarks>
		public void Start()
		{

			XmlConfigurator.Configure();

			while (true)
			{
				string line = Console.ReadLine();
				string[] commandParts = line?.Trim().ToLower().Split(' ');

				if (commandParts.Length > 0)
				{
					string command = commandParts[0];
					string[] args = commandParts.Skip(1).ToArray();

					// remove multiple spaces like say  all      hello world
					string commandString = string.Join(" ", args).Trim();
					args = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

					switch (command)
					{
						case "clear":
							Console.Clear();
							break;
						case "help":
							PrintHelp();
							break;
						case "shutdown":
							ShutdownServers(args);
							break;
						case "say":
							SayToServers(args);
							break;
						case "listall":
							ListAllServers();
							break;
						case "desc":
							DescribeServer(args);
							break;
						case "exit":
							ShutdownAPI();
							break;
						case "addservertest":
							TestGameServerConn();
							break;
						case "addtestplayers":
							AddTestPlayers(args);
							break;
						case "listplayers":
							ListAllPlayers();
							break;
						case "crosschat":
							EnableCrossServerChat();
							break;
						default:
							log.Error("Unknown command. Type 'help' for available commands.\n");
							break;
					}
				}
			}
		}

		/// <summary>
		/// adds tests players to servers
		/// </summary>
		/// <param name="args"></param>
		private void AddTestPlayers(string[] args)
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
							int ServerIndex = 0;

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

		/// <summary>
		/// lists all the players connected to each servers
		/// </summary>
		private void ListAllPlayers()
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

		/// <summary>
		/// toggleable flag that toggles cross server chat
		/// </summary>
		private void EnableCrossServerChat()
		{
			MujApi.IsAcrossServerChatOn = !MujApi.IsAcrossServerChatOn;
			var state = MujApi.IsAcrossServerChatOn ? "Enabled" : "Disabled";
			log.Info($"Cross Server Chat {state}");
		}

		/// <summary>
		/// used to test game server objects
		/// </summary>
		private async void TestGameServerConn()
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
				("!votekick"),
				("!votekick Test"),
				("!kill"),
				("!kill Test"),
				("!skipmap"),
				("!skipmap mapnames"),
				("!skipmap trollflagon"),
				("!skipmap azagor"),
				("!skipmap azagor day"),
				("!skipmap lonovo night"),
				("!bully nouser"),
				("!bully 2376438746"),
				("!bully help"),
				("!update"),
				("!update map"),
				("!update gamemode"),
				("!update map dustydew"), //add
				("!update map dustydew"), //remove
				("!update map tensatown"),
				("!update map valley"),
				("!update map wakistanasdghasjhdg"),
				("!update gamemode dom"),
				("!update gamemode dom"),
				("!update gamemode domination"),
			};

			foreach (var command in commands)
			{
				// simulates a chat event
				await MujApi.OnPlayerChat(mujPlayer1, BattleBitAPI.Common.ChatChannel.AllChat, command);
			}

			await MujApi.OnPlayerChat(mujPlayer2, BattleBitAPI.Common.ChatChannel.AllChat, "!votekick Test");

			foreach (var maps in mujPlayer1.GameServer.MapRotation.GetMapRotation())
			{
				log.Info(maps);
			}
		}

		/// <summary>
		/// used to print the help message to the console
		/// </summary>
		public static void PrintHelp()
		{
			Console.WriteLine("The following commands are available:");

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
				Console.WriteLine($" {command,-32} - {description}");
			}
		}

		/// <summary>
		/// used to shutdown the api
		/// </summary>
		private void ShutdownAPI()
		{
			log.Info("Listener is being closed");
			listener.Stop();

			log.Info("Closing program");
			Environment.Exit(0);
		}

		/// <summary>
		/// used to explain a certain server based on its portnumber
		/// </summary>
		/// <param name="args"></param>
		private void DescribeServer(string[] args)
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

		/// <summary>
		/// prints a list of all the connected servers
		/// </summary>
		public void ListAllServers()
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

		/// <summary>
		/// shuts down servers
		/// </summary>
		/// <param name="args"></param>
		public void ShutdownServers(string[] args)
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

		/// <summary>
		/// shuts down a server based on its port
		/// </summary>
		/// <param name="portNumber"></param>
		public void ShutdownServersByPort(int portNumber)
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
			Console.ResetColor();
		}

		/// <summary>
		/// shuts down all servers
		/// </summary>
		public void ShutdownAllServers()
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

		/// <summary>
		/// sends a message to all or a specific server(s)
		/// </summary>
		/// <param name="args"></param>
		public void SayToServers(string[] args)
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


		/// <summary>
		/// sends a message to all servers used for already filled in "say" <customtext>
		/// </summary>
		/// <param name="args"></param>
		public void SendChatMessageToAllServers(string[] args)
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