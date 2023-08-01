using BattleBitAPI.Common;
using MujAPI;
using System.Text;

namespace MujAPI.Commands
{
	public class ChatCommands
    {


		public static bool IsVoteMapSkipAnnounced = false;
		public static bool IsMapVoteTrollFlagOn = false;
        public static int TotalVoteKicksNeeded = 0;
        public static Dictionary<ulong, int> SteamIDKickVotes = new Dictionary<ulong, int>();

		/// <summary>
		/// used to handle command in game chat
		/// </summary>
		/// <param name="player"></param>
		/// <param name="msg"></param>
		public static void HandleChatCommand(MujPlayer player, ChatChannel channel, string msg)
        {
            string[] commandParts = msg.Trim().ToLower().Split(" ");
            if (commandParts.Length > 0)
            {
                string command = commandParts[0].Substring(1);
                string[] args = commandParts.Skip(1).ToArray();
                switch (command)
                {
                    case "votekick":
						if (args.Length == 1)
                        {
							ulong targetPlayerSteamId = player.GameServer.FindSteamIdByName(args[0], player.GameServer);
							if (!SteamIDKickVotes.ContainsKey(targetPlayerSteamId))
								SteamIDKickVotes[targetPlayerSteamId] = 1; //set to 1 if it doesnt exist
							else
							{
								SteamIDKickVotes[targetPlayerSteamId] += 1; // increment the value of the target player
								SteamIDKickVotes.TryGetValue(targetPlayerSteamId, out int value); //get the votes of the target player
								player.Message($"Vote To Kick {args[0]} Registered. Total Votes: {value}/{TotalVoteKicksNeeded}");
								player.GameServer.UILogOnServer($"Need {TotalVoteKicksNeeded - value} to kick {args[0]}", 5f);
								if (value == 1)
									player.GameServer.AnnounceShort($"A Vote Kick on {args[0]} has been initiated. Needs {TotalVoteKicksNeeded} Votes to kick");
								if (value >= 20)
								{
									player.GameServer.Kick(targetPlayerSteamId, "Vote Kicked");
									player.GameServer.UILogOnServer($"{args[0]} Has Been Kicked", 5f);
									SteamIDKickVotes.Remove(targetPlayerSteamId);
								}
								break;
							}
							break;
                        }
                        else
                        {
							player.Message("Usage: !votekick <playername>");
                            break;
                        }
                    case "kill":
                        if (!player.Stats.Roles.HasFlag(Roles.Moderator))
                        {
                            player.Message("Ur Not an admin");
                            break;
                        }
                        if (args.Length == 1)
                        {
							ulong steamid = player.GameServer.FindSteamIdByName(args[0], player.GameServer);
							player.GameServer.Kill(steamid);
							player.GameServer.AnnounceShort($"<color=red>{args[0]}</color> Has been Smited!!");
							break;
                        }
                        else
                        {
							player.Message("Usage !kill <playername>");
                            break;
                        }
                    case "skipmap":
                        if (args.Length == 1)
                        {
							if (args[0] == "trollflagon" && player.Stats.Roles.HasFlag(Roles.Moderator | Roles.Admin))
                                IsMapVoteTrollFlagOn = !IsMapVoteTrollFlagOn;
                            
                            if (args[0] == "mapnames")
                            {
								StringBuilder sb = new StringBuilder();
								int count = 0;
								foreach (var mapName in MujUtils.stringToEnumMap.Keys)
								{
									if (count > 0 && count % 5 == 0)
										sb.Append(Environment.NewLine);

									sb.Append($"|{mapName}|");
									count++;
								}
								player.Message($"<b><i><color=red>Here is a list of the maps:</color></i></b>\n" + sb);
                            }
                        }
                        if (args.Length == 2)
                        {
							Maps MatchedMap = MujUtils.GetMapsEnumFromMapString(args[0]);
							MapDayNight MatchedMapDayNight = MujUtils.GetDayNightEnumFromString(args[1]);
							if (MatchedMap == Maps.None)
							{
								player.Message("Not a valid map. Type !skipmap mapnames to get a list of the maps");
								break;
							}
							if (IsMapVoteTrollFlagOn && MatchedMap == Maps.Lonovo && MatchedMapDayNight == MapDayNight.Night)
							{
								player.Kick("smh ╭∩╮(-_-)╭∩╮");
								break;
							}
							else
							{
								MapInfo MapInfo = new() { Map = MatchedMap, DayNight = MatchedMapDayNight };
								player.VotedMap = MapInfo;
								break;
							}
                        }
                        if (!IsVoteMapSkipAnnounced)
                        {
                            player.GameServer.AnnounceLong("A Skip Map Vote Has been initiated");
                            IsVoteMapSkipAnnounced = true;
                        }
                        else
                        {	
                            player.Message("this is used to skip the current map Usage: !skipmap <mapname> <day/night>");
                            break;
                        }
                        break;

                    default:
                        player.Message("Invalid Usage of commands");
                        break;
                }
            }
        }
    }
}