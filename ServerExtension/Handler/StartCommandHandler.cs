using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class StartCommandHandler : CommandHandlerBase
    {
        public StartCommandHandler()
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
                CommandType = CommandTypeEnum.Start,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            if (player.GameServer.RoundSettings.State != GameState.WaitingForPlayers && player.GameServer.RoundSettings.State != GameState.CountingDown)
            {
                player.GameServer.SayToChat($"管理员 {player.Name} - 本局游戏已经开始了！");
                return;
            }

            player.GameServer.SayToChat($"管理员 {player.Name} - 使用命令开始了本局!");
            player.GameServer.ForceStartGame();
            player.GameServer.RoundSettings.SecondsLeft = 3;
            return;
        }
    }
}
