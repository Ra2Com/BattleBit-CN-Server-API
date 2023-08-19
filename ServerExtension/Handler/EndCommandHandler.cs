using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class EndCommandHandler : CommandHandlerBase
    {
        public EndCommandHandler()
        {
            commandMessage = "/end";
            helpMessage = "立刻结束本轮对局";
            Aliases = new string[] { "/ed" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator};
          
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {

            return new CommandDTO
            {
                CommandType = CommandTypeEnum.End,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            if (player.GameServer.RoundSettings.State != GameState.EndingGame)
            {
                player.GameServer.SayToChat($"管理员 {player.Name} - 本局游戏已结束！");
                return;
            }

            player.GameServer.SayToChat($"管理员 {player.Name} - 使用命令结束了本局!");
            player.GameServer.ForceEndGame();
            player.GameServer.RoundSettings.SecondsLeft = 3;            
            return;
        }
    }
}
