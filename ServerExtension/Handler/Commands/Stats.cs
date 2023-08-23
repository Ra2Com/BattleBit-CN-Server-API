using BattleBitAPI.Common;
using CommunityServerAPI.Content;
using CommunityServerAPI.ServerExtension;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Utils;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Stats : CommandHandlerBase
    {
        public Stats()
        {
            commandMessage = "/stats";
            helpMessage = "展示你的对局数据";
            Aliases = new string[] { "/s" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator, Roles.Vip, Roles.Special, Roles.None };
            isPrivate = false;
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            var returnInfo = new CommandDTO
            {
                CommandType = CommandTypes.Stats,
                Executor = player.Name,
                Error = false,
            };
            var JIAQUN = MessageOfTheDayManager.GetMOTD("JoinMethodQun");
            returnInfo.Message =
                $"{RichText.Cyan}{player.Name}{RichText.EndColor} 你好，游戏时长 " +
                $"{player.stats.Progress.PlayTimeSeconds / 60} 分钟 , " +
                $"K/D: {player.stats.Progress.KillCount}/{player.stats.Progress.DeathCount}，" +
                $"排名 {RichText.Orange}{player.rank}{RichText.EndColor}" +
                $"{RichText.BR}" +
                $"{JIAQUN}";
            return returnInfo;
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }
    }
}