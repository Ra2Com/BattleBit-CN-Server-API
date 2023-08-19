using System.Runtime.Intrinsics.X86;
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
        public long LastSpeedTime { get; set; }
        public PlayerStats stats { get; set; }
        public List<PositionBef> positionBef { get; set; } = new List<PositionBef>();

        public override async Task OnConnected()
        {
            Console.Out.WriteLineAsync($"MyPlayer OnConnected");

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
            Console.Out.WriteLineAsync($"欢迎 {RichText.Teal}{Name}{RichText.EndColor} ，K/D: {K}/{D}，排名 {RichText.Orange}{rank}{RichText.EndColor} ");
            Message($"{RichText.Cyan}{Name}{RichText.EndColor} 你好，游戏时长 {TimeUtil.GetPhaseDifference(JoinTime)} 分钟 , K/D: {K}/{D}，排名 {RichText.Orange}{rank}{RichText.EndColor}", 3f);

            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (Position.X != 0 && Position.Y != 0)
                        {
                            positionBef.Add(new PositionBef { position = new Vector3() { X = Position.X, Y = Position.Y, Z = Position.Z }, time = TimeUtil.GetUtcTimeMs() });
                            Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {Name} 加入坐标点: {Position}");
                        }

                        // When a player joined the game, send a Message to announce its Community Server data.

                        if (markId != 0)
                        {
                            var markPlayer = GameServer.AllPlayers.FirstOrDefault(o => o.SteamID == markId);
                            if (markPlayer == null)
                                markId = 0;
                            else
                            {
                                float dis = Vector3.Distance(markPlayer.Position, Position);
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
            // Spawn a player when died and give him a new set(example).
            //_ = Task.Run(async () =>
            // {
            //     await Task.Delay(3100);


            //     try
            //     {
            //         int beforePosTime = 15;
            //         var sp = new Vector3() { };
            //         while (true)
            //         {
            //             if (positionBef.TryDequeue(out PositionBef pb))
            //             {
            //                 Console.Out.WriteLineAsync($"{pb.position}");

            //                 if (MyPlayer.GetUtcTimeMs() - (pb.time) > 1000 * beforePosTime)
            //                 {
            //                     if (GameServer.AllPlayers.FirstOrDefault(o => (Vector3.Distance(o.Position, pb.position) < 20f) && o.Team != Team) == null)
            //                     {
            //                         sp = pb.position;
            //                         Console.WriteLine($"{Name}即将复活在{pb.position}");
            //                         break;
            //                     }
            //                 }
            //                 beforePosTime = beforePosTime + 15;
            //             }
            //             else
            //             {
            //                 //request.SpawnPosition = new Vector3();
            //                 Console.WriteLine($"{Name}复活在选择点");
            //                 break;

            //             }
            //         }
            //         SpawnPlayer(SpawnManager.GetRandom(), CurrentWearings, sp, new Vector3() { X = 0, Y = 0, Z = 1 }, PlayerStand.Standing, 5f);


            //         // TODO 在 Oki 部署了真正的地图边界且地面以上随机出生点后，再使用真正的随机出生点，做 RandomSpawn Points 需要适配地图太多且有任何改动都要重新写数值
            //         // 当前随机出生方案，记录玩家 15、30、40、60 秒前的坐标和面朝方位，判断出生坐标的 XYZ <= 20f 内是否有敌人，依次刷新，如果到 60 秒前的坐标仍然不可以刷新，则强制刷新到 60 秒前的坐标，如果依次拉取时取到不存在的值，则强制刷新在 null。无论玩家是选择出生在(重生点、队友、载具还是指定的ABCD点等别的地方）
            //         //request.SpawnPosition = new System.Numerics.Vector3();
            //         //request.LookDirection = new System.Numerics.Vector3();
            //         //Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {player.Name} 复活，MagazineIndex：{request.Loadout.PrimaryWeapon.MagazineIndex}，SkinIndex：{request.Loadout.PrimaryWeapon.SkinIndex}，requestPosition：{request.SpawnPosition.X}，{request.SpawnPosition.Y}，{request.SpawnPosition.Z}。。LookDirection：{request.LookDirection.X}，{request.LookDirection.Y}，{request.LookDirection.Z}");
            //     }
            //     catch (Exception ee)
            //     {
            //         Console.Out.WriteLineAsync(ee.StackTrace);

            //     }

            // });
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

        // Time calculation stuff
       
    }

    public class PositionBef
    {
        public long time { get; set; }

        public Vector3 position { get; set; }
    }
}
