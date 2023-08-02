using BattleBitAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MujAPI
{
	public class ChatCommandHandler
	{
		private Dictionary<string, Action<string[], object[]>> commands;

		public ChatCommandHandler()
		{
			this.commands = new Dictionary<string, Action<string[], object[]>>();
		}

		public void AddCommand(string name, Action<string[], object[]> callback)
		{
			commands[name.ToLower()] = callback ?? throw new ArgumentNullException(nameof(callback), "Callback cannot be null");
		}

		public void ExecuteCommand(string input, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var chatChannel = (ChatChannel)optionalObjects[1];
			
			string[] commandParts = input.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
			if (commandParts.Length == 0)
			{
				player.Message("Invalid Command");
				return;
			}
			
			string commandName = commandParts[0].Substring(0).ToLower();
			string[] args = commandParts.Skip(1).ToArray();

			if(commands.TryGetValue(commandName, out Action<string[], object[]> callback))
			{
				callback(args, optionalObjects); // call the call back
			}
			else
			{
				player.Message("Command not found");
			}

		}
	}
}
