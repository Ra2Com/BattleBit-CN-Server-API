using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MujAPI.Common.Database
{
	public class Models
	{

		public record Players(Int64 SteamId, string Name, DateTime LastTimePlayed, DateTime CreatedAt, int Kills, int Deaths, int Wins,
			int Losses, int Rank, int Exp, string FavouriteWeapon, decimal LongestKill, int TotalHeadShots, int TotalPlayTime);

		public record PlayerPermissions(Int64 SteamId, bool IsAdmin, bool IsModerator, bool IsVip, 
			bool IsSpecial, bool IsTrollFlagController, bool IsPremium, bool IsBanned);

	}
}
