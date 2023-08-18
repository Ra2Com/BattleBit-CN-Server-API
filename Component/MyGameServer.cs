using System.Net.Cache;
using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.Component
{
    internal class MyGameServer : GameServer<MyPlayer>
    {
        public override async Task OnConnected()
        {
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 已与游戏服务器 {ServerName} 建立通信 - {GameIP}:{GamePort}");
            
            // 固定 Random Revenge 的游戏模式和游戏地图
            MapRotation.SetRotation("Salhan", "Wakistan", "Construction", "District");
            GamemodeRotation.SetRotation("Domination");

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
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家 {player.Name} - {player.SteamID} 已连接, IP: {player.IP}");
        }

        public override async Task OnPlayerSpawned(MyPlayer player)
        {


        }

        public override async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
        {

            if (args.BodyPart > 0 && args.BodyPart < PlayerBody.Shoulder)
            {
                // TODO: 记录玩家爆头击杀数
            }

            if (args.Killer != null)
            {
                // Basic Revenger mode function, kills victim if it's down, add Killer's data, do Random Mode's work. etc.
                args.Killer.K++;
                args.Victim.Kill();
                // TODO: 如果击杀的是仇人，且仇人在复活后没有死亡，仇人队伍的 Tickets 要扣除 10（配置项）
                PlayerLoadout victimLoadout = args.Victim.CurrentLoadout;
                args.Killer.SetFirstAidGadget(victimLoadout.FirstAidName, 0, true);
                args.Killer.SetThrowable(victimLoadout.ThrowableName, 0, false);
                args.Killer.SetHeavyGadget(victimLoadout.HeavyGadgetName, 0, false);
                args.Killer.SetLightGadget(victimLoadout.LightGadgetName, 0, false);
                args.Killer.SetSecondaryWeapon(victimLoadout.SecondaryWeapon, 0, false);
                args.Killer.SetPrimaryWeapon(victimLoadout.PrimaryWeapon, 0, false);
                args.Killer.Heal(20);
                args.Victim.markId = args.Killer.SteamID;
                // Announce the victim your killer. And the killer will be tracked.
                // TODO: 如果他在成为你的仇人之后死亡了（包括自杀、退出服务器），都要清除此消息
                MessageToPlayer(args.Victim, $"你被{RichText.Red}{args.Killer.Name}{RichText.EndColor}击杀，敌人剩余血量 {RichText.Green}{args.Killer.HP}{RichText.EndColor}");
            }

            //await Console.Out.WriteLineAsync("Downed: " + args.Victim);
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
        public override async Task OnPlayerDisconnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家已离线: " + player);
        }

        public override async Task OnTick()
        {
            // TODO: 每 2 分钟发布一条 AnnounceShort 让玩家加群反馈
            // TODO: 每 3 分钟发布一条 全服聊天信息 让玩家加群反馈
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

        public override async Task<OnPlayerSpawnArguments> OnPlayerSpawning(MyPlayer player, OnPlayerSpawnArguments request)
        {
            request.Loadout = SpawnManager.GetRandom(); // 出生后随机装备
            request.SpawnStand = PlayerStand.Standing; // 站着出生
            request.SpawnProtection = 5f; // 出生不动保护 5 秒
            // TODO 在 Oki 部署了真正的地图边界且地面以上随机出生点后，再使用真正的随机出生点，做 RandomSpawn Points 需要适配地图太多且有任何改动都要重新写数值
            // 当前随机出生方案，记录玩家 15、30、40、60 秒前的坐标和面朝方位，判断出生坐标的 XYZ <= 20f 内是否有敌人，依次刷新，如果到 60 秒前的坐标仍然不可以刷新，则强制刷新到 60 秒前的坐标，如果依次拉取时取到不存在的值，则强制刷新在 null。无论玩家是选择出生在(重生点、队友、载具还是指定的ABCD点等别的地方）
            //request.SpawnPosition = new System.Numerics.Vector3();
            //request.LookDirection = new System.Numerics.Vector3();
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活，MagazineIndex：{request.Loadout.PrimaryWeapon.MagazineIndex}，SkinIndex：{request.Loadout.PrimaryWeapon.SkinIndex}，requestPosition：{request.SpawnPosition.X}，{request.SpawnPosition.Y}，{request.SpawnPosition.Z}。。LookDirection：{request.LookDirection.X}，{request.LookDirection.Y}，{request.LookDirection.Z}");
            return request;
        }

        // DEVELOP: 在玩家登录时，给玩家定义不同于官方的数据
        public override async Task OnPlayerJoiningToServer(ulong steamID, PlayerJoiningArguments args)
        {
            args.Stats.Progress.Rank = 200;
            args.Stats.Progress.Prestige = 6;

            // TODO: 此处的 Admin 角色最好走 Json 配置
            if (steamID == 76561198090800555)
            {
                args.Stats.Roles = Roles.Admin;
            }
        }

        // 聊天监控和命令
        public override async Task<bool> OnPlayerTypedMessage(MyPlayer player, ChatChannel channel, string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - " + player.Name + "在「" + channel + "」发送聊天 - " + msg);
            // TODO: 聊天记录建议单独保存
            // TODO: 屏蔽词告警
            // TODO: 屏蔽词系统

            // 管理员判断以及命令执行
            // TODO: 管理员类命令执行结果需要打印 Log
            if (player.Stats.Roles != Roles.Admin || !msg.StartsWith("/"))
            {

                return true;
            }

            if (player.Stats.Roles != (Roles.Admin || Roles.Moderator) || !msg.StartsWith("/"))
            {

                return true;
            }

            return false;

        }
    }
}
