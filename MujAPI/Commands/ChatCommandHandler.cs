using BattleBitAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MujAPI
{
	public class ChatCommandHandler
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ChatCommandHandler));


		private Dictionary<string, Action<string[], object[]>> commands { get; set; }

		public ChatCommandHandler()
		{
			this.commands = new Dictionary<string, Action<string[], object[]>>();
		}

		/// <summary>
		/// used to add chat commands
		/// </summary>
		/// <param name="name"></param>
		/// <param name="callback"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public void AddCommand(string name, Action<string[], object[]> callback)
		{
			commands[name.ToLower()] = callback ?? throw new ArgumentNullException(nameof(callback), "Callback cannot be null");
		}

		/// <summary>
		/// executes chat command 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="optionalObjects"></param>
		public void ExecuteCommand(string input, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var chatChannel = (ChatChannel)optionalObjects[1];
			
			string[] commandParts = input.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

			// this is just used to remove the color tags from the server names.
			// i use UK#1 so two capital letters a hashtag and then any amount of numbers
			var ServerRegex = new Regex(@"[A-Z]{2}#\d+");
			string ServerIdentifier = ServerRegex.Match(player.GameServer.ServerName).Value;

			StringBuilder sb = new();
			sb.Append($"Command Issued by ({ServerIdentifier}) {player}:{chatChannel.ToString()}: ");
			foreach ( var commandPart in commandParts )
			{
				sb.Append($" [{commandPart}]");
			}


			if (commandParts.Length == 0)
			{
				player.Message("Invalid Command");
				return;
			}
			
			string commandName = commandParts[0].Substring(0).ToLower(); // the command
			string[] args = commandParts.Skip(1).ToArray(); // the arguments

			if (commands.TryGetValue(commandName, out Action<string[], object[]> callback))
			{
				callback(args, optionalObjects); // call the call back
				log.Debug( sb.ToString() );
			}
			else
			{
				player.Message("Command not found");
			}

		}
	}
}
