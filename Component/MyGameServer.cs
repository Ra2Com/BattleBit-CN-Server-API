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
            ForceStartGame();

            ServerSettings.PointLogEnabled = false;
        }

        List<IPlayerInfo> rankPlayers = new List<IPlayerInfo>();

        public override async Task OnPlayerConnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync("Connected: " + player);
        }
        public override async Task OnPlayerSpawned(MyPlayer player)
        {
            // When a player joined the game, send a Message to announce its Community Server data.
            // todo: 添加 Say 聊天消息
            // 添加 Message 消失时间
            // 确认玩家连接进入后就可以收到，而不是每次复活提示这个，复活应该提示战斗后的消息。
            
        }
        public override async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
        {
            if (args.Killer != null)
            {
                // Basic Revenger mode function, kills victim if it's down, add Killer's data, do Random Mode's work. etc.
                args.Killer.K++;
                args.Victim.Kill();
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
                MessageToPlayer(args.Victim, $"你被{args.Killer.Name}击杀，对方剩余血量{args.Killer.HP}");
            }

            //await Console.Out.WriteLineAsync("Downed: " + args.Victim);
        }
        public override async Task OnPlayerGivenUp(MyPlayer player)
        {
            await Console.Out.WriteLineAsync("Giveup: " + player);
        }
        public override async Task OnPlayerDied(MyPlayer player)
        {
            player.D++;
            //await Console.Out.WriteLineAsync("Died: " + player);
        }
        public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
        {
            await Console.Out.WriteLineAsync(from + " revived " + to);
        }
        public override async Task OnPlayerDisconnected(MyPlayer player)
        {
            await Console.Out.WriteLineAsync("Disconnected: " + player);
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
