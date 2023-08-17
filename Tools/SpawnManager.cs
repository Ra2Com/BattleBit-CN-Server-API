using BattleBitAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommunityServerAPI.Tools
{
    internal static class SpawnManager
    {
        public static List<WeaponItem> PrimaryWeapons = new List<WeaponItem>();
        public static LoadoutJson loadoutJson = new LoadoutJson();

        public static void Init()
        {
            try
            {
                string filepath = $"{Environment.CurrentDirectory}\\Loadout.json";
                string content = File.ReadAllText(filepath);
                loadoutJson = JsonConvert.DeserializeObject<LoadoutJson>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析json出错" + ee.Message);
                return;
            }
            //LoadoutJson loadoutJson = new LoadoutJson();
            //loadoutJson.ListPrimaryWeapon.Add(new PrimaryWeaponJson { Name = "AK74", Barrel = "Ranger", MainSight = "Holographic", SideRail = "VerticalGrip", UnderRail = "TacticalFlashlight" });
            //loadoutJson.ListSecondaryWeapon.Add(new SecondaryWeaponJson { Name = "USP", MainSight = "PistolRedDot" });
            //loadoutJson.ListHeavyGadget.Add(new HeavyGadgetJson { Name = "C4" });
            //loadoutJson.ListLightGadget.Add(new LightGadgetJson { Name = "Small Ammo Kit" });
            //loadoutJson.ListThrowable.Add(new ThrowableJson { Name = "Flashbang" });

            //var x = JsonConvert.SerializeObject(loadoutJson);
        }

        public static PlayerLoadout GetRandom()
        {
            PlayerLoadout playerLoadout = new PlayerLoadout();

            Random rd = new Random();

            int PrimaryWeaponIndex = rd.Next(0, loadoutJson.ListPrimaryWeapon.Count - 1);
            var wi = new WeaponItem();
            wi.ToolName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Name ? default : "none";
            wi.MainSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].MainSight ? default : "none";
            wi.TopSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].TopSight ? default : "none";
            wi.CantedSightName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].CantedSight ? default : "none";
            wi.BarrelName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Barrel ? default : "none";
            wi.SideRailName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].SideRail ? default : "none";
            wi.UnderRailName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].UnderRail ? default : "none";
            wi.BoltName = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].Bolt ? default : "none";
            wi.SkinIndexNumber = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].SkinIndex ? default : 0;
            wi.MagazineIndexNumber = loadoutJson.ListPrimaryWeapon[PrimaryWeaponIndex].MagazineIndex ? default : 0;
            playerLoadout.PrimaryWeapon = wi;

            int SecondaryWeaponIndex = rd.Next(0, loadoutJson.ListSecondaryWeapon.Count - 1);
            var wi2 = new WeaponItem();
            wi2.ToolName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Name;
            wi2.BarrelName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].Barrel ? default : "none";
            wi2.MainSightName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MainSight ? default : "none";
            wi2.SideRailName = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SideRail ? default : "none";
            wi2.SkinIndexNumber = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].SkinIndex ? default : 0;
            wi2.MagazineIndexNumber = loadoutJson.ListSecondaryWeapon[SecondaryWeaponIndex].MagazineIndex ? default : 0;
            playerLoadout.SecondaryWeapon = wi2;

            int ListHeavyGadgetIndex = rd.Next(0, loadoutJson.ListHeavyGadget.Count - 1);
            playerLoadout.HeavyGadgetName = loadoutJson.ListHeavyGadget[ListHeavyGadgetIndex].Name ? default : "none";

            int ListLightGadgetIndex = rd.Next(0, loadoutJson.ListLightGadget.Count - 1);
            playerLoadout.LightGadgetName = loadoutJson.ListLightGadget[ListLightGadgetIndex].Name ? default : "none";

            int ListThrowableIndex = rd.Next(0, loadoutJson.ListThrowable.Count - 1);
            playerLoadout.ThrowableName = loadoutJson.ListThrowable[ListThrowableIndex].Name ? default : "none";

            return playerLoadout;
        }
    }

    public class LoadoutJson
    {
        public List<PrimaryWeaponJson> ListPrimaryWeapon { get; set; } = new List<PrimaryWeaponJson>();

        public List<SecondaryWeaponJson> ListSecondaryWeapon { get; set; } = new List<SecondaryWeaponJson>();

        public List<HeavyGadgetJson> ListHeavyGadget { get; set; } = new List<HeavyGadgetJson>();
        public List<LightGadgetJson> ListLightGadget { get; set; } = new List<LightGadgetJson>();
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
    }

    public class SecondaryWeaponJson
    {
        public string Name { get; set; }
        public string Barrel { get; set; }
        public string MainSight { get; set; }
        public string SideRail { get; set; }
        public string SkinIndex { get; set; }
        public string MagazineIndex { get; set; }
    }

    public class HeavyGadgetJson
    {
        public string Name { get; set; }
    }

    public class LightGadgetJson
    {
        public string Name { get; set; }
    }
    public class ThrowableJson
    {
        public string Name { get; set; }
    }
}
