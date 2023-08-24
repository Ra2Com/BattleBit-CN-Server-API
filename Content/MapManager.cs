using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CommunityServerAPI.Content
{
    internal static class MapManager
    {
        public static MapManagerJson mapmanagerJson = new MapManagerJson();
        public static void Init()
        {
            try
            {
                string mapmanagePath = $"{Environment.CurrentDirectory}\\Config\\MapManager.json";
                string content = File.ReadAllText(mapmanagePath);
                mapmanagerJson = JsonConvert.DeserializeObject<MapManagerJson>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析 MapManager 配置出错，请检查 " + ee.StackTrace);
                return;
            }
        }
        // TODO: 通过传入的 gameMode 得到当前 gameMode 下所有可以使用的地图列表
        // 使用场景: 手动扩容服务器时指定服务器容量，则需要进行多重判断 
        // 再通过这个地图列表返回当前 gameMode 下所有可以使用的 MapName 和 List<byte> MapSize
        // public static Dictionary<string, List<byte>> GetAvailableMapAndSize(string curMode)
        // {
        //     Dictionary<string, List<byte>> mapList = new Dictionary<string, List<byte>>();
        //     return null;
        // }

        // 通过传入的 curMode 配置中当前可用的地图列表
        // WARNING: 在极端情况下如果某个模式所有地图不可用，传出来的默认值是 Salhan，但是如果这个地图在此模式也不可用，那么就会出现问题
        public static List<string> GetAvailableMapList(string curMode)
        {
            var modeItem = mapmanagerJson.ModeAvailableMapSize[0];
            return curMode switch
            {
                "FFA" => modeItem.FFA.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "TDM" => modeItem.TDM.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "DOMI" => modeItem.DOMI.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "RUSH" => modeItem.RUSH.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "FRONTLINE" => modeItem.FRONTLINE.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "GunGameFFA" => modeItem.GunGameFFA.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "GunGameTeam" => modeItem.GunGameTeam.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "ELI" => modeItem.ELI.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "INFECTED" => modeItem.INFECTED.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CONQ" => modeItem.CONQ.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "INFCONQ" => modeItem.INFCONQ.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "TheCatch" => modeItem.TheCatch.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CaptureTheFlag" => modeItem.CaptureTheFlag.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "AAS" => modeItem.AAS.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "CashRun" => modeItem.CashRun.Where(x => x.Available).Select(x => x.MapName).ToList(),
                "SuicideRush" => modeItem.SuicideRush.Where(x => x.Available).Select(x => x.MapName).ToList(),
                _ => new List<string>
                {
                    "Salhan"
                }
            };
        }

        // 传入 isAvailable 获取所有可用的地图模式池
        public static List<string> GetAvailableModeList(bool isAvailable)
        {
            List<string> modeList = new List<string>();
            foreach (var item in mapmanagerJson.ModeAvailableMapSize)
            {
                if (item.FFA.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("FFA");
                }
                if (item.TDM.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("TDM");
                }
                if (item.DOMI.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("DOMI");
                }
                if (item.RUSH.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("RUSH");
                }
                if (item.FRONTLINE.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("FRONTLINE");
                }
                if (item.GunGameFFA.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("GunGameFFA");
                }
                if (item.GunGameTeam.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("GunGameTeam");
                }
                if (item.ELI.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("ELI");
                }
                if (item.INFECTED.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("INFECTED");
                }
                if (item.CONQ.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("CONQ");
                }
                if (item.INFCONQ.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("INFCONQ");
                }
                if (item.TheCatch.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("TheCatch");
                }
                if (item.CaptureTheFlag.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("CaptureTheFlag");
                }
                if (item.AAS.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("AAS");
                }
                if (item.CashRun.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("CashRun");
                }
                if (item.SuicideRush.Any(x => x.Available == isAvailable))
                {
                    modeList.Add("SuicideRush");
                }

            }
            return modeList;
        }

        // 通过传入 curMode 获取一张随机的可用地图，排除掉 curMap
        public static string[] GetARandomAvailableMap(string curMode, string curMap)
        {
            List<string> mapList = GetAvailableMapList(curMode);
            // 排除掉 curMap 之后再随机返回一个地图
            mapList.Remove(curMap);
            int rng = RandomNumberGenerator.GetInt32(0, mapList.Count);
            return new[]
            {
                mapList[rng]
            };
        }

        public class FfaItem
        {
            public string MapName { get; set; }
            public int WorldSize { get; set; }
            public List<byte> MapSize { get; set; }
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
            public List<byte> MapSize { get; set; }
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

        public class EliItem
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

        public class MapManagerJson
        {
            public string _Usage { get; set; }
            public List<ModeAvailableMapSize> ModeAvailableMapSize { get; set; }
        }
        public class ModeAvailableMapSize
        {
            public List<FfaItem> FFA { get; set; }
            public List<TdmItem> TDM { get; set; }
            public List<DomiItem> DOMI { get; set; }
            public List<RushItem> RUSH { get; set; }
            public List<FrontlineItem> FRONTLINE { get; set; }
            public List<GunGameFfaItem> GunGameFFA { get; set; }
            public List<GunGameTeamItem> GunGameTeam { get; set; }
            public List<EliItem> ELI { get; set; }
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
