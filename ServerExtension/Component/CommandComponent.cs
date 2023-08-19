using System.Text;
using BattleBitAPI.Common;
using CommunityServerAPI.ServerExtension.Enums;
using CommunityServerAPI.ServerExtension.Handler;
using CommunityServerAPI.ServerExtension.Model;
using CommunityServerAPI.Tools;

namespace CommunityServerAPI.ServerExtension.Component;

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

        commandHandlers.Add(new EndCommandHandler());
        commandHandlers.Add(new HealCommandHandler());
        commandHandlers.Add(new HelpCommandHandler());
        commandHandlers.Add(new KillCommandHandler());
        commandHandlers.Add(new SpeedCommandHandler());
        commandHandlers.Add(new StartCommandHandler());
        commandHandlers.Add(new StatsCommandHandler());
    }
   

    public async Task HandleCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        var splits = msg.Split(" ");
        var cmd = splits[0].ToLower();
        var playerRole = player.stats.Roles;
        if (playerRole != Roles.Admin || playerRole != Roles.Moderator || !msg.StartsWith("/"))
            return ;
        var commandHandler = commandHandlers.Find(a => a.commandMessage.Contains(cmd) || a.Aliases.Contains(cmd));
        if (null == commandHandler)
        {
            player.GameServer.SayToChat($"{player.Name} - 未知聊天命令，输入 /h 查询帮助");
            return ;
        }
        // 检查执行的 Roles 是什么
        if (commandHandler.roles is not null && !commandHandler.roles.Contains(Roles.None) && !commandHandler.roles.Contains(playerRole))
        {

            return ;
        }
       
        var getCommand = commandHandler.BuildCommand(player, channel);
        if (null == getCommand) 
        {

            return ;
        }
        if (!string.IsNullOrEmpty(getCommand.Message))
        {
            player.Message(getCommand.Message, 5f);
        }
        
        // 额外的处理
        switch (getCommand.CommandType)
        {
            case CommandTypeEnum.Help:
                {
                    player.Message("可用聊天命令:", 2f);
                    var showCommands = new List<CommandHandlerBase>();
                    if (playerRole == Roles.Admin)
                    {
                        showCommands = commandHandlers;
                    }
                    else
                    {
                        showCommands = commandHandlers.Where(a => a.roles is null || a.roles.Count == 0 || !a.roles.Contains(Roles.Admin)).ToList();
                    }                   
                    StringBuilder messageBuilder = new StringBuilder();
                    foreach (var command in showCommands)
                    {
                        messageBuilder.Append($"{RichText.Yellow}{command.commandMessage}{RichText.EndColor} - {command.helpMessage}\n");
                    }
                    string message = messageBuilder.ToString();

                    player.Message(message, 5f);
                    break;
                }
                       
        }
        // 执行这条命令
        commandHandler.Execute(player, msg);
    }
}
