using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using CommunityServerAPI.Component;
using System.Threading.Channels;
using System.Xml;

class Program
{
    static void Main(string[] args)
    {
      
        var apiPort;
        var listener = new ServerListener<MyPlayer, MyGameServer>();

        // TODO: 端口配置读取 Json 解析类结果
        apiPort = 29294;

        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }


}


