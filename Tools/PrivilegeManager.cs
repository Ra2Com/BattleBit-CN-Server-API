using System.Net;
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
    internal static class PrivilegeManager
    {
        public static List<long> UserSteam64 = new List<long>();
        public static PrivilegeJson privJson = new PrivilegeJson();

        public static void Init()
        {
            try
            {
                string filePath = $"{Environment.CurrentDirectory}\\Config\\Privilege.json";
                string content = File.ReadAllText(filePath);
                privJson = JsonConvert.DeserializeObject<PrivilegeJson>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析 Privilege.json 出错，请检查" + ee.Message); // Error when JSON file is wrong
                return;
            }
        }

        //public static PlayerPrivilege GetServerPrivilege()
        //{
        //    PlayerPrivilege prevConfig = new PlayerPrivilege();
        //    // Admin 配置

        //    // Moderator 配置

        //    // VIP 配置

        //    return prevConfig;
        //}
    }

    public class PrivilegeJson
    {
        public List<AdminJson> ListAdmin { get; set; } = new List<AdminJson>();
        public List<ModeratorJson> ListModerator { get; set; } = new List<ModeratorJson>();
        public List<VIPJson> ListVIP { get; set; } = new List<VIPJson>();
    }

    public class AdminJson
    {
        public string UID { get; set; }
        public string Nickname { get; set; }
        public string Steam64 { get; set; }
    }

    public class ModeratorJson
    {
        public string UID { get; set; }
        public string Nickname { get; set; }
        public string Steam64 { get; set; }
    }

    public class VIPJson
    {
        public string UID { get; set; }
        public string Nickname { get; set; }
        public string Steam64 { get; set; }
    }
}
