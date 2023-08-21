using CommunityServerAPI.BattleBitAPI.Common.Data;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CommunityServerAPI.Player
{
    internal static class LoadoutManager
    {
        public static LoadoutJson loadoutJson = new LoadoutJson();
        public static SkinIndexJson weaponnameJson = new SkinIndexJson();

        public static void Init()
        {
            try
            {
                string
                    loadoutsPath =
                        $"{Environment.CurrentDirectory}\\Config\\RandomLoadouts.json"; // PRODUCTION WARNING: You must customize your own RandomLoadouts, Open-soured one is an test example.
                string content = File.ReadAllText(loadoutsPath);
                loadoutJson = JsonConvert.DeserializeObject<LoadoutJson>(content);
                string
                    skinPath =
                        $"{Environment.CurrentDirectory}\\Config\\WeaponSkinIndex.json";
                string skincontent = File.ReadAllText(skinPath);
                // 序列化 WeaponSkinIndex 第一层是武器名，第二层是武器配置，并且根据武器名序列化武器的配置
                weaponnameJson = JsonConvert.DeserializeObject<SkinIndexJson>(skincontent);
                //List<weaponskinJson> = JsonConvert.DeserializeObject<SkinIndexJson>(weaponnameJson);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析武器和皮肤配置出错，请检查 " + ee.Message); // Error when JSON file is wrong
                return;
            }
        }
        public static byte GetSkinIndex(string weaponName)
        {
            if (string.IsNullOrEmpty(weaponName))
            {
                return 0;
            }
            
            return 0;
        }

        public static PlayerLoadout GetRandom()
        {
            PlayerLoadout playerLoadout = new PlayerLoadout();

            // RNG https://learn.microsoft.com/zh-cn/dotnet/api/system.security.cryptography.randomnumbergenerator?view=net-6.0
            // 主武器配置
            int PrimaryWeaponIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListPrimaryWeapon.Count);
            var pWI = new WeaponItem();
            pWI.ToolName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Name ?? "none";
            pWI.MainSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].MainSight ?? "none";
            pWI.TopSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].TopSight ?? "none";
            pWI.CantedSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].CantedSight ?? "none";
            pWI.BarrelName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Barrel ?? "none";
            pWI.SideRailName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].SideRail ?? "none";
            pWI.UnderRailName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].UnderRail ?? "none";
            pWI.BoltActionName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Bolt ?? "none";
            //pWI.SkinIndex = (byte.Parse(loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].SkinIndex ?? "1"));
            pWI.MagazineIndex = (byte.Parse(loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].MagazineIndex ?? "0"));
            playerLoadout.PrimaryWeapon = pWI;
            // 主武器附加弹夹
            playerLoadout.PrimaryExtraMagazines =
                (byte.Parse(loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].PrimaryExtraMagazines ?? "2"));

            // 手枪配置
            int SecondaryWeaponIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListSecondaryWeapon.Count);
            var sWI = new WeaponItem();
            sWI.ToolName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Name;
            sWI.BarrelName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Barrel ?? "none";
            sWI.MainSightName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MainSight ?? "none";
            sWI.SideRailName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SideRail ?? "none";
            //sWI.SkinIndex = (byte.Parse(loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SkinIndex ?? "0"));
            sWI.MagazineIndex =
                (byte.Parse(loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MagazineIndex ?? "0"));
            playerLoadout.SecondaryWeapon = sWI;
            // 副武器附加弹夹
            playerLoadout.SecondaryExtraMagazines =
                (byte.Parse(loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SecondaryExtraMagazines ?? "3"));

            // 绷带配置
            int ListFirstAidIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListFirstAid.Count);
            playerLoadout.FirstAidName = loadoutJson.ListFirstAid[ListFirstAidIndex].Name ?? "none";
            playerLoadout.FirstAidExtra =
                (byte.Parse(loadoutJson.ListFirstAid[ListFirstAidIndex].FirstAidExtra ?? "0"));

            // 轻型道具配置
            int ListLightGadgetIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListLightGadget.Count);
            playerLoadout.LightGadgetName = loadoutJson.ListLightGadget[ListLightGadgetIndex].Name ?? "none";
            playerLoadout.LightGadgetExtra =
                (byte.Parse(loadoutJson.ListLightGadget[ListLightGadgetIndex].LightGadgetExtra ?? "0"));

            // 重型道具配置
            int ListHeavyGadgetIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListHeavyGadget.Count);
            playerLoadout.HeavyGadgetName = loadoutJson.ListHeavyGadget[ListHeavyGadgetIndex].Name ?? "none";
            playerLoadout.HeavyGadgetExtra =
                (byte.Parse(loadoutJson.ListHeavyGadget[ListHeavyGadgetIndex].HeavyGadgetExtra ?? "0"));

            // 投掷物配置
            int ListThrowableIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListThrowable.Count);
            playerLoadout.ThrowableName = loadoutJson.ListThrowable[ListThrowableIndex].Name ?? "none";
            playerLoadout.ThrowableExtra =
                (byte.Parse(loadoutJson.ListThrowable[ListThrowableIndex].ThrowableExtra ?? "1"));

            return playerLoadout;
        }
    }

    public class LoadoutJson
    {
        public List<PrimaryWeaponJson> ListPrimaryWeapon { get; set; } = new List<PrimaryWeaponJson>();

        public List<SecondaryWeaponJson> ListSecondaryWeapon { get; set; } = new List<SecondaryWeaponJson>();

        public List<FirstAidJson> ListFirstAid { get; set; } = new List<FirstAidJson>();
        public List<LightGadgetJson> ListLightGadget { get; set; } = new List<LightGadgetJson>();
        public List<HeavyGadgetJson> ListHeavyGadget { get; set; } = new List<HeavyGadgetJson>();
        public List<ThrowableJson> ListThrowable { get; set; } = new List<ThrowableJson>();
    }

    public class PrimaryWeaponJson
    {
        public string Name { get; set; }
        public string Barrel { get; set; }
        public string MainSight { get; set; }
        public string TopSight { get; set; }
        public string CantedSight { get; set; }
        public string UnderRail { get; set; }
        public string SideRail { get; set; }
        public string Bolt { get; set; }
        public string SkinIndex { get; set; }
        public string MagazineIndex { get; set; }
        public string PrimaryExtraMagazines { get; set; }
    }

    public class SecondaryWeaponJson
    {
        public string Name { get; set; }
        public string Barrel { get; set; }
        public string MainSight { get; set; }
        public string SideRail { get; set; }
        public string SkinIndex { get; set; }
        public string MagazineIndex { get; set; }
        public string SecondaryExtraMagazines { get; set; }
    }

    public class FirstAidJson
    {
        public string Name { get; set; }
        public string FirstAidExtra { get; set; }
    }

    public class LightGadgetJson
    {
        public string Name { get; set; }
        public string LightGadgetExtra { get; set; }
    }

    public class HeavyGadgetJson
    {
        public string Name { get; set; }
        public string HeavyGadgetExtra { get; set; }
    }

    public class ThrowableJson
    {
        public string Name { get; set; }
        public string ThrowableExtra { get; set; }
    }
    
    public class SkinIndexJson
    {
        public List<ListWeaponName> ListWeaponNames { get; set; } = new List<ListWeaponName>();
        public List<ListWeaponSkin> ListWeaponSkin { get; set; } = new List<ListWeaponSkin>();
    }
    
    public class ListWeaponName
    {
        public byte SkinIndex { get; set; }
        public string DisplayName { get; set; }
        public bool Available { get; set; } = true;
    }
    
    public class ListWeaponSkin
    {
        public string DisplayName { get; set; }
    }
}