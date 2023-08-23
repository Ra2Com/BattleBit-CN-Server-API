using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;
namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class ServerCommands : CommandHandlerBase
    {
        public ServerCommands()
        {
            commandMessage = "/opsv";
            helpMessage = "发送任意超管服务器指令";
            Aliases = new string[] { "/sv" };
            roles = new List<Roles>() { Roles.Admin };
            isPrivate = true;
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.ServerCommands,
                Executor = player.Name,
                Error = false
            };
        }
        public override void Execute(MyPlayer player, string cmdMsg)
        {
            cmdMsg = cmdMsg.Substring(cmdMsg.IndexOf(" ") + 1);
            player.GameServer.ExecuteCommand(cmdMsg);
        }
    }
}
