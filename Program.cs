using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using System.Linq.Expressions;
using CommunityServerAPI.Tools;
using System.Net;
using System.Numerics;
using System.Threading.Channels;
using System.Xml;
using CommunityServerAPI.ServerExtension;
using CommunityServerAPI.ServerExtension.Model;

class Program
{
    static void Main(string[] args)
    {
        var listener = new ServerListener<MyPlayer, MyGameServer>();

        // TODO: 端口配置读取 Json 解析类结果或配置
        int apiPort = 29294;
        SpawnManager.Init();
        PrivilegeManager.Init();
        //listener.OnCreatingGameServerInstance += OnCreatingGameServerInstance;
        //listener.OnCreatingPlayerInstance += OnCreatingPlayerInstance;
        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }

    // DEVELOP TODO: 新加了一个验证服务端 Token 的功能，防止端口和服务器功能被别人偷走
    // private static async Task<bool> OnValidateGameServerToken(IPAddress ip, ushort gameport, string sentToken)
    // {
    // await Console.Out.WriteLineAsync($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - {ip}，{gameport} 验证Token: {sentToken}");
    //     return sentToken == "RamboArenaExamp1e@888";
    // }

    private static MyPlayer OnCreatingPlayerInstance()
    {
        return new MyPlayer();
    }

    private static MyGameServer OnCreatingGameServerInstance()
    {
        return new MyGameServer();
    }
}