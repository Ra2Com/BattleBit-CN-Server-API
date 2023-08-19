using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.ServerExtension.Enums;

namespace CommunityServerAPI.ServerExtension.Handler
{
    public class MuteCommandHandler : CommandHandlerBase
    {
        public MuteCommandHandler()
        {
            commandMessage = "/mute";
            helpMessage = "禁言指定的玩家昵称或者 SteamID";
            Aliases = new string[] { "/m" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
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
           var targetPlayer = player.GameServer.AllPlayers.ToList().FirstOrDefault(p => p.Name.ToLower().Contains(target.ToLower()) || p.SteamID.ToString().Contains(target));

           if (target == null)
           {
               player.GameServer.SayToChat($"管理员 {player.Name} - 未找到要禁言的玩家");
               return;
           }
           else if (targetPlayer.Modifications.IsTextChatMuted)
           {
               player.GameServer.SayToChat($"管理员 {player.Name} - 玩家 {targetPlayer.Name} 已被禁言");
               return;
           }
           targetPlayer.Modifications.IsTextChatMuted = true;
           player.GameServer.SayToChat($"管理员 {player.Name} 禁言了 {targetPlayer?.Name}");
           return;
        }
    }
}