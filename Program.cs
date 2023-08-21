using BattleBitAPI.Server;
using BattleBitAPI.Common;
using CommunityServerAPI.Player;
using CommunityServerAPI.ServerExtension;
using CommunityServerAPI.ServerExtension.Model;
using System.Numerics;
using BattleBitAPI;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();

        int apiPort = 29294;
        LoadoutManager.Init();
        PrivilegeManager.Init();
        listener.LogLevel = LogLevel.Sockets | LogLevel.HealtChanges | LogLevel.GameServerErrors | LogLevel.KillsAndSpawns;
        listener.OnLog += OnLog;
        listener.OnCreatingGameServerInstance += OnCreatingGameServerInstance;
        listener.OnCreatingPlayerInstance += OnCreatingPlayerInstance;
        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }
    private static void OnLog(LogLevel level, string message, object? obj)
    {
        Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {level} - {message}");
    }

    // DEVELOP TODO: 新加了一个验证服务端 Token 的功能，防止端口和服务器功能被别人偷走
    // private static async Task<bool> OnValidateGameServerToken(IPAddress ip, ushort gameport, string sentToken)
    // {
    // await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ip}，{gameport} 验证Token: {sentToken}");
    //     return sentToken == "RamboArenaExamp1e@888";
    // }
    private static MyPlayer OnCreatingPlayerInstance(ulong steam64)
    {
        return new MyPlayer();
    }
    
    private static MyGameServer OnCreatingGameServerInstance(IPAddress ip, ushort port)
    {
        return new MyGameServer();
    }
}
