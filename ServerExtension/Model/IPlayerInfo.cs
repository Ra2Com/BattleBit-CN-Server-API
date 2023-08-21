using BattleBitAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.ServerExtension.Model
{
    /// <summary>
    /// Player Infomation Custom Component.
    /// </summary>
    /// 
    /// <remarks>
    /// Player Info includes player K/D, Lifetime XP in your server, marks in Revenger Mode and etc.<br/>
    /// </remarks>
    internal interface IPlayerInfo
    {
        public ulong SteamID { get; }
        public int K { get; set; }
        public int D { get; set; }
        public int HSKill { get; set; }

        public int rank { get; set; }

        public ulong markId { get; set; }
        public float maxHP { get; set; }

        public List<PositionBef> positionBef { get; set; }

        public PlayerStats stats { get; set; }
    }
}