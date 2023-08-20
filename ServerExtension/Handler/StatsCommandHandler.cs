using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Tools;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class StatsCommandHandler : CommandHandlerBase
    {
        public StatsCommandHandler()
        {
            commandMessage = "/stats";
            helpMessage = "展示你的对局数据";
            Aliases = new string[] { "/s" };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            var returnInfo = new CommandDTO
            {
                CommandType = CommandTypes.Stats,
                Executor = player.Name,
                Error = false,
            };
            returnInfo.Message =
                $"{RichText.Cyan}{player.Name}{RichText.EndColor} 你好，游戏时长 {TimeUtil.GetPhaseDifference(player.JoinTime)} 分钟 , K/D: {player.K}/{player.D}，排名 {RichText.Orange}{player.rank}{RichText.EndColor}";
            return returnInfo;
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }
    }
}