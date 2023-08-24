using BattleBitAPI.Common;
using BattleBitAPI.Storage;
using BattleBitAPI.Server;
using CommunityServerAPI.Player;
using CommunityServerAPI.Utils;
using CommunityServerAPI.Content;
using Newtonsoft.Json;
using CommunityServerAPI.ServerExtension.Component;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Handler;
using BattleBitAPI;
using BattleBitAPI.Storage;
using CommunityServerAPI.ServerExtension.Handler.Commands;
using System.Net;
using System.Numerics;

namespace CommunityServerAPI.ServerExtension
{
    public class MyGameServer : GameServer<MyPlayer>, IServerSetting
    {
        DiskStorage ds = new DiskStorage(Environment.CurrentDirectory + "\\PlayerData");

        public override async Task OnConnected()
        {
            Console.WriteLine(
                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 已与游戏服务器 {ServerName} 建立通信 - {GameIP}:{GamePort}");

            // 按照服务端的游戏模式初始化游戏地图池
            SetServerDefaultSettings();

            // 开启玩家体积碰撞
            this.ServerSettings.PlayerCollision = true;

            // 测试用途 For development test ONLY
            ForceStartGame();
        }

        List<IPlayerInfo> _rankPlayers = new List<IPlayerInfo>();

        public override async Task OnPlayerConnected(MyPlayer player)
        {
            try
            {
                Console.WriteLine($"OnPlayerConnected");
                if (TryGetPlayer(player.SteamID, out MyPlayer op))
                {
                    var stFromData = await ds.GetPlayerStatsOf(player.SteamID) ?? new PlayerStats();
                    ulong role = await PrivilegeManager.GetPlayerPrivilege(player.SteamID);
                    if (stFromData.Progress.KillCount > op.stats.Progress.KillCount)
                    {
                        op.stats = stFromData;
                        op.stats.Roles = (Roles)role;
                        Console.WriteLine($"OnPlayerConnected 设置个人数据成功{player.SteamID},{JsonConvert.SerializeObject(op.stats)}");
                    }
                }
                await CalculateRanking();
            }
            catch (Exception ee) { Console.Out.WriteLineAsync($"OnPlayerConnected:{ee.StackTrace}+{ee.Message}"); }

            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家 {player.Name} - {player.SteamID} 已连接, IP: {player.IP}");
        }

        public override async Task OnPlayerDisconnected(MyPlayer player)
        {
            foreach (var item in AllPlayers)
            {
                if (item.markId == player.SteamID)
                {
                    item.markId = 0;
                }
            }
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家 {player.Name} 已离线");
        }

        public override async Task OnPlayerSpawned(MyPlayer player)
        {
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 已复活，坐标 {player.Position}");
        }

        public override async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
        {
            try
            {
                await Console.Out.WriteLineAsync($"击杀者仇人ID：{args.Killer.markId} ，被杀者ID{args.Victim.SteamID}");

                if (args.Killer.markId.ToString() == args.Victim.SteamID.ToString())
                {
                    if (args.Victim.Team == Team.TeamA)
                        this.RoundSettings.TeamATickets -= 10;
                    else if (args.Victim.Team == Team.TeamB)
                        this.RoundSettings.TeamBTickets -= 10;
                    args.Killer.markId = 0;
                    MessageToPlayer(args.Killer.SteamID,
                        $"恭喜你杀掉了你的仇人 {RichText.Red}{args.Victim.Name}{RichText.EndColor} 并缴获他的武器" +
                        $"{RichText.BR}现在你成为了他的仇人", 5f);
                }
                if (args.Killer != null)
                {
                    PlayerLoadout victimLoadout = args.Victim.CurrentLoadout;
                    args.Killer.SetFirstAidGadget(victimLoadout.FirstAidName, 0);
                    args.Killer.SetThrowable(victimLoadout.ThrowableName, victimLoadout.ThrowableExtra);
                    args.Killer.SetHeavyGadget(victimLoadout.HeavyGadgetName, victimLoadout.HeavyGadgetExtra);
                    args.Killer.SetLightGadget(victimLoadout.LightGadgetName, victimLoadout.LightGadgetExtra);
                    args.Killer.SetSecondaryWeapon(victimLoadout.SecondaryWeapon, victimLoadout.SecondaryExtraMagazines);
                    args.Killer.SetPrimaryWeapon(victimLoadout.PrimaryWeapon, victimLoadout.PrimaryExtraMagazines);
                    args.Victim.markId = args.Killer.SteamID;

                    //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {args.Killer.Name} 击杀了 {args.Victim.Name} , 缴获武器{JsonConvert.SerializeObject(victimLoadout.PrimaryWeapon)} ");

                    // Announce the victim your killer. And the killer will be tracked.
                    MessageToPlayer(args.Victim.SteamID,
                        $"你被 {RichText.LightBlue}{args.Killer.Name}{RichText.EndColor} 击倒" +
                        $"{RichText.BR}凶手剩余 {RichText.LightBlue}{args.Killer.HP} HP{RichText.EndColor}", 10f);
                    // 等到消息发布之后再给凶手补充血量，否则击杀血量展示不对
                    args.Killer.Heal(20);
                }

                args.Killer.stats.Progress.KillCount++;
                args.Victim.stats.Progress.DeathCount++;
            }
            catch (Exception ee) { Console.Out.WriteLineAsync($"OnAPlayerDownedAnotherPlayerError:{ee.StackTrace}+{ee.Message}"); }

        }

        public override async Task OnPlayerGivenUp(MyPlayer player)
        {
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家已放弃: " + player);
        }

        public override async Task OnPlayerDied(MyPlayer player)
        {
        }

        public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
        {
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - " + from + " 复活了 " + to);
        }

        public override async Task OnTick()
        {
            // DEVELOP TODO: 每 4 分钟发布一条 AnnounceShort 让玩家加群反馈
            // TODO: 每 7 分钟发布一条 全服聊天信息 让玩家加群反馈


            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - OnTick:{_rankPlayers.Count}人上榜");

        }

        public override async Task<OnPlayerSpawnArguments?> OnPlayerSpawning(MyPlayer player,
            OnPlayerSpawnArguments request)
        {
            try
            {
                request.Loadout = LoadoutManager.GetRandom(); // 出生后随机装备
                request.SpawnStand = PlayerStand.Standing; // 站着出生
                request.SpawnProtection = 5f; // 出生不动保护 5 秒

                int beforePosTime = 15;

                //while (true)
                //{
                //    if (player.positionBef.Count > 0)
                //    {
                //        var pb = player.positionBef.Last();
                //        player.positionBef.Remove(pb);
                //        Console.Out.WriteLineAsync($"{pb.position}");

                //        if (TimeUtil.GetUtcTimeMs() - pb.time > 1000 * beforePosTime)
                //        {
                //            //if (AllPlayers.FirstOrDefault(o =>
                //            //        Vector3.Distance(o.Position, pb.position) < 20f && o.Team != player.Team).SteamID==0)
                //            //{
                //            request.SpawnPosition = new Vector3
                //            { X = pb.position.X - 500, Y = pb.position.Y - 250, Z = pb.position.Z - 500 };
                //            request.RequestedPoint = PlayerSpawningPosition.SpawnAtPoint;
                //            Console.WriteLine(
                //                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 即将复活在 {request.SpawnPosition}");
                //            break;
                //            //}
                //        }
                //    }
                //    else
                //    {
                //        Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活在选择点");
                //        break;
                //    }
                //}

                player.positionBef.Clear();

                // TODO 在 Oki 部署了真正的地图边界且地面以上随机出生点后，再使用真正的随机出生点，做 RandomSpawn Points 需要适配地图太多且有任何改动都要重新写数值
                // 由于 Oki 在 Discord 中提及了地图边界的问题以及 SpawnPosition 的不可写问题，所以暂时使用 TDM 模式的固定出生点
                // Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活，MagazineIndex：{request.Loadout.PrimaryWeapon.MagazineIndex}，SkinIndex：{request.Loadout.PrimaryWeapon.SkinIndex}，requestPosition：{request.SpawnPosition.X}，{request.SpawnPosition.Y}，{request.SpawnPosition.Z}。。LookDirection：{request.LookDirection.X}，{request.LookDirection.Y}，{request.LookDirection.Z}");
            }
            catch (Exception ee)
            {
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ee.StackTrace}--{ee.Message}");
            }

            return request;
        }
        public override async Task OnPlayerJoiningToServer(ulong steamID, PlayerJoiningArguments args)
        {
            try
            {
                var stFromData = await ds.GetPlayerStatsOf(steamID) ?? new PlayerStats();
                ulong role = await PrivilegeManager.GetPlayerPrivilege(steamID);
                if (stFromData.Progress.KillCount > args.Stats.Progress.KillCount)
                {
                    args.Stats = stFromData;
                }
                args.Stats.Roles = stFromData.Roles = (Roles)role;
                args.Stats.Progress.Rank = 200;
                args.Stats.Progress.Prestige = 6;
                // 特殊角色登录日志
                if ((Roles)role == Roles.Admin)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 超级管理员 {steamID} 已连接");
                }
                if ((Roles)role == Roles.Moderator)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 管理员 {steamID} 已连接");
                }


            }
            catch (Exception ee)
            {
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ee.StackTrace}--{ee.Message}");
            }


        }
        // 聊天监控和命令
        public override async Task<bool> OnPlayerTypedMessage(MyPlayer player, ChatChannel channel, string msg)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - " + player.Name + "在「" +
                                             channel + "」发送聊天 - " + msg);
            // TODO: 聊天记录建议单独保存
            // TODO: 屏蔽词告警
            // TODO: 屏蔽词系统

            await CommandComponent.Initialize().HandleCommand(player, channel, msg);

            return true;
        }

        public override async Task OnSavePlayerStats(ulong steamID, PlayerStats stats) // 当储存玩家进度信息时
        {
            try
            {
                Console.WriteLine($"OnSavePlayerStats:{steamID},PlayerStats:{JsonConvert.SerializeObject(stats)}  服务器数据");
                if (TryGetPlayer(steamID, out MyPlayer op))
                {
                    Console.WriteLine($"OnSavePlayerStats:{steamID},PlayerStats:{JsonConvert.SerializeObject(op.stats)}  本地");
                }
                if (op.stats.Progress.KillCount > stats.Progress.KillCount)
                    stats.Progress.KillCount = op.stats.Progress.KillCount;
                if (op.stats.Progress.DeathCount > stats.Progress.DeathCount)
                    stats.Progress.DeathCount = op.stats.Progress.DeathCount;
                await ds.SavePlayerStatsOf(steamID, stats);
            }
            catch (Exception ee)
            {
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ee.StackTrace}--{ee.Message}");

            }

        }

        public override async Task OnGameStateChanged(GameState oldState, GameState newState)
        {
            await CalculateRanking();
            switch (newState)
            {
                case GameState.WaitingForPlayers:
                    Console.Out.WriteLineAsync($" ---------- 等待玩家 ----------");
                    // 全局对局设置 - 1个玩家可以开干了
                    RoundSettings.SecondsLeft = 10;
                    RoundSettings.PlayersToStart = 1;
                    // DEVELOP: 测试时立马开始下一局游戏
                    ForceStartGame();
                    break;
                case GameState.EndingGame:
                    await Console.Out.WriteLineAsync($" ---------- 对局 {RoundIndex} 结束 - 会话 {SessionID} ----------");
                    await Console.Out.WriteLineAsync($" ---------- 存储 {AllPlayers.Count()} 个玩家数据 ----------");
                    await this.EndingSaveAllPlayerStats();

                    // 给 AllPlayer 中所有的玩家名称执行 ServerMOTD
                    foreach (var player in AllPlayers)
                    {
                        await ServerMOTD(player);
                    }

                    break;
                case GameState.CountingDown:
                    {
                        await Console.Out.WriteLineAsync($" ---------- {RoundIndex} 开始倒计时 - 会话 {SessionID} ----------");
                        RoundSettings.SecondsLeft = 10;

                        break;
                    }
                case GameState.Playing:
                    try
                    {
                        await Console.Out.WriteLineAsync($" ---------- 对局 {RoundIndex} 开始 - 会话 {SessionID} ----------");
                        this.MapRotation.ClearRotation();
                        var nextMap = MapManager.GetARandomAvailableMap(Gamemode);
                        this.MapRotation.SetRotation(nextMap.ToArray());
                        await Console.Out.WriteLineAsync($" ---------- 下张地图已随机为 {nextMap[0]}  ----------");
                        SayToAllChat($"随机地图结果 — 下张地图是 — {nextMap[0]}");
                        // TODO: 把对局配置设置项都移动到单一配置文件中
                        this.RoundSettings.SecondsLeft = 1800;
                        this.SetRoundTickets();

                    }
                    catch (Exception ee) { Console.Out.WriteLineAsync($"PlayingError:{ee.StackTrace}+{ee.Message}"); }
                    break;
            }
        }
        public async Task CalculateRanking()
        {
            _rankPlayers.Clear();
            foreach (var item in AllPlayers)
            {
                _rankPlayers.Add(item);
            }

            _rankPlayers = _rankPlayers.OrderByDescending(x => x.stats.Progress.KillCount / (x.stats.Progress.DeathCount + 1)).ToList();
            for (int i = 0; i < _rankPlayers.Count; i++)
            {
                if (TryGetPlayer(_rankPlayers[i].SteamID, out MyPlayer op))
                {
                    op.rank = i + 1;
                }
            }
        }
        // 存储所有当前玩家的数据
        private async Task EndingSaveAllPlayerStats()
        {
            //todo 
            //foreach (var player in AllPlayers)
            //{
            //    await OnSavePlayerStats(player.SteamID, player.stats);
            //}
        }
        private void SetServerDefaultSettings()
        {
            MapRotation.ClearRotation();
            MapRotation.SetRotation(MapManager.GetAvailableMapList(Gamemode).ToArray());
            GamemodeRotation.ClearRotation();
            GamemodeRotation.SetRotation(Gamemode);
        }
        private async Task ServerMOTD(MyPlayer player)
        {
            var JIAQUN = MessageOfTheDayManager.GetMOTD("JoinMethodQun");
            var MOTD = MessageOfTheDayManager.GetMOTD("WelcomeMsg");
            MessageToPlayer(player.SteamID, $"{RichText.Vip}{RichText.Cyan}{player.Name}{RichText.EndColor} 你好" +
                    $"{RichText.BR}游戏时长 {player.stats.Progress.PlayTimeSeconds / 60} 分钟 , K/D: {player.stats.Progress.KillCount}/{player.stats.Progress.DeathCount} , 爆头 {player.stats.Progress.Headshots} 次" +
                    $"{RichText.BR}当前排名 {RichText.Orange}{player.rank}{RichText.EndColor}" +
                    $"{RichText.BR}" +
                    $"{RichText.BR}{RichText.LightBlue}===请注意==={RichText.EndColor}" +
                    $"{RichText.BR}{MOTD}" +
                    $"{RichText.BR}" +
                    $"{RichText.BR}{JIAQUN}", 15f);
        }
        private void SetRoundTickets()
        {
            var playerNum = AllPlayers.Count();
            double addroundTickets = playerNum switch
            {
                <= 4 => 100,
                <= 10 => playerNum * 2,
                <= 20 => playerNum * 3,
                <= 36 => playerNum * 5,
                <= 48 => playerNum * 7,
                <= 64 => playerNum * 11,
                <= 80 => playerNum * 13,
                <= 96 => playerNum * 17,
                <= 128 => playerNum * 20,
                _ => RoundSettings.MaxTickets
            };
            RoundSettings.MaxTickets += addroundTickets;
            RoundSettings.TeamATickets += addroundTickets;
            RoundSettings.TeamBTickets += addroundTickets;
        }
    }
}