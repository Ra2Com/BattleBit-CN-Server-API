using CommunityServerAPI.ServerExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class HealCommandHandler : CommandHandlerBase
    {
        public HealCommandHandler()
        {
            // commandMessage = "/heal";
            // helpMessage = "治疗自己 100 生命值，每分钟只能使用一次";
            // Aliases = new string[] { "/h" };
            // needVIP = True;
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }

        // public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel, string msg)
        // {
        //     return new CommandDTO
        //     {
        // TODO: VIP 的治疗命令，每 1分钟只能用一次
        //         Action = CommandType.Heal,
        //         Executor = player.Name,
        //         Error = false,
        //     };
        // }
    }
}
