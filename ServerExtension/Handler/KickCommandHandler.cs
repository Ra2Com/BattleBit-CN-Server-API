using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class KickCommandHandler : CommandHandlerBase
    {
        public KickCommandHandler()
        {
            commandMessage = "/kick";
            helpMessage = "指定玩家昵称或者 SteamID 踢出玩家";
            Aliases = new string[] { "/k" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Kick,
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
                player.GameServer.SayToChat($"管理员 {player.Name} - 未找到要踢出的玩家");
                return;
            }

            targetPlayer.Kick();
            player.GameServer.SayToChat($"{targetPlayer?.Name} 被管理员 {player.Name} 踢出");
            return;
        }
    }
}