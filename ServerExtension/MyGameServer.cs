using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.Tools;
using System.Numerics;
using Newtonsoft.Json;
using CommunityServerAPI.ServerExtension.Component;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Handler;

namespace CommunityServerAPI.ServerExtension
{
    internal class MyGameServer : GameServer<MyPlayer>
    {
        public override async Task OnConnected()
        {
            Console.WriteLine(
                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 已与游戏服务器 {ServerName} 建立通信 - {GameIP}:{GamePort}");

            // 固定 Random Revenge 的游戏模式和游戏地图
            MapRotation.ClearRotation();
            // MapRotation.SetRotation("Salhan", "Wakistan", "Construction", "District");
            MapRotation.SetRotation("Salhan", "Azagor", "Dustydew", "SandySunset", "WineParadise", "Frugis", "TensaTown");
            GamemodeRotation.ClearRotation();
            // GamemodeRotation.SetRotation("Domination");
            GamemodeRotation.SetRotation("TDM");

            // TODO: 这些数值配置最好都到一个 Json 解析配置类里面去
            RoundSettings.MaxTickets = 1500;

            // 全局对局设置 - 2个玩家,10 秒后就可以开干了
            RoundSettings.PlayersToStart = 2;
            RoundSettings.SecondsLeft = 10;

            // 开启玩家体积碰撞
            ServerSettings.PlayerCollision = true;

            // 测试用途 For development test ONLY
            ForceStartGame();
        }


        List<IPlayerInfo> rankPlayers = new List<IPlayerInfo>();

        public override async Task OnPlayerConnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync(
                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家 {player.Name} - {player.SteamID} 已连接, IP: {player.IP}");
        }

        public override async Task OnPlayerDisconnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家 {player.Name} 已离线");
        }

        public override async Task OnPlayerSpawned(MyPlayer player)
        {
            await Console.Out.WriteLineAsync(
                $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 已复活，坐标 {player.Position}");
        }

        public override async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
        {
            if (args.BodyPart > 0 && args.BodyPart < PlayerBody.Shoulder)
            {
                // DEVELOP TODO: 记录玩家爆头击杀数
            }

            if (args.Killer != null)
            {
                // Basic Revenger mode function, kills victim if it's down, add Killer's data, do Random Mode's work. etc.
                args.Killer.K++;
                // DEVELOP TODO: 如果击杀的是仇人，且仇人在复活后没有死亡，仇人队伍的 Tickets 要扣除 10
                PlayerLoadout victimLoadout = args.Victim.CurrentLoadout;
                args.Killer.SetFirstAidGadget(victimLoadout.FirstAidName, 0, true);
                args.Killer.SetThrowable(victimLoadout.ThrowableName, 0, true);
                args.Killer.SetHeavyGadget(victimLoadout.HeavyGadgetName, 0, true);
                args.Killer.SetLightGadget(victimLoadout.LightGadgetName, 0, true);
                // DEVELOP TODO: 需要在别的地方更换死者武器，放到 OnPlayerSpawned 那边的方法
                //args.Killer.SetSecondaryWeapon(victimLoadout.SecondaryWeapon, 0, true);
                //args.Killer.SetPrimaryWeapon(victimLoadout.PrimaryWeapon, 0, true);
                args.Victim.markId = args.Killer.SteamID;
                // 获取双方距离
                float killDistance = Vector3.Distance(args.VictimPosition, args.KillerPosition).ToString("#0.0");

                await Console.Out.WriteLineAsync(
                    $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {args.Killer.Name} 击杀了 {args.Victim.Name} - {killDistance}M, 缴获武器{JsonConvert.SerializeObject(victimLoadout.PrimaryWeapon)} ");

                // Announce the victim your killer. And the killer will be tracked.
                MessageToPlayer(args.Victim,
                    $"你被 {RichText.Red}{args.Killer.Name}{RichText.EndColor} 在 {RichText.Navy}{killDistance} 米{RichText.EndColor}击倒" +
                    $"{RichText.LineBreak}凶手剩余 {RichText.Maroon}{args.Killer.HP} HP{RichText.EndColor}");
                // 等到消息发布之后再给凶手补充血量，否则血量展示不对
                args.Killer.Heal(20);
            }
        }

        public override async Task OnPlayerGivenUp(MyPlayer player)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家已放弃: " + player);
        }

        public override async Task OnPlayerDied(MyPlayer player)
        {
            player.D++;
            //await Console.Out.WriteLineAsync("Died: " + player);
        }

        public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - " + from + " 复活了 " + to);
        }

