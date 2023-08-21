﻿using System.Net;
using BattleBitAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CommunityServerAPI.ServerExtension.Model;

namespace CommunityServerAPI.Tools
{
    public class PrivilegeManager
    {
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

        public static async Task<ulong> GetPlayerPrivilege(ulong steamID)
        {
            // TODO: 这个代码需要review
            // 判断玩家是否在列表中
            var playerJson = privJson.ListPlayer.FirstOrDefault(x => x.Steam64 == steamID.ToString());

            return playerJson.Role;

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
        public ulong Role { get; set; }
        public string Steam64 { get; set; }
    }
}