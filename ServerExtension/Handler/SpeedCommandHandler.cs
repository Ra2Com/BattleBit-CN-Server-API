using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.Tools;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class SpeedCommandHandler : CommandHandlerBase
    {
        public SpeedCommandHandler()
        {
            commandMessage = "/speed";
            helpMessage = "增加自己 10% 的移动速度，每 2 分钟只能使用一次";
            Aliases = new string[] { "/sp", "/js" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator, Roles.Vip };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Speed,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var timeDiff = TimeUtil.GetUtcTime(DateTime.Now) - player.LastSpeedTime;

            if (timeDiff < 120)
            {
                player.GameServer.SayToChat("你还不能使用加速技能，剩余时间：" + (120 - timeDiff) + " 秒", player.SteamID);
                return;
            }
            else
            {
                player.Modifications.RunningSpeedMultiplier = 1.1f;
                player.LastSpeedTime = TimeUtil.GetUtcTime(DateTime.Now);
            }

            return;
        }
    }
}