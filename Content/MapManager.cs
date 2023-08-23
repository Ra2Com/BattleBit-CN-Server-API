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
        // TODO: 通过传入的 gameMode 得到当前 gameMode 下所有可以使用的地图列表
        // 使用场景: 手动扩容服务器时指定服务器容量，则需要进行多重判断 
        // 再通过这个地图列表返回当前 gameMode 下所有可以使用的 MapName 和List<byte> MapSize
        public static Dictionary<string, List<byte>> GetAvailableMapAndSize(string curMode)
        {
            Dictionary<string, List<byte>> mapList = new Dictionary<string, List<byte>>();
            return null;
        }
        
        // 通过传入的 curMode 配置中当前可用的地图列表
        // WARNING: 在极端情况下如果某个模式所有地图不可用，传出来的默认值是 Salhan，但是如果这个地图在此模式也不可用，那么就会出现问题
        public static List<string> GetAvailableMapList(string curMode)
        {
            return curMode switch
            {
                "FFA" => mapmanagerJson.FFA.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "TDM" => mapmanagerJson.TDM.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "DOMI" => mapmanagerJson.DOMI.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "RUSH" => mapmanagerJson.RUSH.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "FRONTLINE" => mapmanagerJson.FRONTLINE.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "GunGameFFA" => mapmanagerJson.GunGameFFA.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "GunGameTeam" => mapmanagerJson.GunGameTeam.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "ELIMI" => mapmanagerJson.ELIMI.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "INFECTED" => mapmanagerJson.INFECTED.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CONQ" => mapmanagerJson.CONQ.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "INFCONQ" => mapmanagerJson.INFCONQ.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "TheCatch" => mapmanagerJson.TheCatch.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CaptureTheFlag" => mapmanagerJson.CaptureTheFlag.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "AAS" => mapmanagerJson.AAS.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CashRun" => mapmanagerJson.CashRun.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "SuicideRush" => mapmanagerJson.SuicideRush.Where(x => x.Available).Select(x => x.MapName).ToList(),
                _ => new List<string>
                {
                    "Salhan"
                }
            };
        }
        
        // 通过传入 curMode 获取一张随机的可用地图
        public static List<string> GetARandomAvailableMap(string curMode)
        {
            List<string> mapList = GetAvailableMapList(curMode);
            int index = RandomNumberGenerator.GetInt32(0,mapList.Count);
            return new List<string>()
            {
                mapList[index]
            };
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
