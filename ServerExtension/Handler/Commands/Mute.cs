using CommunityServerAPI.BattleBitAPI.Common.Enums;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler.Commands
{
    public class Mute : CommandHandlerBase
    {
        public Mute()
        {
            commandMessage = "/mute";
            helpMessage = "禁言指定的玩家昵称或者 SteamID";
            Aliases = new string[] { "/m" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Mute,
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
                player.GameServer.SayToChat($"未找到要禁言的玩家", player.SteamID);
                return;
            }
            else if (targetPlayer.Modifications.IsTextChatMuted)
            {
                player.GameServer.SayToChat($"玩家 {targetPlayer?.Name} 已经被禁言过了", player.SteamID);
                return;
            }

            targetPlayer.Modifications.IsTextChatMuted = true;
            player.GameServer.SayToAllChat($"管理员 {player.Name} 禁言了 {targetPlayer?.Name}");
            return;
        }
    }
}