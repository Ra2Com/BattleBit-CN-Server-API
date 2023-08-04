using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.MujAPI.Common.Utils;
using MujAPI.Common;
using System.Text;

namespace MujAPI.Commands
{
    public class ChatCommands
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ChatCommands));


		public static bool IsVoteMapSkipAnnounced = false;
		public static bool IsMapVoteTrollFlagOn = false;
		public static int TotalVoteKicksNeeded = 20;
		public static Dictionary<ulong, int> SteamIDKickVotes = new Dictionary<ulong, int>();


		public static void RegisterMyCommands(ChatCommandHandler commandHandler)
		{
			//register commands
			commandHandler.AddCommand("votekick", ChatCommands.VoteKickCommand);
			commandHandler.AddCommand("kill", ChatCommands.KillCommand);
			commandHandler.AddCommand("skipmap", ChatCommands.SkipMapCommand);
			commandHandler.AddCommand("bully", ChatCommands.BullyUserCommand);
			commandHandler.AddCommand("update", ChatCommands.UpdateMapOrGameModeRotation);
			commandHandler.AddCommand("gamerule", ChatCommands.GameRuleCommand);
		}

		//gamerule Callback
		private static void GameRuleCommand(string[] args, object[] optionalObjects)
		{
			var Player = (MujPlayer)optionalObjects[0];
			var Gameserver = Player.GameServer;

			log.Info($"Update Issued by {(MujPlayer)optionalObjects[0]}");
			if (args.Length == 0)
			{
				Player.Message($"Usage: !gamerule <banweapon|unbanweapon|bangadget|unbangadget|banclass|unbanclass> \n<classname|<weaponname|all>|<gadgetname>");
				return;
			}
			if (args.Length == 2 || args[0].Contains("wearings"))
			{
				switch (args[0])
				{
					case "banweapon":
						if (args[1] == "all")
						{
							foreach (var allWeapon in Weapons.GetAllWeapons().Values)
							{
								MujApi.Rules.weaponBans.BanWeapon(allWeapon);
							}
							Player.Message($"All Weapons Banned o_0!!");
							break;
						}
						else if (Weapons.TryFind(args[1].ToUpper(), out Weapon weapon))
						{
							if (MujApi.Rules.weaponBans.BanWeapon(weapon))
							{
								Player.Message($"{weapon.Name} Has been banned");
								break;
							}
							else
							{
								Player.Message($"{weapon.Name} is already on ban list");
								break;
							}
						}
						else
						{
							Player.Message("Could not find weapon");
							break;
						}
					case "unbanweapon":
						if (args[1] == "all")
						{
							foreach (var allWeapon in Weapons.GetAllWeapons().Values)
							{
								MujApi.Rules.weaponBans.UnBanWeapon(allWeapon);
							}
							Player.Message($"All Weapons Banned o_0!!");
							break;
						}
						else if (Weapons.TryFind(args[1].ToUpper(), out Weapon unbanweapon))
						{
							if (MujApi.Rules.weaponBans.UnBanWeapon(unbanweapon))
							{
								Player.Message($"{unbanweapon.Name} Has been unbanned");
								break;
							}
							else
							{
								Player.Message($"{unbanweapon.Name} is not on ban list");
								break;
							}
						}
						else
						{
							Player.Message("Could not find weapon");
							break;
						}
					case "bangadget":
						if (Gadgets.TryFind(args[1], out Gadget bangadget))
						{
							if (MujApi.Rules.gadgetBans.BanGadget(bangadget))
							{
								Player.Message($"{bangadget.Name} has been banned");
								break;
							}
							else
							{
								Player.Message($"{bangadget.Name} is already on ban list");
								break;
							}
						}
						else
						{
							Player.Message("Could not find gadget");
							break;
						}
					case "unbangadget":
						if (Gadgets.TryFind(args[1], out Gadget unbangadget))
						{
							if (MujApi.Rules.gadgetBans.UnBanGadget(unbangadget))
							{
								Player.Message($"{unbangadget.Name} has been unbanned");
								break;
							}
							else
							{
								Player.Message($"{unbangadget.Name} is not in banlist");
								break;
							}
						}
						else
						{
							Player.Message("Could not find gadget");
							break;
						}
					case "banclass":
						if (Enum.TryParse<GameRole>(args[1], true, out GameRole BanGameRole))
						{
							if (MujApi.Rules.classBans.BanClass(BanGameRole))
							{
								Player.Message($"{BanGameRole} has been banned");
								break;
							}
							else
							{
								Player.Message($"{BanGameRole} is already banned");
								break;
							}
						}
						else
						{
							Player.Message($"Could not find class");
							break;
						}
					case "unbanclass":
						if (Enum.TryParse<GameRole>(args[1], true, out GameRole gameRole))
						{
							if (MujApi.Rules.classBans.UnBanClass(gameRole))
							{
								Player.Message($"{gameRole} has been unbanned");
								break;
							}
							else
							{
								Player.Message($"{gameRole} is already unbanned");
								break;
							}
						}
						else
						{
							Player.Message($"Could not find class");
							break;
						}
					case "banwearings":
						Player.Message("Not Implemented");
						break;
					case "unbanwearings":
						Player.Message("Not Implemented");
						break;
					default:
						Player.Message($"Usage: !gamerule <banweapon|unbanweapon|bangadget|unbangadget|banclass|unbanclass> \n<classname|weaponname|gadgetname>");
						break;
				}
				return;
			}
			else
			{
				Player.Message($"Usage: !gamerule <banweapon|unbanweapon|bangadget|unbangadget|banclass|unbanclass> \n<classname|weaponname|gadgetname>");
				return;
			}
		}

		// !update Callback
		private static void UpdateMapOrGameModeRotation(string[] args, object[] optionalObjects)
		{
			var Player = (MujPlayer)optionalObjects[0];
			var GameServer = Player.GameServer;

			log.Info($"Update Issued by {(MujPlayer)optionalObjects[0]}");
			if (args.Length == 0)
			{
				Player.Message("Usage: !update <map|gamemode> <mapname|gamemodename>.");
				return;
			}
			else if (args[0] == "map" && args.Length == 2)
			{
				if (!MujUtils.IsExistInMaps(args[1]))
				{
					Player.Message("That map doesnt exist");
					return;
				}
				if (GameServer.MapRotation.AddToRotation(args[1]))
				{
					Player.Message($"{args[1]} Added To Rotation");
					return;
				}
				else
				{
					GameServer.MapRotation.RemoveFromRotation(args[1]);
					Player.Message($"{args[1]} Removed From Rotation");
					return;
				}
			}
			else if (args[0] == "gamemode" && args.Length == 2)
			{
				if (!MujUtils.IsExistInGameModes(args[1]))
				{
					Player.Message("That gamemode doesnt exist");
					return;
				}
				if (GameServer.GamemodeRotation.AddToRotation(args[1]))
				{
					Player.Message($"{args[1]} Added To Rotation");
					return;
				}
				else
				{
					GameServer.GamemodeRotation.RemoveFromRotation(args[1]);
					Player.Message($"{args[1]} Removed From Rotation");
					return;
				}
			}
			else 
			{
				Player.Message("Usage: !update <map|gamemode> <mapname|gamemodename>.");
				return; 
			}
		}

		// !votekick Callback
		public static void VoteKickCommand(string[] args, object[] optionalObjects)
		{
			log.Info($"VoteKick Issued by {(MujPlayer)optionalObjects[0]}");
			if (args.Length == 1)
			{
				var player = (MujPlayer)optionalObjects[0];
				var GameServer = player.GameServer;
				ulong targetPlayerSteamId = GameServer.FindSteamIdByName(args[0], GameServer);

				//helper function to handle votes
				void HandleVotes(int votes)
				{
					// sends message to player and a uilog on the server
					player.Message($"Vote To Kick Player:({args[0]}) Registered. Total Votes: {votes}/{TotalVoteKicksNeeded}");
					GameServer.UILogOnServer($"Need ({TotalVoteKicksNeeded - votes}) to kick player:({args[0]})", 5f);
					
					//if there is 1 vote then an announcement is made
					if (votes == 1)
					{
						GameServer.AnnounceShort($"A Vote Kick on Player:({args[0]}) has been initiated. Needs {TotalVoteKicksNeeded} Votes to kick");
						return;
					}

					//if there are 20 or more votes then that player is kicked
					if (votes >= 20)
					{
						GameServer.Kick(targetPlayerSteamId, "Vote Kicked");
						log.Info($"{targetPlayerSteamId} has been kicked from {GameServer}");
						GameServer.UILogOnServer($"({args[0]}) Has Been Kicked", 5f);
						SteamIDKickVotes.Remove(targetPlayerSteamId);
					}
				}

				if (!SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
				{
					//set to 1 if it doesnt exist
					SteamIDKickVotes[targetPlayerSteamId] = 1;
					SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
					HandleVotes(value);
					return;
				}
				if (SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
				{
					// increment the value of the target player
					SteamIDKickVotes[targetPlayerSteamId] += 1; 
					SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
					HandleVotes(value);
				}
			}
			else
			{
				var player = (MujPlayer)optionalObjects[0];
				player.Message("Usage: !votekick <playername>");
			}
		}

		// !kill Callback
		public static void KillCommand(string[] args, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var GameServer = player.GameServer;

			if (args.Length == 0)
				log.Info($"VoteKick Issued by {player}");
			else
				log.Info($"VoteKick Issued by {player}: args: {args[0]}");

			//checks if player is mod or admin
			if (!player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
			{
				player.Message("Can only be ran by admin or mod");
				return;
			}
			//if trusted player is passed then kill command is issued 
			if (args.Length == 1)
			{
				if (ulong.TryParse(args[0], out ulong SteamId))
				{
					MujPlayer Victim = GameServer.FindPlayerBySteamId(SteamId, GameServer);
					if (Victim == null)
					{
						player.Message("Player could not be found");
						return;
					}
					GameServer.Kill(SteamId);
				}
				else
				{
					// their steamid by name
					ulong victimSteamID = GameServer.FindSteamIdByName(args[0], GameServer);
					if (victimSteamID == 0)
					{
						player.Message("Player could not be found");
						return;
					}
					GameServer.Kill(victimSteamID);
				}
				log.Info($"{SteamId} killed");
				GameServer.AnnounceShort($"<color=red>{args[0]}</color> Has been Smited!!");
				return;
			}
			else
			{
				player.Message("Usage !kill <playername>");
			}
		}

		// !skipmap Callback
		public static void SkipMapCommand(string[] args, object[] optionalObjects)
		{
			var player = (MujPlayer)optionalObjects[0];
			var GameServer = player.GameServer;

			// looks for 1 argument
			if (args.Length == 1)
			{
				// flag that can only be enabled by admin or mod
				if (args[0] == "trollflagon" && player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
				{
					IsMapVoteTrollFlagOn = !IsMapVoteTrollFlagOn;
					string enabled = IsMapVoteTrollFlagOn ? "Enabled" : "Disabled";
					player.Message($"Lonovo Night Troll Flag {enabled}");
					return;
				}

				// returns a list of the maps available to vote
				if (args[0] == "mapnames")
				{
					StringBuilder sb = new StringBuilder();
					int count = 0;
					foreach (var mapName in MujUtils.GetMaps())
					{
						if (count > 0 && count % 5 == 0)
							sb.Append(Environment.NewLine);

						sb.Append($"|{mapName}|");
						count++;
					}
					player.Message($"<b><i><color=red>Here is a list of the maps:</color></i></b>\n" + sb);
					return;
				}
			}

			//looks for 2 arguments
			if (args.Length == 2)
			{
				Maps MatchedMap = MujUtils.GetMapFromString(args[0]);
				MapDayNight MatchedMapDayNight = MujUtils.GetDayNightEnumFromString(args[1]);
				// sends error message to user if they dont input a valid map name
				if (MatchedMap == Maps.None)
				{
					player.Message("Not a valid map. Type !skipmap mapnames to get a list of the maps");
					return;
				}
				// kicks the player for choosing lonovo night
				if (IsMapVoteTrollFlagOn && MatchedMap == Maps.Lonovo && MatchedMapDayNight == MapDayNight.Night)
				{
					string reason = "smh ╭∩╮(-_-)╭∩╮";
					player.Kick(reason);
					log.Info($"{player} kicked from {GameServer} | Reason: {reason}");
					return;
				}
				else
				{
					MapInfo mapInfo = new() { Map = MatchedMap, DayNight = MatchedMapDayNight };
					player.VotedMap = mapInfo;
					//adds the user to a votemaplist
					if (!MujApi.VoteMapList.ContainsKey(player))
					{
						MujApi.VoteMapList.Add(player, mapInfo);
						return;
					}
					else
					{
						player.Message("You Have Already Voted");
						return;
					}
				}
			}
			//if (!IsVoteMapSkipAnnounced)
			//{
			//	//announces to the game a map skip has been initiated
			//	GameServer.AnnounceLong("A Skip Map Vote Has been initiated");
			//	IsVoteMapSkipAnnounced = true;
			//}
			else
			{
				player.Message("this is used to skip the current map Usage: !skipmap <mapname> <day/night>");
				return;
			}
		}

		// !bully Callback
		public static void BullyUserCommand(string[] args, object[] optionalObjects)
		{
			var Player = (MujPlayer)optionalObjects[0];
			var GameServer = Player.GameServer;

			//helper method for removing and adding to dictionary
			void RemoveOrAddToBullyList(MujPlayer victim)
			{
				ulong VictimSteamId = victim.SteamID;
				if (MujApi.BullyList.ContainsKey(VictimSteamId))
					MujApi.BullyList.Remove(VictimSteamId);
				else
					MujApi.BullyList[VictimSteamId] = victim;
				Player.Message($"Player targeted");
			}

			// permission check
			if (!Player.Stats.Roles.HasFlag(Roles.Admin | Roles.Moderator))
			{
				Player.Message("Can only be ran by admin or mod");
				return;
			}
			
			//check if only 1 argument
			if (args.Length == 1) 
			{
				if (args[0] == "help")
				{
					Player.Message($"<color=red>bullying a user can be toggled by typing their name or steamid again</color>");
					return;
				}
				if (ulong.TryParse(args[0], out ulong steamid))
				{
					//find the player
					MujPlayer Victim =  GameServer.FindPlayerBySteamId(steamid, GameServer);
					if (Victim == null)
					{
						Player.Message("Victim could not be found");
						return;
					}
					RemoveOrAddToBullyList(Victim);
				}
				else
				{
					// their steamid
					ulong victimSteamID = GameServer.FindSteamIdByName(args[0], GameServer);
					if (victimSteamID == 0)
					{
						Player.Message("Victim could not be found");
						return;
					}
					MujPlayer Victim = GameServer.FindPlayerBySteamId(victimSteamID, GameServer);
					RemoveOrAddToBullyList(Victim);
				}
			}
			else
				Player.Message("Usage: !bully <steamid|username>");
		}
	}
}