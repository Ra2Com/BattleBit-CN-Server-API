using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MujAPI.Common.Database
{
	public class Models
	{

		public class MujDbContext : DbContext
		{
			private static string ConnectionString = EnvReader.GetStringValue("DB_CONNECTION");

			public DbSet<Player> Players { get; set; }
			public DbSet<PlayerPermissions> PlayerPermissions { get; set; }
			public DbSet<PlayerWarnings> PlayerWarnings { get; set; }
			public DbSet<PlayerWeaponStats> PlayerWeaponStats { get; set; }

			public DbSet<MapRotation> MapRotations { get; set; }
			public DbSet<GameModeRotation> GameModeRotations { get; set; }
			public DbSet<Motd> Motd { get; set; }
			public DbSet<WeaponBan> WeaponBan { get; set; }
			public DbSet<GameServer> GameServer { get; set; }
			
			public DbSet<MatchData> MatchData { get; set; }
			public DbSet<TeamData> TeamData { get; set; }
			public DbSet<TeamPlayer> TeamPlayer { get; set; }

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
			}

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				//set primary keys
				modelBuilder.Entity<Player>()
					.HasKey(p => p.SteamId);
				modelBuilder.Entity<PlayerPermissions>()
					.HasKey(p => p.SteamId);
				modelBuilder.Entity<PlayerWarnings>()
					.HasKey(p => p.Id);
				modelBuilder.Entity<PlayerWeaponStats>()
					.HasKey(p => p.Id);
				modelBuilder.Entity<GameServer>()
					.HasKey(p => p.GameServerId);
				modelBuilder.Entity<MapRotation>()
					.HasKey(mr => mr.MapRotationId);
				modelBuilder.Entity<GameModeRotation>()
					.HasKey(mr => mr.GameModeRotationId);
				modelBuilder.Entity<Motd>()
					.HasKey(mr => mr.MotdId);
				modelBuilder.Entity<WeaponBan>()
					.HasKey(mr => mr.WeaponBanId);
				modelBuilder.Entity<MatchData>()
					.HasKey(mr => mr.MatchId);
				modelBuilder.Entity<TeamData>()
					.HasKey(mr => mr.TeamId);
				modelBuilder.Entity<TeamPlayer>()
					.HasKey(mr => mr.TeamId);



				// player shit
				modelBuilder.Entity<Player>()
					.HasMany(p => p.PlayerWarnings)
					.WithOne(pw => pw.Player)
					.HasForeignKey(w => w.SteamId);

				modelBuilder.Entity<Player>()
					.HasMany(p => p.PlayerWeaponStats)
					.WithOne(ws => ws.Player)
					.HasForeignKey(ws => ws.SteamId);

				modelBuilder.Entity<Player>()
					.HasOne(p => p.PlayerPermissions)
					.WithOne(pp => pp.Player)
					.HasForeignKey<PlayerPermissions>(pp => pp.SteamId);
				
				modelBuilder.Entity<PlayerWarnings>()
					.HasOne(p => p.Player)
					.WithMany(pw => pw.PlayerWarnings)
					.HasForeignKey(pw => pw.SteamId);

				// game server shit
				modelBuilder.Entity<GameServer>()
					.HasMany(gs => gs.Motd)
					.WithMany()
					.UsingEntity(j => j.ToTable("GameServerMotd"));

				modelBuilder.Entity<GameServer>()
					.HasMany(gs => gs.MatchData)
					.WithOne(md => md.GameServer)
					.HasForeignKey(md => md.GameServerId);

				// match data shit
				modelBuilder.Entity<MatchData>()
					.HasMany(md => md.TeamData)
					.WithOne(td => td.MatchData)
					.HasForeignKey(td => td.MatchId);

				// team data shit
				modelBuilder.Entity<TeamData>()
					.HasMany(td => td.TeamPlayers)
					.WithOne(tp => tp.TeamData)
					.HasForeignKey(tp => tp.TeamId);

				// team player shit
				modelBuilder.Entity<TeamPlayer>()
					.HasOne(tp => tp.TeamData)
					.WithMany(td => td.TeamPlayers)
					.HasForeignKey(tp => tp.TeamId);

				modelBuilder.Entity<TeamPlayer>()
					.HasOne(tp => tp.Player)
					.WithMany()
					.HasForeignKey(tp => tp.PlayerId);

			}
		}

		public class Player
		{
			public Int64 SteamId { get; set; }
			public string Name { get; set; }
			public DateTime LastTimePlayed { get; set; }
			public DateTime CreatedAt { get; set; }
			public int Kills { get; set; }
			public int Deaths { get; set; }
			public int Wins { get; set; }
			public int Losses { get; set; }
			public int Rank { get; set; }
			public int Exp { get; set; }
			public string FavouriteWeapon { get; set; }
			public decimal LongestKill { get; set; }
			public int TotalHeadShots { get; set; }
			public int TotalPlayTime { get; set; }

			public PlayerPermissions PlayerPermissions { get; set; }
			public ICollection<PlayerWarnings> PlayerWarnings { get; set; } = new List<PlayerWarnings>();
			public ICollection<PlayerWeaponStats> PlayerWeaponStats { get; set; } = new List<PlayerWeaponStats>();
		}

		public class PlayerPermissions
		{
			public Int64 SteamId { get; set; }
			public bool IsAdmin { get; set; }
			public bool IsModerator { get; set; }
			public bool IsVip { get; set; }
			public bool IsSpecial { get; set; }
			public bool IsTrollFlagController { get; set; }
			public bool IsPremium { get; set; }
			public bool IsBanned { get; set; }

			public Player Player { get; set; }

		}

		public class PlayerWarnings
		{
			public int Id { get; set; }
			public Int64 SteamId { get; set; }
			public string Message { get; set; }

			public Player Player { get; set; }
		}

		public class PlayerWeaponStats
		{
			public int Id { get; set; }
			public Int64 SteamId { get; set; }
			public string WeaponName { get; set; }
			public int Kills { get; set; }
			public int HeadShots { get; set; }
			public decimal Accuracy { get; set; }
			public DateTime CreatedAt { get; set; }

			public Player Player { get; set; }
		}

		public class MapRotation
		{
			public int MapRotationId { get; set; }
			public string MapName { get; set; }
			public int Position { get; set; }
			public DateTime CreatedAt { get; set; }
		}

		public class GameModeRotation
		{
			public int GameModeRotationId { get; set; }
			public string GameModeName { get; set; }
			public int Position { get; set; }
			public DateTime CreatedAt { get; set; }
		}

		public class Motd
		{
			public int MotdId { get; set; }
			public string MotdMessage { get; set; }
			public DateTime CreatedAt { get; set; }
		}

		public class WeaponBan
		{
			public int WeaponBanId { get; set; }
			public string WeaponName { get; set; }
			public DateTime CreatedAt { get; set; }
		}

		public class GameServer
		{
			public int GameServerId { get; set; }
			public string ServerName { get; set; }
			public string IPAddress { get; set; }
			public int Port { get; set; }
			public string Status { get; set; }
			public DateTime CreatedAt { get; set; }

			public ICollection<Motd> Motd { get; set; }
			public ICollection<MatchData> MatchData { get; set; }

		}

		public class MatchData
		{
			public int MatchId { get; set; }
			public int GameServerId { get; set; }
			public DateTime CreatedAt { get; set; }
			public int USAPoints { get; set; }
			public int USAKills { get; set; }
			public int USADeaths { get; set; }
			public int RUSPoints { get; set; }
			public int RUSKills { get; set; }
			public int RUSDeaths { get; set; }

			public GameServer GameServer { get; set; }
			public ICollection<TeamData> TeamData { get; set; } = new List<TeamData>();
		}

		public class TeamData
		{
			public int MatchId { get; set; }
			public int TeamId { get; set; }
			public DateTime CreatedAt { get; set; }
			public int Kills { get; set; }
			public int Deaths { get; set; }

			public MatchData MatchData { get; set; }
			public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
		}

		public class TeamPlayer
		{
			public int TeamId { get; set; }
			public Int64 PlayerId { get; set; }
			public DateTime CreatedAt { get; set; }
			public int Kills { get; set; }
			public int Deaths { get; set; }

			public TeamData TeamData { get; set; }
			public Player Player { get; set; }
		}

		public record DBGameServer(int GameServerId, string ServerName, string IPAddress, int Port, string Status, DateTime CreatedAt);
		
	}

}
