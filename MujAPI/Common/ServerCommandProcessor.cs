using BattleBitAPI.Server;
using System.Net;
using System.Text;
using System.Linq;

namespace MujAPI
{
	public class ServerCommandProcessor
    {

        private readonly ServerListener<MyPlayer> listener;
        private readonly object lockObject = new object();

        /// <summary>
        /// this process user console input and checks for commands
        /// </summary>
        /// <remarks>
        /// <c>(MyPlayer)</c> class needs to be public <br/>
        /// </remarks>
        /// <param name="listener">the api listener</param>
        public ServerCommandProcessor(ServerListener<MyPlayer> listener)
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

						case "stopapi":
                            ShutdownAPI();
                            break;

                        case "test":
                            
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknown command. Type 'help' for available commands.\n");
                            Console.ResetColor();
                            break;
                    }
                }
            }
        }

		/// <summary>
		/// used to print the help message to the console
		/// </summary>
		public static void PrintHelp()
		{
			Console.WriteLine("The following commands are available:");
			Console.ForegroundColor = ConsoleColor.Green;

			var commands = new[]
			{
                ("shutdown <all|portnumber>", "shutdown all servers or specific server"),
                ("say <all|portnumber> <message>", "send a message to all servers or specific server"),
                ("listall", "lists all the servers connected to the api"),
                ("desc <serverport>", "shows a description of that server"),
                ("clear", "clears the console"),
                ("stopapi", "shuts down the api")
            };

			foreach (var (command, description) in commands)
			{
				Console.WriteLine($" {command,-32} - {description}");
			}

            Console.WriteLine();
			Console.ResetColor();
		}

		/// <summary>
		/// used to shutdown the api
		/// </summary>
		private void ShutdownAPI()
		{
            Console.ForegroundColor= ConsoleColor.Red;
            Console.WriteLine("Listener is being closed");
            listener.Stop();

            Console.WriteLine("Closing program");
            Console.ResetColor();
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
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Invalid usage. Type 'desc <portnumber>' to get a description of a server.\n" +
								"To get a list of your servers type 'listall'.\n");
							Console.ResetColor();
							break;
						}
						else
						{

							switch (PortNumber)
							{
                                // validate port min/max
								case < IPEndPoint.MinPort:
								case > IPEndPoint.MaxPort:
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine("Invalid port number\n");
									Console.ResetColor();
									return;
							}

							switch (listener.mActiveConnections.Count)
							{
                                // if there are no game servers connected
								case 0:
									Console.ForegroundColor = ConsoleColor.Red;
									Console.WriteLine("No Servers Found :(\n");
									Console.ResetColor();
									return;
								default:
									{
										foreach (var gameServer in listener.mActiveConnections.Values
                                            .Where(gameServer => PortNumber == gameServer.GamePort))
										{
											Console.ForegroundColor = ConsoleColor.Green;
											StringBuilder stringBuilder = new StringBuilder();
											stringBuilder.AppendLine($" Basic Server Info:{gameServer}");
											stringBuilder.AppendLine($" Current Map:{gameServer.Map}");
											stringBuilder.AppendLine($" Current MapDayNight:{gameServer.DayNight}");
											stringBuilder.AppendLine($" Current GameMode:{gameServer.Gamemode}");
											stringBuilder.AppendLine($" Current Players:{gameServer.CurrentPlayers}");
											stringBuilder.AppendLine($" Current Players In Queue:{gameServer.InQueuePlayers}");
											Console.WriteLine(stringBuilder);
											Console.ResetColor();
											return;
										}
										break;
									}
							}
						}
						break;
					}
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid usage. Type 'desc <portnumber>' to get a description of a server.\n" +
						"To get a list of your servers type 'listall'.\n");
					Console.ResetColor();
					return;
			}
		}

        /// <summary>
        /// prints a list of all the connected servers
        /// </summary>
        public void ListAllServers()
        {
			Console.ForegroundColor = ConsoleColor.Red;
			switch (listener.mActiveConnections.Count)
			{
                // if theres no game servers connected
				case 0:
					Console.WriteLine("No Servers Found :(\n");
					Console.ResetColor();
					return;
				default:
					{
						Console.ForegroundColor = ConsoleColor.Green;
						foreach (var gameServer in listener.mActiveConnections.Values)
						{
							Console.WriteLine(gameServer);
						}
						Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n" +
						"Use 'listall' to get all the servers\n");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n" +
					"Use 'listall' to get all the servers\n");
				Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid port number\n");
                Console.ResetColor();
                return;
            }

            bool foundServer = false;
            foreach (GameServer gameServer in listener.mActiveConnections.Values)
            {
                if (gameServer.GamePort == portNumber)
                {
                    gameServer.StopServer();
                    foundServer = true;
                }
            }

            if (foundServer)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Closed server on port:{portNumber}\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not find server with port:{portNumber}\n");
            }
            Console.ResetColor();
        }

        /// <summary>
        /// shuts down all servers
        /// </summary>
        public void ShutdownAllServers()
        {
            if (listener.mActiveConnections.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Servers Found :(\n");
                Console.ResetColor();
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (GameServer gameServer in listener.mActiveConnections.Values)
                {
                    Console.WriteLine(gameServer + " is shutting down");
                    gameServer.StopServer();
                }
                Console.WriteLine("all servers shutdown\n");
                Console.ResetColor();
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
						Console.ForegroundColor = ConsoleColor.Green;
						string message = string.Join(" ", args, 1, args.Length - 1);

						switch (whichServers)
						{
                            // second argument segment all or port
							case "all":
								{
									foreach (GameServer gameServer in listener.mActiveConnections.Values)
									{
										if (gameServer == null) continue;
										gameServer.SayToChat(message);
									}
									Console.WriteLine("Message Sent to servers: " + message + "\n");
									Console.ResetColor();
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
										Console.ForegroundColor = ConsoleColor.Red;
										Console.WriteLine("Invalid port number\n");
										Console.ResetColor();
										return;
								}

								//look for server with port number
								GameServer ChosenServer = listener.mActiveConnections
									.FirstOrDefault(server => server.Value.GamePort == PortNumber).Value;
								switch (ChosenServer)
								{
									case null:
										Console.ForegroundColor = ConsoleColor.Red;
										Console.WriteLine($"Could not find server with port:{PortNumber}\n");
										break;
									default:
										Console.ForegroundColor = ConsoleColor.Green;
										Console.WriteLine($"Message Sent to server: {ChosenServer} \n");
										break;
								}
								Console.ResetColor();
								break;
						}
						break;
					}
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Invalid usage. Type 'say <all|portnumber> <message>' to send a message.\n");
					Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    string message = string.Join(" ", args, 1, args.Length - 1);

                    if (whichServers == "all")
                    {
                        foreach (GameServer gameServer in listener.mActiveConnections.Values)
                        {
                            if (gameServer == null) continue;
                            gameServer.SayToChat(message);
                        }
                        Console.WriteLine("Message Sent to servers: " + message + "\n");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("An Error with cross server chat occured");
						Console.ResetColor();
						return;
                    }
                }
            }
		}
	}
}