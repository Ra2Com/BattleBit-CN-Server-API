using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class AddBot : CommandHandlerBase
    {
        public AddBot()
        {
            commandMessage = "/addbot";
            helpMessage = "添加一个机器人";
            Aliases = new string[] { "/ab" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
        }
        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.AddBot,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var splits = cmdMsg.Split(" ");
            player.GameServer.ExecuteCommand("join bot" + splits[1]);
        }
    }
}