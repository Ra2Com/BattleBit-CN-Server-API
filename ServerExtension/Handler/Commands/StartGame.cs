using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class StartGame : CommandHandlerBase
    {
        public StartGame()
        {
            commandMessage = "/start";
            helpMessage = "立刻开始本轮对局";
            Aliases = new string[] { "/s" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Start,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            if (player.GameServer.RoundSettings.State != GameState.WaitingForPlayers &&
                player.GameServer.RoundSettings.State != GameState.CountingDown)
            {
                player.GameServer.SayToChat($"本局游戏已经开始了！", player.SteamID);
                return;
            }

            player.GameServer.SayToAllChat($"管理员 {player.Name} - 使用命令开始了本局!");
            player.GameServer.ForceStartGame();
            player.GameServer.RoundSettings.SecondsLeft = 3;
            return;
        }
    }
}