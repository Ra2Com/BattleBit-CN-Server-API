using BattleBitAPI;
using BattleBitAPI.Common;
using Org.BouncyCastle.Ocsp;
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

        // DEVELOP: 在玩家登录时，给玩家定义不同于官方的数据
        public override Task<PlayerStats> OnGetPlayerStats(ulong steamID, PlayerStats officialStats)
        {
            officialStats.Progress.Rank = 200;
            officialStats.Progress.Prestige = 6;
            return Task.FromResult(officialStats);
        }

        public override async Task OnConnected()
        {
            _ = Task.Run(async () =>
            {
                // Message to display your Killer's distance.
                while (true)
                {
                    // When a player joined the game, send a Message to announce its Community Server data.
                    await Task.Delay(3000);
                    Message($"{RichText.Cyan(Name)}你好，游戏时长{MyPlayer.GetPhaseDifference(JoinTime)} , K/D: {K}/{D}，排名{RichText.Orange(rank)}", 3f);
                    // TODO: 同时添加 Say 聊天消息
                    SayToChat($"欢迎 {RichText.Purple(Name)} , K/D: {K}/{D}，排名{RichText.Orange(rank)}");

                    if (markId != 0)
                    {
                        var markPlayer = GameServer.AllPlayers.First(o => o.SteamID == markId);
                        if (markPlayer == null)
                            markId = 0;
                        else
                        {
                            var dis = Vector3.Distance(markPlayer.Position, this.Position);
                            this.Message($"你的仇人{RichText.Red(markPlayer.Name)}距你{dis}米", 3f);
                        }
                    }
                }
            });
        }

        // 聊天监控和命令
        public override async Task<bool> OnPlayerTypedMessage(MyPlayer player, ChatChannel channel, string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - " + player.Name + "在「" + channel + "」发送聊天 - " + msg);
            // TODO: 聊天记录建议单独保存
            // TODO: 屏蔽词告警
            // TODO: 屏蔽词系统

            // 管理员命令执行
            if (player.SteamID != 76561198395073327 || !msg.StartsWith("/")) return true;

        }

        public override async Task OnDied()
        {
            // Spawn a player when died and give him a new set(example).
            // QUESTION: 一般设置玩家的道具都是在 OnPlayerSpawning 中，这样不管玩家在死亡的时候更换什么道具都将被覆盖掉
            // TODO: 最好按照 /Config/WeaponData.json 内容进行配置，方便后面修改数值
            _ = Task.Run(async () =>
             {
                 await Task.Delay(3000);
                 PlayerLoadout playerLoadout = new PlayerLoadout();
                 //主武器
                 playerLoadout.PrimaryWeapon.Tool = Weapons.AK74;
                 playerLoadout.PrimaryWeapon.SetAttachment(Attachments.Ranger);
                 playerLoadout.PrimaryWeapon.SetAttachment(Attachments.Holographic);
                 playerLoadout.PrimaryWeapon.SetAttachment(Attachments.VerticalGrip);
                 playerLoadout.PrimaryWeapon.SetAttachment(Attachments.TacticalFlashlight);
                 //手枪
                 playerLoadout.SecondaryWeapon.Tool = Weapons.USP;
                 playerLoadout.SecondaryWeapon.SetAttachment(Attachments.PistolRedDot);
                 //主附件池
                 playerLoadout.HeavyGadget = Gadgets.C4;
                 //轻附件
                 playerLoadout.LightGadget = Gadgets.SmallAmmoKit;
                 //手雷
                 playerLoadout.Throwable = Gadgets.Flashbang;

                 SpawnPlayer(playerLoadout, CurrentWearings, new Vector3() { }, new Vector3() { }, PlayerStand.Standing, 3);
             });
        }


        public override async Task OnSpawned()
        {

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
}
