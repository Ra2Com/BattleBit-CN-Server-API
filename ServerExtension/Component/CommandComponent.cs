using BattleBitAPI.Common;
using System.Text;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Handler;
using CommunityServerAPI.ServerExtension.Handler.Commands;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Utils;
using Newtonsoft.Json;

namespace CommunityServerAPI.ServerExtension.Component
{
    public class CommandComponent
    {
        private List<CommandHandlerBase> mCommandHandlers = new List<CommandHandlerBase>();
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
            mCommandHandlers.Add(new Heal());
            mCommandHandlers.Add(new SpeedMod());

            // 普通玩家命令
            mCommandHandlers.Add(new Stats());
            mCommandHandlers.Add(new Help());

            // 管理员都可用的命令 
            mCommandHandlers.Add(new Kill());
            mCommandHandlers.Add(new Mute());
            mCommandHandlers.Add(new Kick());
            mCommandHandlers.Add(new Ban());
            mCommandHandlers.Add(new EndGame());
            mCommandHandlers.Add(new StartGame());
            mCommandHandlers.Add(new Announce());
            mCommandHandlers.Add(new AnnounceLong());
            mCommandHandlers.Add(new RefreshConfig());
            mCommandHandlers.Add(new AddBot());
            mCommandHandlers.Add(new RemoveBot());
            mCommandHandlers.Add(new BotFire());

            // 超级管理员可用的命令
            mCommandHandlers.Add(new ServerCommands());
        }


        public async Task HandleCommand(MyPlayer player, ChatChannel channel, string msg)
        {
            Console.WriteLine($"{player.Name}发送命令:{msg}");
            var splits = msg.Split(" ");
            var cmd = splits[0].ToLower();
            var playerRole = player.stats.Roles;
            var commandHandler = mCommandHandlers.Find(a => a.commandMessage==cmd || a.Aliases.Contains(cmd));
            if (null == commandHandler && msg.StartsWith("/"))
            {
                player.GameServer.MessageToPlayer(player.SteamID, $"{player.Name} - 未知聊天命令，输入 /help 查看帮助");
                return;
            }
            Console.WriteLine($"{JsonConvert.SerializeObject(commandHandler)}");
            Console.WriteLine($"{JsonConvert.SerializeObject(playerRole)}");

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
                        showCommands = mCommandHandlers
                            .Where(a => a.roles is null || a.roles.Count == 0 || !a.roles.Contains(playerRole)).ToList();
                        // TODO 排除掉 mCommandHandlers 中 isPrivate 为 true 的命令

                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var command in showCommands)
                        {
                            messageBuilder.Append(
                                $"{RichText.Yellow}{command.commandMessage}{RichText.EndColor} - {command.helpMessage}{RichText.LineBreak}");
                        }

                        string message = messageBuilder.ToString();

                        player.Message(message, 8f);
                        break;
                        return;
                    }
            }

            // 执行这条命令并打印日志
            await Console.Out.WriteLineAsync(
                ($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - [{player.Role}] - {player.Name} 执行命令 -" + msg));
            commandHandler.Execute(player, msg);
        }
    }
}