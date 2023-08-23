using BattleBitAPI;
using BattleBitAPI.Common;
using CommunityServerAPI.Content;
using CommunityServerAPI.Player;
using CommunityServerAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Model
{
    public class MyPlayer : Player<MyPlayer>, IPlayerInfo
    {
        public int rank { get; set; } = 1;
        public float HSRate { get; set; }

        public int Score { get; set; } = 0;
        public ulong markId { get; set; } = 0;

        public long LastHealTime { get; set; } = TimeUtil.GetUtcTime(DateTime.Now);
        public long LastSpeedTime { get; set; } = TimeUtil.GetUtcTime(DateTime.Now);

        public PlayerStats stats { get; set; } = new PlayerStats();
        public List<PositionBef> positionBef { get; set; } = new List<PositionBef>();

        public override async Task OnConnected()
        {
            Console.Out.WriteLineAsync($"MyPlayer 进程已连接");
            markId = 0;
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                PlayerMOTD();
            });

            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        // if (Position.X != 0 && Position.Y != 0)
                        // {
                        //     positionBef.Add(new PositionBef { position = new Vector3() { X = Position.X, Y = Position.Y, Z = Position.Z }, time = TimeUtil.GetUtcTimeMs() });
                        //     await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {Name} 加入坐标点: {Position}");
                        // }

                        if (markId != 0 && markId != SteamID)
                        {
                            var markPlayer = GameServer.AllPlayers.FirstOrDefault(o => o.SteamID == markId);
                            if (markPlayer == null)
                                markId = 0;
                            else
                            {
                                var dis = Vector3.Distance(markPlayer.Position, Position).ToString("#0.0");
                                Message($"仇人 {RichText.Red}{markPlayer.Name}{RichText.EndColor} 距你 {RichText.Red}{dis}{RichText.EndColor} 米");
                                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家{Name}：K/D: {stats.Progress.KillCount}/{stats.Progress.DeathCount},仇人 {markId}");

                            }
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ee)
                {
                    Console.Out.WriteLineAsync(ee.StackTrace);
                }

            });
        }




        public override async Task OnDied()
        {


        }


        public override async Task OnSpawned()
        {
            // 娱乐服，咱不玩流血那套
            Modifications.DisableBleeding();

            // 娱乐服，换弹速度降低到 75%
            Modifications.ReloadSpeedMultiplier = 1.35f;

            // 白天，用个鬼的夜视仪
            Modifications.CanUseNightVision = false;

            // 倒地后马上就死
            Modifications.DownTimeGiveUpTime = 1f;

            // 更拟真一点，学学 CSGO 跳跃转向丢失速度
            Modifications.AirStrafe = false;

            // 死了马上就能活
            Modifications.RespawnTime = 0.5f;

            // 开启击杀通知
            Modifications.KillFeed = true;

            // 刚枪服务器，所有武器伤害值都降低到 75%
            Modifications.GiveDamageMultiplier = 0.75f;
        }

        public override async Task OnSessionChanged(long oldSessionID, long newSessionID)
        {
            markId = 0;
            await PlayerMOTD();

        }

        private async Task PlayerMOTD()
        {
            // When a player joined the game, send a Message to announce its Community Server data.
            var JIAQUN = MessageOfTheDayManager.GetMOTD("JoinMethodQun");
            var MOTD = MessageOfTheDayManager.GetMOTD("WelcomeMsg");
            GameServer.SayToChat($"{RichText.Cyan}{JIAQUN}{RichText.EndColor}，欢迎 {RichText.Olive}{Name}{RichText.EndColor}，排名 {RichText.Orange}{rank}{RichText.EndColor} 进服", SteamID);
            Message($"{RichText.Vip}{RichText.Cyan}{Name}{RichText.EndColor} 你好" +
                    $"{RichText.BR}游戏时长 {this.stats.Progress.PlayTimeSeconds / 60} 分钟 , K/D: {stats.Progress.KillCount}/{stats.Progress.DeathCount} , 爆头 {stats.Progress.Headshots} 次" +
                    $"{RichText.BR}当前排名 {RichText.Orange}{rank}{RichText.EndColor}" +
                    $"{RichText.BR}" +
                    $"{RichText.BR}{RichText.LightBlue}===请注意==={RichText.EndColor}" +
                    $"{RichText.BR}{MOTD}" +
                    $"{RichText.BR}" +
                    $"{RichText.BR}{JIAQUN}", 30f);
        }

    }

    public class PositionBef
    {
        public long time { get; set; }

        public Vector3 position { get; set; }
    }
}
