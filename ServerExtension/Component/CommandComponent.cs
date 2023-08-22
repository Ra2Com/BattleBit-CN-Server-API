using BattleBitAPI.Common;
using System.Text;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Handler;
using CommunityServerAPI.ServerExtension.Handler.Commands;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Utils;

namespace CommunityServerAPI.ServerExtension.Component
{
    public class CommandComponent
    {
        private List<CommandHandlerBase> commandHandlers = new List<CommandHandlerBase>();
        private static CommandComponent _instance;

        public static CommandComponent Initialize()
        {
            if (null == _instance)
            {
                _instance = new CommandComponent();
                return _instance;
            }

            return _instance;
        }

        private CommandComponent()
        {
            // VIP 等高级角色命令
            commandHandlers.Add(new Heal());
            commandHandlers.Add(new SpeedMod());

            // 普通玩家命令
            commandHandlers.Add(new Stats());
            commandHandlers.Add(new Help());

            // 仅管理员可用的命令 
            commandHandlers.Add(new Kill());
            commandHandlers.Add(new Mute());
            commandHandlers.Add(new Kick());
            commandHandlers.Add(new Ban());
            commandHandlers.Add(new EndGame());
            commandHandlers.Add(new StartGame());
            commandHandlers.Add(new Announce());
            commandHandlers.Add(new AnnounceLong());
            commandHandlers.Add(new RefreshConfig());
        }


        public async Task HandleCommand(MyPlayer player, ChatChannel channel, string msg)
        {
            var splits = msg.Split(" ");
            var cmd = splits[0].ToLower();
            var playerRole = player.stats.Roles;
            var commandHandler = commandHandlers.Find(a => a.commandMessage.Contains(cmd));
            if (null == commandHandler)
            {
                player.GameServer.SayToChat($"{player.Name} - 未知聊天命令，输入 /h 查看帮助", player.SteamID);
                return;
            }

            // 检查执行的 Roles 是什么
            if (commandHandler.roles is not null && !commandHandler.roles.Contains(playerRole))
            {
                return;
            }

            var getCommand = commandHandler.BuildCommand(player, channel);
            if (null == getCommand)
            {
                return;
            }

            if (!string.IsNullOrEmpty(getCommand.Message))
            {
                player.Message(getCommand.Message, 5f);
            }

            // 针对 Help 指令的处理
            switch (getCommand.CommandType)
            {
                case CommandTypes.Help:
                    {
                        player.Message("可用聊天命令:", 2f);
                        var showCommands = new List<CommandHandlerBase>();
                        showCommands = commandHandlers
                            .Where(a => a.roles is null || a.roles.Count == 0 || !a.roles.Contains(playerRole)).ToList();

                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var command in showCommands)
                        {
                            messageBuilder.Append(
                                $"{RichText.Yellow}{command.commandMessage}{RichText.EndColor} - {command.helpMessage}{RichText.LineBreak}");
                        }

                        string message = messageBuilder.ToString();

                        player.Message(message, 5f);
                        break;
                    }
            }

            // 执行这条命令并打印日志
            await Console.Out.WriteLineAsync(
                ($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - [{player.Role}] - {player.Name} 执行命令 -" + msg));
            commandHandler.Execute(player, msg);
        }
    }
}