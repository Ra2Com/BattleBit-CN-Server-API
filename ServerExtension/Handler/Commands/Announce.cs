using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Announce : CommandHandlerBase
    {
        public Announce()
        {
            commandMessage = "/announce";
            helpMessage = "发送短公告";
            Aliases = new[] { "/an" };
            roles = new List<Roles> { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Announce,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var splits = cmdMsg.Split(" ");
            player.GameServer.AnnounceShort(splits[1]);
        }
    }
}