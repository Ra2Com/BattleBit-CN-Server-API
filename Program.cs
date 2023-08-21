using BattleBitAPI.Server;
using BattleBitAPI.Common;
using CommunityServerAPI.Tools;
using CommunityServerAPI.ServerExtension;
using CommunityServerAPI.ServerExtension.Model;
using System.Numerics;
using BattleBitAPI;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();

        int apiPort = 29294;
        SpawnManager.Init();
        PrivilegeManager.Init();
        listener.LogLevel = LogLevel.Sockets | LogLevel.HealtChanges | LogLevel.GameServerErrors | LogLevel.KillsAndSpawns;
        listener.OnLog += OnLog;
        // DEVELOPER TODO: 这玩意不注释会报错
        // listener.OnCreatingGameServerInstance += OnCreatingGameServerInstance;
        // listener.OnCreatingPlayerInstance += OnCreatingPlayerInstance;
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
    // private static MyPlayer OnCreatingPlayerInstance()
    // {
    //     return new MyPlayer();
    // }
    //
    // private static MyGameServer OnCreatingGameServerInstance()
    // {
    //     return new MyGameServer();
    // }
}
