using CommunityServerAPI.BattleBitAPI.Common.Enums;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Kick : CommandHandlerBase
    {
        public Kick()
        {
            commandMessage = "/kick";
            helpMessage = "指定玩家昵称或者 SteamID 踢出玩家";
            Aliases = new string[] { "/k" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
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
                player.GameServer.SayToChat($"未找到要踢出的玩家", player.SteamID);
                return;
            }

            targetPlayer.Kick();
            player.GameServer.SayToAllChat($"{targetPlayer?.Name} 被管理员 {player.Name} 踢出");
            return;
        }
    }
}