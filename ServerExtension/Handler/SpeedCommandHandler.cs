using CommunityServerAPI.ServerExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class SpeedCommandHandler:CommandHandlerBase
    {
        public SpeedCommandHandler()
        {
            // TODO: VIP 的增加移动速度 10% 命令，每 1分钟只能用一次
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            return;
        }
    }
}
