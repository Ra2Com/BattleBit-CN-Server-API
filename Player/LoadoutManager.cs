using BattleBitAPI.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CommunityServerAPI.Player
{
    internal static class LoadoutManager
    {
        public static LoadoutJson loadoutJson = new LoadoutJson();
        public static WeaponSkinJson weaponnameJson = new WeaponSkinJson();
        
        public static void Init()
        {
            LoadoutInit();
            WeaponSkinInit();
        }

        public static void LoadoutInit()
        {
            try
            {
                string
                    loadoutsPath =
                        $"{Environment.CurrentDirectory}\\Config\\RandomLoadouts.json"; // PRODUCTION WARNING: You must customize your own RandomLoadouts, Open-soured one is an test example.
                string content = File.ReadAllText(loadoutsPath);
                loadoutJson = JsonConvert.DeserializeObject<LoadoutJson>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析武器 RandomLoadouts 配置出错，请检查 " + ee.Message); // Error when JSON file is wrong
                return;
            }
        }
        
        public static void WeaponSkinInit()
        {
            try
            {
                string
                    skinPath =
                        $"{Environment.CurrentDirectory}\\Config\\WeaponSkinIndex.json";
                string skincontent = File.ReadAllText(skinPath);
                weaponnameJson = JsonConvert.DeserializeObject<WeaponSkinJson>(skincontent);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析武器皮肤 WeaponSkinIndex 配置出错，请检查 " + ee.Message); // Error when JSON file is wrong
                return;
            }
        }
        // 获取传入武器的最大皮肤索引
        public static byte GetMaxSkinIndex(string weaponName)
        {
            var selectedName = weaponnameJson.WeaponSkinIndex.Find(x => x.Weapon == weaponName);
            if (selectedName != null)
            {
                var maxIndex = selectedName.Skins.Max(x => x.SkinIndex);
                return maxIndex;
            }
            return 0;
        }
        // 获取传入武器的随机皮肤索引
        public static byte RandomSkinIndex(string weaponName)
        {
            var selectedName = weaponnameJson.WeaponSkinIndex.Find(x => x.Weapon == weaponName);
            if (selectedName != null)
            {
                var maxIndex = selectedName.Skins.Max(x => x.SkinIndex);
                var randomskin = RandomNumberGenerator.GetInt32(0, maxIndex);
                return (byte)randomskin;
            }
            return 0;
        }
        
        // 获取传入武器和皮肤名称的皮肤索引
        public static byte SeletedSkinIndex(string weaponName, string skinName)
        {
            var selectedName = weaponnameJson.WeaponSkinIndex.Find(x => x.Weapon == weaponName);
            if (selectedName != null)
            {
                var selectedSkin = selectedName.Skins.Find(x => x.DisplayName == skinName);
                if (selectedSkin != null)
                {
                    return selectedSkin.SkinIndex;
                }
            }
            return 0;
        }

        public static PlayerLoadout GetRandom()
        {
            PlayerLoadout playerLoadout = new PlayerLoadout();

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
            pWI.SkinIndex = RandomSkinIndex(pWI.ToolName); //随机皮肤
            pWI.MagazineIndex = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].MagazineIndex;
            playerLoadout.PrimaryWeapon = pWI;
            // 主武器附加弹夹
            playerLoadout.PrimaryExtraMagazines = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].PrimaryExtraMagazines;

            // 手枪配置
            int SecondaryWeaponIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListSecondaryWeapon.Count);
            var sWI = new WeaponItem();
            sWI.ToolName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Name;
            sWI.BarrelName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Barrel ?? "none";
            sWI.MainSightName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MainSight ?? "none";
            sWI.SideRailName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SideRail ?? "none";
            sWI.SkinIndex = RandomSkinIndex(sWI.ToolName);//随机皮肤
            sWI.MagazineIndex = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MagazineIndex;
            playerLoadout.SecondaryWeapon = sWI;
            // 副武器附加弹夹
            playerLoadout.SecondaryExtraMagazines = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SecondaryExtraMagazines;

            // 绷带配置
            int ListFirstAidIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListFirstAid.Count);
            playerLoadout.FirstAidName = loadoutJson.ListFirstAid[ListFirstAidIndex].Name ?? "none";
            playerLoadout.FirstAidExtra = loadoutJson.ListFirstAid[ListFirstAidIndex].FirstAidExtra;

            // 轻型道具配置
            int ListLightGadgetIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListLightGadget.Count);
            playerLoadout.LightGadgetName = loadoutJson.ListLightGadget[ListLightGadgetIndex].Name ?? "none";
            playerLoadout.LightGadgetExtra = loadoutJson.ListLightGadget[ListLightGadgetIndex].LightGadgetExtra;

            // 重型道具配置
            int ListHeavyGadgetIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListHeavyGadget.Count);
            playerLoadout.HeavyGadgetName = loadoutJson.ListHeavyGadget[ListHeavyGadgetIndex].Name ?? "none";
            playerLoadout.HeavyGadgetExtra = loadoutJson.ListHeavyGadget[ListHeavyGadgetIndex].HeavyGadgetExtra;

            // 投掷物配置
            int ListThrowableIndex = RandomNumberGenerator.GetInt32(0, loadoutJson.ListThrowable.Count);
            playerLoadout.ThrowableName = loadoutJson.ListThrowable[ListThrowableIndex].Name ?? "none";
            playerLoadout.ThrowableExtra = loadoutJson.ListThrowable[ListThrowableIndex].ThrowableExtra;

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
        public byte SkinIndex { get; set; }
        public byte MagazineIndex { get; set; }
        public byte PrimaryExtraMagazines { get; set; } = 4;
    }

    public class SecondaryWeaponJson
    {
        public string Name { get; set; }
        public string Barrel { get; set; }
        public string MainSight { get; set; }
        public string SideRail { get; set; }
        public byte SkinIndex { get; set; }
        public byte MagazineIndex { get; set; }
        public byte SecondaryExtraMagazines { get; set; } = 2;
    }

    public class FirstAidJson
    {
        public string Name { get; set; }
        public byte FirstAidExtra { get; set; }
    }

    public class LightGadgetJson
    {
        public string Name { get; set; }
        public byte LightGadgetExtra { get; set; } = 1;
    }

    public class HeavyGadgetJson
    {
        public string Name { get; set; }
        public byte HeavyGadgetExtra { get; set; } = 1;
    }

    public class ThrowableJson
    {
        public string Name { get; set; }
        public byte ThrowableExtra { get; set; } = 1;
    }
    
    public class WeaponSkinJson
    {
        public List <WeaponSkinIndexItem> WeaponSkinIndex { get; set; } = new List<WeaponSkinIndexItem>();
    }

    public class WeaponSkinIndexItem
    {
        public string Weapon { get; set; }
        public List <ListSkinIndex > Skins { get; set; }
    }
    public class ListSkinIndex
    {
        public byte SkinIndex { get; set; }
        public string DisplayName { get; set; }
        public bool Available { get; set; } = true;
    }

    public class WeaponSkinIndex
    {
        public WeaponSkinIndexInfo name { get; set; }
    }

    public class WeaponSkinIndexInfo
    {
        public int SkinIndex { get; set; }
        public string DisplayName { get; set; }
    }
}