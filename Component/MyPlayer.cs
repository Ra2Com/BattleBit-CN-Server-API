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
        public long JionTime { get; set; } = GetUtcTimeMs();

        public int K { get; set; } = 0;
        public int D { get; set; } = 0;
        public int rank { get; set; } = 0;

        public int Score { get; set; } = 0;
        public ulong markId { get; set; } = 0;
        public float maxHP { get; set; }

        public override async Task OnConnected()
        {
            _ = Task.Run(async () =>
            {
                // Message to display your Killer's distance.
                while (true)
                {
                    await Task.Delay(3000);
                    if (markId != 0)
                    {
                        var markPlayer = GameServer.AllPlayers.First(o => o.SteamID == markId);
                        if (markPlayer == null)
                            markId = 0;
                        else
                        {
                            var dis = Vector3.Distance(markPlayer.Position, this.Position);
                            this.Message($"你的仇人{markPlayer.Name}距你{dis}米");
                        }
                    }
                }
            });
        }

        public override async Task OnDied()
        {
            // Spawn a player when died.
            _ = Task.Run(async () =>
             {
                 await Task.Delay(3000);
                 SpawnPlayer(new PlayerLoadout() { }, new PlayerWearings() { }, new Vector3() { }, new Vector3() { }, PlayerStand.Standing, 3);
             });
        }

        public override async Task OnSpawned()
        {

        }

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
