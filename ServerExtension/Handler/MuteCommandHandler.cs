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
            helpMessage = "禁言某个玩家";
            Aliases = new string[] { "/m" };
            roles = new List<Roles>() { Roles.Admin, Roles.Moderator };
        }
        public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
        {
            return new CommandDTO
            {
                CommandType = CommandTypeEnum.Mute,
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
           else
           {
               targetPlayer.Modifications.IsTextChatMuted = true;
           }
           player.GameServer.SayToChat($"管理员 {player.Name} 使用命令禁言 {targetPlayer?.Name}");
           return;
        }
    }
}