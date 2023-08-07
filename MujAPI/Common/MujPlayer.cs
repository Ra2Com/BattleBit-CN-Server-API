using BattleBitAPI;
using BattleBitAPI.Common;
using MujAPI.Common;

namespace MujAPI
{
	public class MujPlayer : Player
	{

		public bool IsPremium { get; set; }
		public MapInfo VotedMap { get; set; }
		public PlayerStats Stats { get; set; }
		public bool TrollFlagController { get; set; }
		public int TimesBullied { get; set; }

		public MujPlayer(ulong steamID)
		{
			SteamID = steamID;

		}
	}

}
