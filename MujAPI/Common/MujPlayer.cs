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

		public bool IsPremium { get; set; }
		public MapInfo VotedMap { get; set; }
		public PlayerStats Stats { get; set; }
		public bool TrollFlagController { get; set; }

		public MujPlayer(ulong steamID)
		{
			SteamID = steamID;

		}
	}

}
