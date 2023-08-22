using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class RemoveBot : CommandHandlerBase
    {
        public RemoveBot()
        {
            commandMessage = "/removebot";
            helpMessage = "删除所有机器人";
            Aliases = new string[] { "/rb" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
        }
        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.RemoveBot,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            player.GameServer.ExecuteCommand("remove bot");
        }
    }
}