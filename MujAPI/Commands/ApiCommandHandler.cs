using BattleBitAPI.Server;

namespace MujAPI.Commands
{
	public class ApiCommandHandler
    {
        //logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ApiCommandHandler));

        private readonly ServerListener<MujPlayer> listener;
        private Dictionary<string, Action<string[], object[]>> apiCommands { get; set; }

        /// <summary>
        /// this process user console input and checks for commands
        /// </summary>
        /// <remarks>
        /// <c>(MujPlayer)</c> class needs to be public <br/>
        /// </remarks>
        /// <param name="listener">the api listener</param>
        public ApiCommandHandler(ServerListener<MujPlayer> listener)
        {
            this.listener = listener;
			this.apiCommands = new Dictionary<string, Action<string[], object[]>>();
		}

        // add command
        public void AddCommand(string CommandName, Action<string[], object[]> CommandCallback)
        {
            apiCommands[CommandName.ToLower()] = CommandCallback ?? throw new ArgumentNullException(nameof(CommandName), "Callback cannot be null");
        }

        // execute command
        public void ExecuteCommand(string input, object[] objects)
        {
            string[] commandParts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string command = commandParts[0];
            string[] args = commandParts.Skip(1).ToArray();

			// remove multiple spaces like say  all      hello world
			string commandString = string.Join(" ", args).Trim();
			args = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (apiCommands.TryGetValue(command, out Action<string[], object[]> callback))
            {
                callback(args, objects); // call the call back
            }
            else
            {
                log.Error("Command not found");
            }

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

                object[] objects = new object[] { this.listener };

                ApiCommands.RegisterApiCommands(this, listener); // register commands

                ExecuteCommand(line, objects); // execute command
            }
        }

    }
}