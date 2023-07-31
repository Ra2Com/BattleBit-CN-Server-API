using BattleBitAPI;
using BattleBitAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MujAPI
{
	public class MujPlayer : Player
	{

		public bool isPremium { get; set; }
		public MapInfo votedMap { get; set; }
		public PlayerStats stats { get; set; }
		public bool trollFlagController { get; set; }

		public MujPlayer(ulong steamID)
		{
			SteamID = steamID;
		}
	}

}
