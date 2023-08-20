using System.Net;
using BattleBitAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.Tools
{
    internal class PrivilegeManager
    {
        public static PrivilegeJson privJson = new PrivilegeJson();
        string[] serverRoles = new string[] { "None", "Admin", "Moderator", "Special", "Vip" };
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

        public void GetPlayerPrivilege(ulong steamid, PlayerJoiningArguments args)
        {
            // 判断玩家是否在列表中
            var playerJson = privJson.ListPlayer.Find(x => x.Steam64 == steamid.ToString());
            if (playerJson == null)
            {
                // 不在列表中，给予默认权限
                args.Stats.Roles = Roles.None;
                return;
            }
            else
            {
                // 在列表中，判断是否有权限
                if (PlayerJson.Role.Contains(playerJson.Role))
                {
                    int index = Array.IndexOf(serverRoles, playerJson.Role);
                    // 有权限，直接返回
                    args.Stats.Roles = Roles.None.playerJson.Role;
                    return;
                }
                else
                {
                    // 没有权限，给予默认权限
                    args.Stats.Roles = Roles.None;
                    return;
                }
            }
        }
    }

    public class PrivilegeJson
    {
        public List<PlayerJson> ListPlayer { get; set; } = new List<PlayerJson>();
    }

    public class PlayerJson
    {
        public string UID { get; set; }
        public string Nickname { get; set; }
        public string Role { get; set; }
        public string Steam64 { get; set; }
    }
}
