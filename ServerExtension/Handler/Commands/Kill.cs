using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Kill : CommandHandlerBase
    {
        public Kill()
        {
            commandMessage = "/slay";
            helpMessage = "通过玩家昵称或者 SteamID 杀死玩家";
            Aliases = new string[] { };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Kill,
                Executor = player.Name,
                Error = false,
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var target = cmdMsg.Split(" ")[1..].Aggregate((a, b) => a + " " + b);
            var targetPlayer = player.GameServer.AllPlayers.ToList().FirstOrDefault(p =>
                p.Name.ToLower().Contains(target.ToLower()) || p.SteamID.ToString().Contains(target));

            if (target == null)
            {
                player.GameServer.SayToChat($"未找到要杀死的玩家", player.SteamID);
                return;
            }

            targetPlayer?.Kill();
            player.GameServer.SayToAllChat($"管理员 {player.Name} 使用命令杀死 {targetPlayer?.Name}");
            return;
        }
    }
}