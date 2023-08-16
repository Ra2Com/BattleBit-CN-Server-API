using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
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
            Console.WriteLine($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 已与游戏服务器建立通信! {GameIP}:{GamePort} {ServerName}");
            // 固定 Random Revenge 的游戏模式和游戏地图
            MapRotation.SetRotation("Salhan", "Wakistan", "Construction", "District");
            GamemodeRotation.SetRotation("Domination");

            // 娱乐服，咱不玩流血那套
            ServerSettings.BleedingEnabled = false;

            // TODO: 这些数值配置最好都到一个 Json 解析配置类里面去
            RoundSettings.MaxTickets = 1500;

            // 2个玩家就可以开干了
            RoundSettings.PlayersToStart = 2;

            // 测试用途 For development test ONLY
            ForceStartGame();

            // 不知道干啥的游戏设置
            ServerSettings.PointLogEnabled = false;
        }

        List<IPlayerInfo> rankPlayers = new List<IPlayerInfo>();

        public override async Task OnPlayerConnected(MyPlayer player)
        {
            Console.WriteLine($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 玩家 {player.Name} - {player.SteamID} 已连接, IP: {player.IP}");
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
                MessageToPlayer(args.Victim, $"你被{RichText.Red({args.Killer.Name})}击杀，他还有 {RichText.Bold({args.Killer.HP})} 血");
            }

            //await Console.Out.WriteLineAsync("Downed: " + args.Victim);
        }
        public override async Task OnPlayerGivenUp(MyPlayer player)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 玩家已放弃: " + player);
        }
        public override async Task OnPlayerDied(MyPlayer player)
        {
            player.D++;
            //await Console.Out.WriteLineAsync("Died: " + player);
        }
        public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - " + from + " 复活了 " + to);
        }
        public override async Task OnPlayerDisconnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 玩家已离线: " + player);
        }

        public override async Task OnTick()
        {
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
    }
}
