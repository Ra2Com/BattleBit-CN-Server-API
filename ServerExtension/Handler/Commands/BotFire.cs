using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class BotFire : CommandHandlerBase
    {
        public BotFire()
        {
            commandMessage = "/botfire";
            helpMessage = "让所有机器人开火";
            Aliases = new string[] { "/bf" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
        }
        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.BotFire,
                Executor = player.Name,
                Error = false
            };
        }
        
        public override void Execute(MyPlayer player, string cmdMsg)
        {
            player.GameServer.ExecuteCommand("bot fire");
        }
    }
}
