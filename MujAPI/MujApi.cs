using BattleBitAPI.Common;
using BattleBitAPI.Server;
using MujAPI.Commands;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MujAPI
{
    public class MujApi
    {
        private static CommandProcessor serverCommandProcessor;
        private static Dictionary<MujPlayer, bool> premiumPlayers = new Dictionary<MujPlayer, bool>();
        private static Dictionary<ulong, Roles> thePoliceMods = new Dictionary<ulong, Roles>();
        public static Dictionary<MujPlayer, MapInfo> VoteMapList = new Dictionary<MujPlayer, MapInfo>();
        public static Dictionary<string, GameServer> GameServerIdentifiers = new Dictionary<string, GameServer>();

        //logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        //flags
        public static bool IsAcrossServerChatOn = false;

        public static void Start()
        {
            var listener = new ServerListener<MujPlayer>();
            listener.OnPlayerTypedMessage += OnPlayerChat;
            listener.OnGameServerConnected += OnGameServerConnected;
            listener.OnGameServerConnecting += OnGameServerConnecting;
            listener.OnPlayerConnected += OnPlayerConnected;
            listener.OnPlayerSpawning += OnPlayerSpawning;
            listener.OnGetPlayerStats += OnGetPlayerStats;
            //listener.OnMatchEnding += OnMatchEnding;
            listener.Start(29294);//Port

            //TestUserVotes(listener);

			serverCommandProcessor = new CommandProcessor(listener);
            Task.Run(() => serverCommandProcessor.Start()); //start server command processor



        }

        private static void TestUserVotes(ServerListener<MujPlayer> listener)
        {
            Random rnd = new Random();
            long min = (long)Math.Pow(10, 16);
            long max = (long)Math.Pow(10, 17) - 1;

            for (int i = 0; i < 20; i++)
            {
                ulong steamdid = (ulong)rnd.NextInt64(min, max);
                VoteMapList.Add(new MujPlayer(steamdid), new MapInfo((Maps)rnd.Next(1, 4), (MapDayNight)rnd.Next(0, 2)));
            }

            foreach (var keyValuePair in VoteMapList)
            {
                ulong SteamId = keyValuePair.Key.SteamID;
                string VotedMap = keyValuePair.Value.ToString();

                Console.WriteLine($"{SteamId} voted {VotedMap}");
            }

            var (totalOccurrencesCount, maxOccurrencesCount) = MujUtils.GetOccurances(VoteMapList);
            Console.WriteLine($"Total Occurrences Count: {totalOccurrencesCount}");
            Console.WriteLine($"Max Occurrences Count: {maxOccurrencesCount}");

            var HighestVotedMap = MujUtils.GetMapInfoWithHighestOccurrences(VoteMapList);
            Console.WriteLine($"Highest Vote: {HighestVotedMap}");

        }

        //callback hooks
        private static Task<PlayerStats> OnGetPlayerStats(ulong steamid, PlayerStats stats)
        {
            if (steamid == 76561198347766467)
            {
                Roles roles = Roles.Admin | Roles.Moderator;
                stats.Roles = roles;
                thePoliceMods.Add(steamid, roles);
                return Task.FromResult(stats);
            }
            return Task.FromResult(stats);
        }


        private static async Task OnPlayerConnected(MujPlayer player)
        {
            thePoliceMods.TryGetValue(player.SteamID, out var roles);
            player.Stats.Roles = roles;

            premiumPlayers.TryGetValue(player, out var isPremium);
            player.IsPremium = isPremium;


            if (!isPremium)
                player.Kick("Not a premium player. pay $2 bux");
        }

        private static async Task<bool> OnGameServerConnecting(IPAddress address)
        {
			log.Info(address.ToString() + " is attempting to connect");
            return true;
        }

        private static async Task OnPlayerChat(MujPlayer player, ChatChannel channel, string msg)
        {
            if (msg.StartsWith("!"))
            {
                ChatCommands.HandleChatCommand(player, channel, msg);
                log.Info(msg);
                // will check if they already voted
                if (!VoteMapList.ContainsKey(player))
                {
                    VoteMapList.Add(player, player.VotedMap);
                    return;
                }
                else
                {
                    player.GameServer.MessageToPlayer(player, "Already Voted Cannot Vote Again");
                    return;
                }
            }
            else if (IsAcrossServerChatOn && channel.HasFlag(ChatChannel.AllChat)) //experimental fr fr
            {
                string ServerIdentifier = GameServerIdentifiers.FirstOrDefault(kvp => kvp.Value == player.GameServer).Key;

                string[] ChatMessage = new string[2];
                ChatMessage[0] = "all";
                ChatMessage[1] = $"({ServerIdentifier}) {player.Name} : {msg}";

                serverCommandProcessor.SendChatMessageToAllServers(ChatMessage);
            }

        }

        private static async Task OnGameServerConnected(GameServer server)
        {
			var ServerRegex = new Regex(@"[A-Z]{2}#\d+");
			string ServerIdentifier = ServerRegex.Match(server.ServerName).Value;

            GameServerIdentifiers.Add(ServerIdentifier, server);

			log.Info($"{server} just connected");

            Timer timer = new(MujUtils.SendMessageEveryFiveMinutes, server, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        }

        private static async Task OnMatchEnding(GameServer server)
        {
            MapInfo MostVotedMap = MujUtils.GetMapInfoWithHighestOccurrences(VoteMapList);
            var (totalMapCount, maxMapCount) = MujUtils.GetOccurances(VoteMapList);

            //TODO switch to most voted map if skip vote is initiated

            log.Info($"{MostVotedMap}, {maxMapCount}, {totalMapCount}");
        }


        private static async Task<PlayerSpawnRequest> OnPlayerSpawning(MujPlayer player, PlayerSpawnRequest request)
        {
            if (request.Loadout.PrimaryWeapon.Tool == Weapons.M4A1)
            {
                //Don't allow M4A1
                request.Loadout.PrimaryWeapon.Tool = null;
            }
            else if (request.Loadout.PrimaryWeapon.Tool.WeaponType == WeaponType.SniperRifle)
            {
                //Force 6x if weapon is sniper.
                request.Loadout.PrimaryWeapon.MainSight = Attachments._6xScope;
            }

            //Override pistol with deagle
            request.Loadout.SecondaryWeapon.Tool = Weapons.DesertEagle;

            //Force everyone to use RPG
            request.Loadout.LightGadget = Gadgets.Rpg7HeatExplosive;

            //Don't allow C4s
            if (request.Loadout.HeavyGadget == Gadgets.C4)
                request.Loadout.HeavyGadget = null;

            //Spawn player 2 meter above than the original position.
            request.SpawnPosition.Y += 2f;

            //Remove spawn protection
            request.SpawnProtection = 0f;

            //Remove chest armor
            request.Wearings.Chest = null;

            //Give extra 10 more magazines on primary
            request.Loadout.PrimaryExtraMagazines += 10;

            //Give extra 5 more throwables 
            request.Loadout.ThrowableExtra += 5;

            return request;
        }

    }
}
