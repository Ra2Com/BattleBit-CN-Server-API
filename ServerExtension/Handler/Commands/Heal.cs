using CommunityServerAPI.BattleBitAPI.Common.Enums;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Utils;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Heal : CommandHandlerBase
    {
        public Heal()
        {
            commandMessage = "/heal";
            helpMessage = "治疗自己 100 生命值，每 2 分钟只能使用一次";
            Aliases = new string[] { "/jx", "/hp" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator, Roles.Vip };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Heal,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var timeDiff = TimeUtil.GetUtcTime(DateTime.Now) - player.LastHealTime;

            if (timeDiff < 120)
            {
                player.GameServer.SayToChat("你还不能使用治疗技能，剩余时间：" + (120 - timeDiff) + " 秒", player.SteamID);
                return;
            }
            else
            {
                player.Heal(100);
                player.LastHealTime = TimeUtil.GetUtcTime(DateTime.Now);
            }

            return;
        }
    }
}