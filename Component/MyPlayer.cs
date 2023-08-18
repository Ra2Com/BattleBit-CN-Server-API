using BattleBitAPI;
using BattleBitAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.Component
{
    internal class MyPlayer : Player<MyPlayer>, IPlayerInfo
    {
        public long JoinTime { get; set; } = GetUtcTimeMs();

        public int K { get; set; } = 0;
        public int D { get; set; } = 0;
        public int rank { get; set; } = 1;

        public int Score { get; set; } = 0;
        public ulong markId { get; set; } = 0;
        public float maxHP { get; set; }
        public PlayerStats stats { get; set; }
        public Queue<PositionBef> positionBef { get; set; } = new Queue<PositionBef>(10);

        public override async Task OnConnected()
        {
            // 特殊角色登录日志
            if (stats.Roles == Roles.Admin)
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 超级管理员 {SteamID} 已连接, IP: {IP}");
            }
            if (stats.Roles == Roles.Moderator)
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 管理员 {SteamID} 已连接, IP: {IP}");
            }

            _ = Task.Run(async () =>
            {

                // 同时添加 Say 聊天消息
                GameServer.SayToChat($"欢迎 {RichText.Purple}{Name}{RichText.EndColor} ，K/D: {K}/{D}，排名 {RichText.Orange}{rank}{RichText.EndColor} ");

                // Message to display your Killer's distance and welcome msg.
                while (true)
                {
                    positionBef.Enqueue(new PositionBef { position = Position, time = GetUtcTimeMs() });
                    // When a player joined the game, send a Message to announce its Community Server data.
                    await Task.Delay(3000);
                    Message($"{RichText.Cyan}{Name}{RichText.EndColor} 你好，游戏时长{MyPlayer.GetPhaseDifference(JoinTime)} , K/D: {K}/{D}，排名 {RichText.Orange}{rank}{RichText.EndColor}", 3f);

                    if (markId != 0)
                    {
                        var markPlayer = GameServer.AllPlayers.First(o => o.SteamID == markId);
                        if (markPlayer == null)
                            markId = 0;
                        else
                        {
                            var dis = Vector3.Distance(markPlayer.Position, this.Position);
                            this.Message($"仇人 {RichText.Red}{markPlayer.Name}{RichText.EndColor} 距你 {dis} 米");
                        }
                    }
                }
            });
        }



        public override async Task OnDied()
        {
            // Spawn a player when died and give him a new set(example).
            _ = Task.Run(async () =>
             {
                 await Task.Delay(1000);


                 SpawnPlayer(new PlayerLoadout { }, new PlayerWearings { }, new Vector3() { }, new Vector3() { }, PlayerStand.Standing, 1);
             });
        }


        public override async Task OnSpawned()
        {
            // 娱乐服，咱不玩流血那套
            Modifications.DisableBleeding();

            // 娱乐服，换弹速度降低到 70%
            Modifications.ReloadSpeedMultiplier = 0.7f;

            // 白天，用个鬼的夜视仪
            Modifications.CanUseNightVision = false;

            // 倒地后马上就死
            Modifications.DownTimeGiveUpTime = 1f;

            // 更拟真一点，学学 CSGO 跳跃转向丢失速度
            Modifications.AirStrafe = false;

            // 死了马上就能活
            Modifications.RespawnTime = 1f;

            // 开启击杀通知
            Modifications.KillFeed = true;

            // 刚枪服务器，所有武器伤害值都降低到 75%
            Modifications.GiveDamageMultiplier = 0.75f;
        }

        // Time calculation stuff
        public static string GetPhaseDifference(long oldtime)
        {
            var dif = GetUtcTimeMs() - oldtime;
            return (dif / 1000 / 60).ToString() + "分钟";
        }
        public static long GetUtcTimeMs()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        public static long GetUtcTime(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

    }

    public class PositionBef
    {
        public long time { get; set; }

        public Vector3 position { get; set; }
    }
}
