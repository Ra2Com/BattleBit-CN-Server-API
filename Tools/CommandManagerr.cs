using System.Text;
using CommunityServerAPI.Common;

namespace CommunityServerAPI.Tools;

public class CommandManager
{
    public async Task handleCommand(MyPlayer player, Command cmd)
    {
        switch (cmd.Action)
        {
            case CommandType.Help:
            {
                player.Message("可用聊天命令:", 2f);
                // DEVELOP TODO: 不知道咋根据玩家权限和命令权限来显示不同的命令 List
                var commands = MyGameServer.ChatCommands.Where(c => !c.needAdmin || player.IsAdmin).ToList();
                
                StringBuilder messageBuilder = new StringBuilder();
                foreach (var command in commands)
                {
                    messageBuilder.Append($"{RichText.Yellow}{command.commandMessage}{RichText.EndColor} - {command.helpMessage}\n");
                }
                string message = messageBuilder.ToString();
                
                player.Message(message, 5f);
                break;
            }
            case CommandType.Stats:
            {              
                player.Message($"{RichText.Cyan}{player.Name}{RichText.EndColor} 你好，游戏时长{MyPlayer.GetPhaseDifference(JoinTime)} , K/D: {player.K}/{player.D}，排名 {RichText.Orange}{player.rank}{RichText.EndColor}", 5f);
                break;
            }
            case CommandType.Kill:
            {
                var target = cmd.Message.Split(" ")[1..].Aggregate((a, b) => a + " " + b);
                var targetPlayer = player.GameServer.AllPlayers.ToList().FirstOrDefault(p => p.Name.ToLower().Contains(target.ToLower()) || p.SteamID.ToString().Contains(target));
                
                if (target == null)
                {
                    player.SayToChat($"管理员 {player.Name} - 未找到要杀死的玩家");
                    break;
                }
                
                targetPlayer?.Kill();
                player.SayToChat($"管理员 {player.Name} 使用命令杀死 {targetPlayer?.Name}");
                break;
            }
            case CommandType.Start:
            {
                if (player.GameServer.RoundSettings.State != GameState.WaitingForPlayers && player.GameServer.RoundSettings.State != GameState.CountingDown)
                {
                    player.SayToChat($"管理员 {player.Name} - 本局游戏已经开始了！");
                    break;
                }
                
                player.SayToChat($"管理员 {player.Name} - 使用命令开始了本局!");
                player.GameServer.ForceStartGame();
                player.GameServer.RoundSettings.SecondsLeft = 3;
                break;
            }
            case CommandType.End:
            {
                if (player.GameServer.RoundSettings.State != GameState.EndingGame)
                {
                    player.SayToChat($"管理员 {player.Name} - 本局游戏已结束！");
                    break;
                }
                
                player.SayToChat($"管理员 {player.Name} - 使用命令结束了本局!");
                player.GameServer.ForceEndGame();
                player.GameServer.RoundSettings.SecondsLeft = 3;
                break;
            }
            default:
            {
                player.SayToChat($"{player.Name} - 未知聊天命令，输入 /h 查询帮助");
                break;
            }
        }
    }
}