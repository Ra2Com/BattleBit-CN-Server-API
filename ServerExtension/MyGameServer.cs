using BattleBitAPI.Common;
using BattleBitAPI.Storage;
using BattleBitAPI.Server;
using CommunityServerAPI.Player;
using CommunityServerAPI.Utils;
using System.Numerics;
using Newtonsoft.Json;
using CommunityServerAPI.ServerExtension.Component;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Handler;
using BattleBitAPI;
using BattleBitAPI.Storage;
using CommunityServerAPI.ServerExtension.Handler.Commands;
using System.Net;

namespace CommunityServerAPI.ServerExtension
{
    public class MyGameServer : GameServer<MyPlayer>
    {
        DiskStorage ds = new DiskStorage(Environment.CurrentDirectory + "\\data");

        public override async Task OnConnected()
        {
            Console.WriteLine(
                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 已与游戏服务器 {ServerName} 建立通信 - {GameIP}:{GamePort}");

            // 固定 Random Revenge 的游戏模式和游戏地图
            this.MapRotation.ClearRotation();
            // MapRotation.SetRotation("Salhan", "Wakistan", "Construction", "District");
            this.MapRotation.SetRotation("Salhan", "Azagor", "Dustydew", "SandySunset", "WineParadise", "Frugis",
                "TensaTown");
            this.GamemodeRotation.ClearRotation();
            // GamemodeRotation.SetRotation("Domination");
            this.GamemodeRotation.SetRotation("TDM");

            // TODO: 这些数值配置最好都到一个 Json 解析配置类里面去
            // RoundSettings.MaxTickets = 1500;

            // 全局对局设置 - 2个玩家,10 秒后就可以开干了
            this.RoundSettings.PlayersToStart = 1;
            this.RoundSettings.SecondsLeft = 10;

            // 开启玩家体积碰撞
            this.ServerSettings.PlayerCollision = true;

            // 测试用途 For development test ONLY
            ForceStartGame();


        }


        List<IPlayerInfo> _rankPlayers = new List<IPlayerInfo>();

        public override async Task OnPlayerConnected(MyPlayer player)
        {
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
                if (args.BodyPart > 0 && args.BodyPart < PlayerBody.Shoulder)
                {
                    // 爆头击杀数据
                    args.Killer.HSKill++;
                }
                await Console.Out.WriteLineAsync($"击杀者仇人ID：{args.Killer.markId} ，被杀者ID{args.Victim.SteamID}");

                if (args.Killer.markId.ToString() == args.Victim.SteamID.ToString())
                {
                    if (args.Victim.Team == Team.TeamA)
                        this.RoundSettings.TeamATickets -= 10;
                    else if (args.Victim.Team == Team.TeamB)
                        this.RoundSettings.TeamBTickets -= 10;
                    args.Killer.markId = 0;
                    MessageToPlayer(args.Killer.SteamID, $"恭喜复仇成功",5f);
                }

                if (args.Killer != null)
                {
                    args.Killer.K++;
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
                    MessageToPlayer(args.Victim,
                        $"你被 {RichText.LightBlue}{args.Killer.Name}{RichText.EndColor}击倒" +
                        $"{RichText.LineBreak}凶手剩余 {RichText.LightBlue}{args.Killer.HP} HP{RichText.EndColor}",10f);
                    // 等到消息发布之后再给凶手补充血量，否则血量展示不对
                    args.Killer.Heal(20);
                }
            }
            catch (Exception ee) { Console.Out.WriteLineAsync($"OnAPlayerDownedAnotherPlayerError:{ee.StackTrace}+{ee.Message}"); }

        }

        public override async Task OnPlayerGivenUp(MyPlayer player)
        {
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家已放弃: " + player);
        }

        public override async Task OnPlayerDied(MyPlayer player)
        {
            player.D++;
        }

        public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
        {
            //await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - " + from + " 复活了 " + to);
        }

        public override async Task OnTick()
        {
            // DEVELOP TODO: 每 2 分钟发布一条 AnnounceShort 让玩家加群反馈
            // DEVELOP TODO: 每 3 分钟发布一条 全服聊天信息 让玩家加群反馈
            // Calculate current ranking.
            _rankPlayers.Clear();
            foreach (var item in AllPlayers)
            {
                _rankPlayers.Add(item);
            }

            _rankPlayers = _rankPlayers.OrderByDescending(x => x.K / x.D).ToList();
            for (int i = 0; i < _rankPlayers.Count; i++)
            {
                _rankPlayers[i].rank = i + 1;
            }
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

        // DEVELOP: 在玩家登录时，给玩家定义不同于官方的数据
        public override async Task OnPlayerJoiningToServer(ulong steamID, PlayerJoiningArguments args)
        {
            try
            {
                var stFromData = await ds.GetPlayerStatsOf(steamID);
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - stFromData：{JsonConvert.SerializeObject(stFromData)}");

                ulong role = await PrivilegeManager.GetPlayerPrivilege(steamID);
                stFromData.Roles = (Roles)role;

                // 特殊角色登录日志
                if ((Roles)role == Roles.Admin)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 超级管理员 {steamID} 已连接");
                }
                if ((Roles)role == Roles.Moderator)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 管理员 {steamID} 已连接");
                }

                if (stFromData != null)
                    args.Stats = stFromData;

                args.Stats.Progress.Rank = 200;
                args.Stats.Progress.Prestige = 6;
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
                Console.WriteLine($"OnSavePlayerStats:{steamID},PlayerStats:{stats.Roles}");
                await ds.SavePlayerStatsOf(steamID, stats);
            }
            catch (Exception ee)
            {
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ee.StackTrace}--{ee.Message}");

            }

        }
        
        public override async Task OnGameStateChanged(GameState oldState, GameState newState) 
        {
            if (newState == GameState.WaitingForPlayers)
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} ---------- 等待玩家 ----------");
                // 全局对局设置 - 2个玩家,10 秒后就可以开干了
                this.RoundSettings.PlayersToStart = 1;
                this.RoundSettings.SecondsLeft = 10;
                // DEVELOP: 测试时立马开始下一句游戏
                ForceStartGame();
            }
            
            
        }
    }
}