        public override async Task OnTick()
        {
            // DEVELOP TODO: 每 2 分钟发布一条 AnnounceShort 让玩家加群反馈
            // DEVELOP TODO: 每 3 分钟发布一条 全服聊天信息 让玩家加群反馈
            // Calculate current ranking.
            rankPlayers.Clear();
            foreach (var item in AllPlayers)
            {
                rankPlayers.Add(item);
            }

            rankPlayers = rankPlayers.OrderByDescending(x => x.K / x.D).ToList();
            for (int i = 0; i < rankPlayers.Count; i++)
            {
                rankPlayers[i].rank = i + 1;
            }
        }

        public override async Task<OnPlayerSpawnArguments> OnPlayerSpawning(MyPlayer player,
            OnPlayerSpawnArguments request)
        {
            try
            {
                request.Loadout = SpawnManager.GetRandom(); // 出生后随机装备
                request.SpawnStand = PlayerStand.Standing; // 站着出生
                request.SpawnProtection = 5f; // 出生不动保护 5 秒

                int beforePosTime = 15;

                while (true)
                {
                    if (player.positionBef.Count > 0)
                    {
                        var pb = player.positionBef.Last();
                        player.positionBef.Remove(pb);
                        Console.Out.WriteLineAsync($"{pb.position}");

                        if (TimeUtil.GetUtcTimeMs() - pb.time > 1000 * beforePosTime)
                        {
                            if (AllPlayers.FirstOrDefault(o =>
                                    Vector3.Distance(o.Position, pb.position) < 20f && o.Team != player.Team) == null)
                            {
                                request.SpawnPosition = new Vector3
                                    { X = pb.position.X - 500, Y = pb.position.Y - 250, Z = pb.position.Z - 500 };
                                request.RequestedPoint = PlayerSpawningPosition.SpawnAtPoint;
                                Console.WriteLine(
                                    $"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 即将复活在 {request.SpawnPosition}");
                                break;
                            }
                        }

                        beforePosTime = beforePosTime + 15;
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活在选择点");
                        break;
                    }
                }

                player.positionBef.Clear();

                // TODO 在 Oki 部署了真正的地图边界且地面以上随机出生点后，再使用真正的随机出生点，做 RandomSpawn Points 需要适配地图太多且有任何改动都要重新写数值
                // 由于 Oki 在 Discord 中提及了地图边界的问题以及 SpawnPosition 的不可写问题，所以暂时使用 TDM 模式的固定出生点
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活，MagazineIndex：{request.Loadout.PrimaryWeapon.MagazineIndex}，SkinIndex：{request.Loadout.PrimaryWeapon.SkinIndex}，requestPosition：{request.SpawnPosition.X}，{request.SpawnPosition.Y}，{request.SpawnPosition.Z}。。LookDirection：{request.LookDirection.X}，{request.LookDirection.Y}，{request.LookDirection.Z}");
            }
            catch (Exception ee)
            {
                Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ee.StackTrace}");
            }

            return request;
        }

        // DEVELOP: 在玩家登录时，给玩家定义不同于官方的数据
        public override async Task OnPlayerJoiningToServer(ulong steamID, PlayerJoiningArguments args)
        {
            args.Stats.Progress.Rank = 200;
            args.Stats.Progress.Prestige = 6;

            // DEVELOP TODO: 此处的角色权限设置最好走 Json 配置，后续再升级成 API 读取
            if (steamID == 76561198090800555)
            {
                args.Stats.Roles = Roles.Admin;
            }

            if (steamID == 765611980908011)
            {
                args.Stats.Roles = Roles.Moderator;
            }

            if (steamID == 765611980908022)
            {
                args.Stats.Roles = Roles.Vip;
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
            var player = rankPlayers.Find(o => o.SteamID == steamID);

            player.stats = stats;
        }

        public override async Task OnRoundEnded()
        {
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} ---------- 本局游戏结束 ----------");
            // DEVELOP: 测试时立马开始下一句游戏
            ForceStartGame();
        }
    }
}