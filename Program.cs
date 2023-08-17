using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.Component;
using CommunityServerAPI.Tools;
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
        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }


}


