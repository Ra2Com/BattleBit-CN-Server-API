using BattleBitAPI.Common;
using CommunityServerAPI.Tools;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.ServerExtension.Handler;

public class RefreshCommandHandler : CommandHandlerBase
{
    public RefreshCommandHandler()
    {
        commandMessage = "/refresh";
        helpMessage = "刷新所有载入的配置文件 (如: 道具配置)";
        Aliases = new[] { "/config" };
        roles = new List<Roles> { Roles.Admin, Roles.Moderator };
        isPrivate = true;
    }

    public override CommandDTO BuildCommand(MyPlayer player, ChatChannel channel)
    {
        return new CommandDTO
        {
            CommandType = CommandTypes.Refresh,
            Executor = player.Name,
            Error = false,
        };
    }

    public override void Execute(MyPlayer player, string cmdMsg)
    {
        SpawnManager.Init();
        PrivilegeManager.Init();
        player.GameServer.SayToChat($"管理员 {player.Name} - 配置文件已刷新");
    }
}