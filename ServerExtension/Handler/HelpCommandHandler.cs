using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class HelpCommandHandler : CommandHandlerBase
    {
        public HelpCommandHandler()
        {
            commandMessage = "/help";
            helpMessage = "展示本帮助信息";
            Aliases = new string[] { "/h" };
           
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypeEnum.Help,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }
    }
}
