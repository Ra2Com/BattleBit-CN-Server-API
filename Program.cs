using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.Component;
using CommunityServerAPI.Tools;
using System.Net;
using System.Numerics;
using System.Threading.Channels;
using System.Xml;

class Program
{
    static void Main(string[] args)
    {

        var listener = new ServerListener<MyPlayer, MyGameServer>();

        // TODO: 端口配置读取 Json 解析类结果
        int apiPort = 29294;
        SpawnManager.Init();
        SpawnManager.GetRandom();
        listener.OnGameServerConnecting += OnGameServerConnecting;
        listener.OnValidateGameServerToken += OnValidateGameServerToken;
        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }

    private static async Task<bool> OnValidateGameServerToken(IPAddress ip, ushort gameport, string sentToken)
    {
        await Console.Out.WriteLineAsync(ip + ":" + gameport + " sent " + sentToken);
        return true;
    }

    private static async Task<bool> OnGameServerConnecting(IPAddress arg)
    {
        await Console.Out.WriteLineAsync(arg.ToString() + " connecting");
        return true;
    }

}


