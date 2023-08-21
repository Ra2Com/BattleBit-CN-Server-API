using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class EndGame : CommandHandlerBase
    {
        public EndGame()
        {
            commandMessage = "/end";
            helpMessage = "立刻结束本轮对局";
            Aliases = new string[] { "/ed" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.End,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            if (player.GameServer.RoundSettings.State != GameState.EndingGame)
            {
                player.GameServer.SayToChat($"本局游戏已结束！", player.SteamID);
                return;
            }

            player.GameServer.SayToAllChat($"管理员 {player.Name} - 使用命令结束了本局!");
            player.GameServer.ForceEndGame();
            player.GameServer.RoundSettings.SecondsLeft = 3;
            return;
        }
    }
}