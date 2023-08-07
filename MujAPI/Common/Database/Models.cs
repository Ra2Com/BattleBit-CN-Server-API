using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MujAPI.Common.Database
{
	public class Models
	{

		public record DBPlayers(Int64 SteamId, string Name, DateTime LastTimePlayed, DateTime CreatedAt, int Kills, int Deaths, int Wins,
			int Losses, int Rank, int Exp, string FavouriteWeapon, decimal LongestKill, int TotalHeadShots, int TotalPlayTime);

		public record DBPlayerPermissions(Int64 SteamId, bool IsAdmin, bool IsModerator, bool IsVip, 
			bool IsSpecial, bool IsTrollFlagController, bool IsPremium, bool IsBanned);

		public record DBGameServer(int GameServerId, string ServerName, string IPAddress, int Port, string Status, DateTime CreatedAt);

		public record DBMotd(int MotdId, string Motd, DateTime CreatedAt)
		{
			public DBMotd() : this(default, default, default)
			{
			}
		}
	}
}
