using BattleBitAPI.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CommunityServerAPI.Content
{
    internal static class MapManager
    {
        public static MapManagerJsonItem mapmanagerJson = new MapManagerJsonItem();
        public static void Init()
        {
            try
            {
                string mapmanagePath = $"{Environment.CurrentDirectory}\\Config\\MapManager.json";
                string content = File.ReadAllText(mapmanagePath);
                mapmanagerJson = JsonConvert.DeserializeObject<MapManagerJsonItem>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析 MapManager 配置出错，请检查 " + ee.StackTrace);
                return;
            }
        }
        // 通过传入的 gameMode 得到当前 gameMode 下所有可以使用的地图列表
        // 再通过这个地图列表返回当前 gameMode 下所有可以使用的 MapName 和List<byte> MapSize
        public static Dictionary<string, List<byte>> GetAvailableMapAndSize(string curMode)
        {
            Dictionary<string, List<byte>> mapList = new Dictionary<string, List<byte>>();
            return null;
        }
        
        // 通过传入的 curMode 得到当前可用的地图列表
        public static List<string> GetAvailableMapList(string curMode)
        {
            switch (curMode)
            {
                case "FFA":
                    return mapmanagerJson.FFA.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "TDM":
                    return mapmanagerJson.TDM.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "DOMI":
                    return mapmanagerJson.DOMI.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "RUSH":
                    return mapmanagerJson.RUSH.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "FRONTLINE":
                    return mapmanagerJson.FRONTLINE.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "GunGameFFA":
                    return mapmanagerJson.GunGameFFA.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "GunGameTeam":
                    return mapmanagerJson.GunGameTeam.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "ELIMI":
                    return mapmanagerJson.ELIMI.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "INFECTED":
                    return mapmanagerJson.INFECTED.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "CONQ":
                    return mapmanagerJson.CONQ.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "INFCONQ":
                    return mapmanagerJson.INFCONQ.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "TheCatch":
                    return mapmanagerJson.TheCatch.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "CaptureTheFlag":
                    return mapmanagerJson.CaptureTheFlag.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "AAS":
                    return mapmanagerJson.AAS.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "CashRun":
                    return mapmanagerJson.CashRun.Where(x => x.Available).Select(x => x.MapName).ToList();
                case "SuicideRush":
                    return mapmanagerJson.SuicideRush.Where(x => x.Available).Select(x => x.MapName).ToList();
                default:
                    return new List<string> { "Salhan" };
            }
        }
        

        public class FfaItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public bool Available { get; set; }
        }

        public class TdmItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class DomiItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class RushItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class FrontlineItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<int> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class GunGameFfaItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class GunGameTeamItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class ElimiItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class InfectedItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class ConqItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class InfconqItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class TheCatchItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class CaptureTheFlagItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class AasItem
        {

            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class CashRunItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class SuicideRushItem
        {

            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
            public bool Available { get; set; }
        }

        public class MapManagerJsonItem
        {
            public List<FfaItem> FFA { get; set; }
            public List<TdmItem> TDM { get; set; }
            public List<DomiItem> DOMI { get; set; }
            public List<RushItem> RUSH { get; set; }
            public List<FrontlineItem> FRONTLINE { get; set; }
            public List<GunGameFfaItem> GunGameFFA { get; set; }
            public List<GunGameTeamItem> GunGameTeam { get; set; }
            public List<ElimiItem> ELIMI { get; set; }
            public List<InfectedItem> INFECTED { get; set; }
            public List<ConqItem> CONQ { get; set; }
            public List<InfconqItem> INFCONQ { get; set; }
            public List<TheCatchItem> TheCatch { get; set; }
            public List<CaptureTheFlagItem> CaptureTheFlag { get; set; }
            public List<AasItem> AAS { get; set; }
            public List<CashRunItem> CashRun { get; set; }
            public List<SuicideRushItem> SuicideRush { get; set; }
        }
    }
}
