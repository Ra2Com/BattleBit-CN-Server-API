using BattleBitAPI;
using BattleBitAPI.Common;
using CommunityServerAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Model
{
    public class MyPlayer : Player<MyPlayer>, IPlayerInfo
    {
        // DEVELOP TODO: 玩家离线、没有复活时要停止计时
        public long JoinTime { get; set; } = TimeUtil.GetUtcTimeMs();

        public int K { get; set; } = 0;
        public int D { get; set; } = 0;
        public int rank { get; set; } = 1;

        public int Score { get; set; } = 0;
        public ulong markId { get; set; } = 0;
        public float maxHP { get; set; }
        
        public long LastHealTime { get; set; } = TimeUtil.GetUtcTimeMs();
        public long LastSpeedTime { get; set; } = TimeUtil.GetUtcTimeMs();

        public PlayerStats stats { get; set; }
        public List<PositionBef> positionBef { get; set; } = new List<PositionBef>();

        public override async Task OnConnected()
        {
            Console.Out.WriteLineAsync($"MyPlayer 进程已连接");

            // 特殊角色登录日志
            if (stats?.Roles == Roles.Admin)
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 超级管理员 {SteamID} 已连接, IP: {IP}");
            }
            if (stats?.Roles == Roles.Moderator)
            {
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 管理员 {SteamID} 已连接, IP: {IP}");
            }

            // 同时添加 Say 聊天消息
            GameServer.SayToChat($"{RichText.Teal}QQ群：887245025{RichText.EndColor}，欢迎 {RichText.Teal}{Name}{RichText.EndColor}，排名 {RichText.Orange}{rank}{RichText.EndColor} 进服");
            await Console.Out.WriteLineAsync($"{RichText.Joy}欢迎 {RichText.Teal}{Name}{RichText.EndColor} ，K/D: {K}/{D}，排名 {RichText.Orange}{rank}{RichText.EndColor} ");
            Message($"{RichText.Joy}{RichText.Cyan}{Name}{RichText.EndColor} 你好" +
                    $"{RichText.LineBreak}你的游戏时长 {TimeUtil.GetPhaseDifference(JoinTime)} 分钟 , K/D: {K}/{D}" +
                    $"{RichText.LineBreak}当前排名 {RichText.Orange}{rank}{RichText.EndColor}" +
                    $"{RichText.LineBreak}" +
                    $"{RichText.LineBreak}{RichText.Patreon}{RichText.Red}===请注意==={RichText.EndColor}" +
                    $"{RichText.LineBreak}本服务器为社区服，你所有获得的游戏或装备进度都将只存在本服务器，不与官方服务器共享数据" +
                    $"{RichText.LineBreak}" +
                    $"{RichText.LineBreak}玩家 QQ群：887245025", 5f);

            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (Position.X != 0 && Position.Y != 0)
                        {
                            positionBef.Add(new PositionBef { position = new Vector3() { X = Position.X, Y = Position.Y, Z = Position.Z }, time = TimeUtil.GetUtcTimeMs() });
                            await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {Name} 加入坐标点: {Position}");
                        }

                        // When a player joined the game, send a Message to announce its Community Server data.

                        if (markId != 0)
                        {
                            var markPlayer = GameServer.AllPlayers.FirstOrDefault(o => o.SteamID == markId);
                            if (markPlayer == null)
                                markId = 0;
                            else
                            {
                                var dis = Vector3.Distance(markPlayer.Position, Position).ToString("#0.0");
                                // DEVELOP TODO: 如果他在成为你的仇人之后死亡了（包括自杀、退出服务器），都要清除此消息
                                Message($"仇人 {RichText.Red}{markPlayer.Name}{RichText.EndColor} 距你 {RichText.Navy}{dis}{RichText.EndColor} 米");
                                Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 玩家{Name}：K/D: {K}/{D},仇人 {markId}");

                            }
                        }
                        await Task.Delay(3000);
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

            // 娱乐服，换弹速度降低到 70%
            Modifications.ReloadSpeedMultiplier = 0.7f;

            // 白天，用个鬼的夜视仪
            Modifications.CanUseNightVision = false;

            // 倒地后马上就死
            Modifications.DownTimeGiveUpTime = 0.1f;

            // 更拟真一点，学学 CSGO 跳跃转向丢失速度
            Modifications.AirStrafe = false;

            // 死了马上就能活
            Modifications.RespawnTime = 1f;

            // 开启击杀通知
            Modifications.KillFeed = true;

            // 刚枪服务器，所有武器伤害值都降低到 75%
            Modifications.GiveDamageMultiplier = 0.75f;
        }
    }

    public class PositionBef
    {
        public long time { get; set; }

        public Vector3 position { get; set; }
    }
}
