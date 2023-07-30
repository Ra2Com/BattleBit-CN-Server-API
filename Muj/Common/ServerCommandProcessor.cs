using BattleBitAPI.Server;
using System.Net;

namespace CommunityServerAPI.Muj.Common
{
	public class ServerCommandProcessor
    {

        private readonly ServerListener<MyPlayer> listener;

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

                        case "listallservers":
                            ListAllServers();
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

        public static void PrintHelp()
        {
            Console.WriteLine("The following commands are available:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                " shutdown <all | portnumber> - shutdown all servers or specific server\n" +
                " say <message> - send a message to all servers\n" +
                " listallservers - lists all the servers connected to the api\n" +
                " clear - clears the console\n"
                );
            Console.ResetColor();
        }

        public void ListAllServers()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (listener.mActiveConnections.Count == 0)
            {
                Console.WriteLine("No Servers Found :(\n");
                Console.ResetColor();
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (GameServer gameServer in listener.mActiveConnections.Values)
                {
                    Console.WriteLine(gameServer);
                }
                Console.ResetColor();
            }
        }

        public void ShutdownServers(string[] args)
        {
            if (args.Length > 0)
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
                    Console.WriteLine("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid usage. Type 'shutdown all' to shutdown all servers or 'shutdown <portnumber>' to shutdown a specific server.\n");
                Console.ResetColor();
            }
        }

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

        public void SayToServers(string[] args)
        {
            if (args.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string message = string.Join(" ", args);
                Console.WriteLine("Message Sent to servers: " + message + "\n");
                foreach (GameServer gameServer in listener.mActiveConnections.Values)
                {
                    if (gameServer == null) continue;
                    gameServer.SayToChat(message);
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid usage. Type 'say <message>' to send a message.\n");
                Console.ResetColor();
            }
        }
    }
}