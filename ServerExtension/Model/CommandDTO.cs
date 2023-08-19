using BattleBitAPI;
using CommunityServerAPI.ServerExtension.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Model
{
    public class CommandDTO
    {
        public CommandTypes CommandType { get; set; }
        public ulong SteamId { get; set; }
        public string Executor { get; set; }
        public Player<MyPlayer> Target { get; set; }
        public ulong TargetSteamId { get; set; }
        public string Message { get; set; }
        public Vector3 Location { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string Reason { get; set; }
        public bool Error { get; set; }
    }
}
