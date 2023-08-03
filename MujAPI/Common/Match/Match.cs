using BattleBitAPI;
using BattleBitAPI.Common;
using BattleBitAPI.Server;

namespace MujAPI
{
	public class Match
	{
		public int MatchID { get; set; }
		public GameServer GameServer { get; set; }
		public Dictionary<Team, int> TeamKills { get; set; }
		public Dictionary<Squads, int> SquadKills { get; set; }

		public Match(int matchid, GameServer gameServer) 
		{
			this.MatchID = matchid;
			this.TeamKills = new Dictionary<Team, int>();
			this.SquadKills = new Dictionary<Squads, int>();
			this.GameServer = gameServer;
		}

		public void IncrementTeamKill(Player player)
		{
			if (TeamKills.ContainsKey(player.Team))
				TeamKills[player.Team]++;
			else
				TeamKills[player.Team] = 1;
		}

		public void DecrementTeamKill(Player player)
		{
			if (TeamKills.ContainsKey(player.Team))
				TeamKills[player.Team]--;
			else
				TeamKills[player.Team] = 0;
		}

		public void IncrementSquadKill(Player player)
		{
			if (SquadKills.ContainsKey(player.Squad))
				SquadKills[player.Squad]++;
			else
				SquadKills[player.Squad] = 1;
		}

		public void DecrementSquadKill(Player player)
		{
			if (SquadKills.ContainsKey(player.Squad))
				SquadKills[player.Squad]--;
			else
				SquadKills[player.Squad] = 0;
		}

		public int GetTeamKills(Team team)
		{
			int TeamKillsValue = TeamKills.TryGetValue(team, out int Kills) ? Kills : 0;
			return TeamKillsValue;
		}

		public int GetSquadKills(Squads squads)
		{
			int SquadKillsValue = SquadKills.TryGetValue(squads, out int Kills) ? Kills : 0;
			return SquadKillsValue;
		}

	}
}
