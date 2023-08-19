using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                CommandType = CommandTypeEnum.Stats,
                Executor = player.Name,
                Error = false,
               
            };
            returnInfo.Message = $"{RichText.Cyan}{player.Name}{RichText.EndColor} 你好，游戏时长{TimeUtil.GetPhaseDifference(player.JoinTime)} , K/D: {player.K}/{player.D}，排名 {RichText.Orange}{player.rank}{RichText.EndColor}";
            return returnInfo;
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }
    }
}
