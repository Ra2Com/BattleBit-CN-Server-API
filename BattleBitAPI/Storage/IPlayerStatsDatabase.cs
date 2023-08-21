using CommunityServerAPI.BattleBitAPI.Common.Data;

namespace CommunityServerAPI.BattleBitAPI.Storage
{
	public interface IPlayerStatsDatabase
	{
		public Task<PlayerStats> GetPlayerStatsOf(ulong steamID);
		public Task SavePlayerStatsOf(ulong steamID, PlayerStats stats);
	}
}
