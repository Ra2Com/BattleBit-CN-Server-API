using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;
using System.Linq.Expressions;
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
        listener.OnCreatingGameServerInstance += OnCreatingGameServerInstance;
        listener.OnCreatingPlayerInstance += OnCreatingPlayerInstance;
        listener.Start(apiPort);

        if (listener.IsListening)
            Console.WriteLine($"{DateTime.Now.ToString("MM/dd HH:mm:ss")} - 开始监听端口: {apiPort}");

        Thread.Sleep(-1);
    }

    // 新加了一个验证服务端 Token 的功能，防止端口被别人偷走
    private static MyPlayer OnCreatingPlayerInstance()
    {
        return new MyPlayer();
    }

    private static MyGameServer OnCreatingGameServerInstance()
    {
        return new MyGameServer();
    }
}
//class MyPlayer : Player<MyPlayer>
//{
//    private string mydb;
//    public MyPlayer(string mydb)
//    {
//        this.mydb = mydb;
//    }

//    public override async Task OnSpawned()
//    {
//    }
//}
//class MyGameServer : GameServer<MyPlayer>
//{
//    private string myDbConnection;
//    public MyGameServer(string mySecretDBConnection)
//    {
//        this.myDbConnection = mySecretDBConnection;
//    }

//    public override async Task OnConnected()
//    {
//        ForceStartGame();
//        ServerSettings.PlayerCollision = true;
//    }
//    public override async Task OnTick()
//    {
//    }
//    public override async Task<OnPlayerSpawnArguments> OnPlayerSpawning(MyPlayer player, OnPlayerSpawnArguments request)
//    {
//        request.Wearings.Eye = "Eye_Zombie_01";
//        request.Wearings.Face = "Face_Zombie_01";
//        request.Wearings.Face = "Hair_Zombie_01";
//        request.Wearings.Skin = "Zombie_01";
//        request.Wearings.Uniform = "ANY_NU_Uniform_Zombie_01";
//        request.Wearings.Head = "ANV2_Universal_Zombie_Helmet_00_A_Z";
//        request.Wearings.Belt = "ANV2_Universal_All_Belt_Null";
//        request.Wearings.Backbag = "ANV2_Universal_All_Backpack_Null";
//        request.Wearings.Chest = "ANV2_Universal_All_Armor_Null";

//        return request;
//    }

//    public override async Task OnPlayerConnected(MyPlayer player)
//    {
//        await Console.Out.WriteLineAsync("Connected: " + player);
//        player.Modifications.CanSpectate = true;
//        player.Modifications.CanDeploy = false;

//    }
//    public override async Task OnPlayerSpawned(MyPlayer player)
//    {
//        await Console.Out.WriteLineAsync("Spawned: " + player);
//    }
//    public override async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<MyPlayer> args)
//    {
//        await Console.Out.WriteLineAsync("Downed: " + args.Victim);
//    }
//    public override async Task OnPlayerGivenUp(MyPlayer player)
//    {
//        await Console.Out.WriteLineAsync("Giveup: " + player);
//    }
//    public override async Task OnPlayerDied(MyPlayer player)
//    {
//        await Console.Out.WriteLineAsync("Died: " + player);
//    }
//    public override async Task OnAPlayerRevivedAnotherPlayer(MyPlayer from, MyPlayer to)
//    {
//        await Console.Out.WriteLineAsync(from + " revived " + to);
//    }
//    public override async Task OnPlayerDisconnected(MyPlayer player)
//    {
//        await Console.Out.WriteLineAsync("Disconnected: " + player);
//    }
//}
