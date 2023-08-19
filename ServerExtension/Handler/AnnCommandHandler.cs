using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler

{
    public class AnnCommandHandler : CommandHandlerBase
    {
        public AnnCommandHandler()
        {
            commandMessage = "/announce";
            helpMessage = "发送短公告";
            Aliases = new string[] { "/an" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Announce,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var splits = cmdMsg.Split(" ");
            player.GameServer.AnnounceShort(splits[1]);
            return;
        }
    }
}