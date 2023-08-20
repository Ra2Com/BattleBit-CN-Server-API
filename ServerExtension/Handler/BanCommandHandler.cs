using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class BanCommandHandler : CommandHandlerBase
    {
        public BanCommandHandler()
        {
            commandMessage = "/ban";
            helpMessage = "封禁指定的玩家昵称或者 SteamID";
            Aliases = new string[] { "/b" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
            isPrivate = true;
        }

        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypes.Ban,
                Executor = player.Name,
                Error = false
            };
        }

        public override void Execute(MyPlayer player, string cmdMsg)
        {
            var target = cmdMsg.Split(" ")[1..].Aggregate((a, b) => a + " " + b);
            var targetPlayer = player.GameServer.AllPlayers.ToList().FirstOrDefault(p =>
                p.Name.ToLower().Contains(target.ToLower()) || p.SteamID.ToString().Contains(target));

            if (target == null)
            {
                player.GameServer.SayToChat($"未找到要封禁的玩家", player.SteamID);
                return;
            }

            targetPlayer.Ban();
            player.GameServer.AnnounceShort($"{targetPlayer?.Name} 被管理员 {player.Name} 封禁了");
            return;
        }
    }
}