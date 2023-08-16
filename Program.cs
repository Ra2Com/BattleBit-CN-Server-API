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
        int ApiPort;
        var listener = new ServerListener<MyPlayer, MyGameServer>();

        ApiPort = 29294;

        listener.Start(ApiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/DD hh:mm:ss")} - 开始监听端口: {ApiPort}");

        Thread.Sleep(-1);
    }


}


