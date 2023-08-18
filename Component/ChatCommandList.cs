using System.Security;
using BattleBitAPI.Common;
using System.Numerics;
using BattleBitAPI;
using BattleBitAPI.Server;
using CommunityServerAPI.Common;

namespace CommunityServerAPI.Component;

public abstract class ChatCommandList
{
    public string commandMessage;
    public string helpMessage;
    public string[] Aliases;
    public bool needAdmin;
    public bool needModerator;
    public bool needVIP;

    public virtual Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return null;
    }
}

public class HealCommand : ChatCommandList
{
    public HealCommand()
    {
        // commandMessage = "/heal";
        // helpMessage = "治疗自己 100 生命值，每分钟只能使用一次";
        // Aliases = new string[] { "/h" };
        // needVIP = True;
    }

    // public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    // {
    //     return new Command
    //     {
    // TODO: VIP 的治疗命令，每 1分钟只能用一次
    //         Action = CommandType.Heal,
    //         Executor = player.Name,
    //         Error = false,
    //     };
    // }
}

public class SpeedCommand : ChatCommandList
{
    public SpeedCommand()
    {
        // TODO: VIP 的增加移动速度 10% 命令，每 1分钟只能用一次
    }
}

public class HelpCommand : ChatCommandList
{
    public HelpCommand()
    {
        commandMessage = "/help";
        helpMessage = "展示本帮助信息";
        Aliases = new string[] { "/h" };
        needAdmin = false;
    }

    public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return new Command
        {
            Action = CommandType.Help,
            Executor = player.Name,
            Error = false,
        };
    }
}

public class StatsCommand : ChatCommandList
{
    public StatsCommand()
    {
        commandMessage = "/stats";
        helpMessage = "展示你的对局数据";
        Aliases = new string[] { "/s" };
        needAdmin = false;
    }
    
    public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return new Command
        {
            Action = CommandType.Stats,
            Executor = player.Name,
            Error = false,
        };
    }
}

public class KillCommand : ChatCommandList
{
    public KillCommand()
    {
        commandMessage = "/kill";
        helpMessage = "通过玩家昵称或者 SteamID 杀死玩家";
        Aliases = new string[] { "/k" };
        if (player.isModerator)
            needModerator = true;
        else
            needAdmin = true;
    }

    public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return new Command
        {
            Action = CommandType.Kill,
            Executor = player.Name,
            Message = msg,
            Error = false,
        };
    }
}

public class StartCommand : ChatCommandList
{
    public StartCommand()
    {
        commandMessage = "/start";
        helpMessage = "立刻开始本轮对局";
        Aliases = new string[] { "/s" };
        if (player.isModerator)
            needModerator = true;
        else
            needAdmin = true;
    }
    
    public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return new Command
        {
            Action = CommandType.Start,
            Executor = player.Name,
            Error = false,
        };
    }
}

public class EndCommand : ChatCommandList
{
    public EndCommand()
    {
        commandMessage = "/end";
        helpMessage = "立刻结束本轮对局";
        Aliases = new string[] { "/ed" };
        if (player.isModerator)
            needModerator = true;
        else
            needAdmin = true;
    }
    
    public override Command ChatCommand(MyPlayer player, ChatChannel channel, string msg)
    {
        return new Command
        {
            Action = CommandType.End,
            Executor = player.Name,
            Error = false,
        };
    }
}

public class Command
{
    public CommandType Action { get; set; }
    public ulong SteamId { get; set; }
    public string Executor { get; set; }
    public Player<MyPlayer> Target { get; set; }
    public ulong TargetSteamId { get; set; }
    public string Message { get; set; }
    public Vector3 Location { get; set; }
    public int Amount { get; set; }
    public int Duration { get; set; }
    public string Reason { get; set; }
    public bool Error { get; set; }
}