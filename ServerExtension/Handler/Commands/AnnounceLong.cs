using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class AnnounceLong : CommandHandlerBase
    {
        public AnnounceLong()
        {
            commandMessage = "/annlong";
            helpMessage = "发送长公告";
            Aliases = new string[] { "/ann" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.AnnounceLong,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var splits = cmdMsg.Split(" ");
            player.GameServer.AnnounceLong(splits[1]);
            return;
        }
    }
}