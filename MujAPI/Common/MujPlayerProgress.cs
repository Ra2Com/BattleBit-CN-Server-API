using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleBitAPI.Common;

namespace MujAPI.Common
{
	public class MujPlayerProgress : PlayerStats.PlayerProgess
	{

		public int Kills { get; set; }
		public int Deaths { get; set; }
		public int Wins { get; set; }
		public int Losses { get; set; }
		public int Rank { get; set; }
		public int Exp { get; set; }
		public string FavouriteWeapon { get; set; }
		public decimal LongestKill { get; set; }
		public int TotalHeadShots { get; set; }


		public MujPlayerProgress(MujPlayerProgress playerProgress)
		{
			this.KillCount = (uint)playerProgress.Kills;
			this.DeathCount = (uint)playerProgress.Deaths;
			this.WinCount = (uint)playerProgress.Wins;
			this.LoseCount = (uint)playerProgress.Losses;
			
		}
	}
}
