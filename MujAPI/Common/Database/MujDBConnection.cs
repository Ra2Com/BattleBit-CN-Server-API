using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using static MujAPI.Common.Database.Models;

namespace MujAPI.Common.Database
{
	public class MujDBConnection
	{
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MujDBConnection));

		// gameserver shit
		// add the game server to the database
		public static async Task<GameServer> DbAddGameServer(string ServerName, string ServerIPAddress, int ServerPort)
		{
			await using var dbContext = new MujDbContext();

			var newGameServer = new GameServer
			{
				ServerName = ServerName,
				IPAddress = ServerIPAddress,
				Port = ServerPort,
				Status = "Online"
			};

			try
			{
				dbContext.GameServer.Add(newGameServer);
				await dbContext.SaveChangesAsync();
				var createdGameServer = await dbContext.GameServer.FindAsync(newGameServer.GameServerId);
				return createdGameServer;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}

		}

		// update the game server status to db
		public static async Task<GameServer> DbUpdateServerStatus(string serverName, string ServerIPAddress, int ServerPort, string NewStatus)
		{
			if (NewStatus != "Online" && NewStatus != "Offline" && NewStatus != "Maintenance")
				return null;

			await using var dbContext = new MujDbContext();

			// check if the server exists
			var existingServer = await dbContext.GameServer
				.FirstAsync(gs => gs.IPAddress == ServerIPAddress && gs.Port == ServerPort);

			if (existingServer == null)
				return null;

			// update the server that exists
			existingServer.ServerName = serverName;
			existingServer.Status = NewStatus;
			try
			{
				dbContext.GameServer.Update(existingServer);
				await dbContext.SaveChangesAsync();
				return existingServer;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// get the server by its port
		public static async Task<GameServer> DbGetServerByPort(int ServerPort)
		{
			await using var dbContext = new MujDbContext();
			var gameServers = await dbContext.GameServer
				.Where(gs => gs.Port == ServerPort)
				.FirstAsync();
			return gameServers;
		}

		// get the server by its ip and port
		public static async Task<GameServer> DbGetServerByIpAndPort(string IPAddress, int ServerPort)
		{
			await using var dbContext = new MujDbContext();
			var gameServer = await dbContext.GameServer
				.Where(gs => gs.IPAddress == IPAddress && gs.Port == ServerPort)
				.FirstAsync();
			return gameServer;
		}

		// player database shit
		// get the player permissions from database
		public static async Task<PlayerPermissions> DbGetPlayerPermissions(ulong SteamId)
		{
			await using var dbContext = new MujDbContext();
			var playerPermissions = await dbContext.PlayerPermissions
				.Include(pp => pp.Player)
				.Where(pp => pp.SteamId == (long)SteamId)
				.FirstAsync();
				return playerPermissions;
		}
		
		// get players in database
		public static async Task<List<Player>> DbGetPlayers()
		{
			await using var dbContext = new MujDbContext();
			try
			{
				var players = dbContext.Players.ToList();
				return players;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// get player stat from database
		public static async Task<Player> DbGetPlayer(ulong SteamId)
		{
			await using var dbContext = new MujDbContext();
			try
			{
				var player = await dbContext.Players.FirstAsync(p => p.SteamId == (long)SteamId);
				return player;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// add player to database
		public static async void DbAddPlayer(ulong steamId, string name)
		{
			await using var dbContext = new MujDbContext();

			var newUser = new Player
			{
				SteamId = (long)steamId,
				Name = name,
				CreatedAt = DateTime.Now
			};

			try
			{
				dbContext.Players.Add(newUser);
				await dbContext.SaveChangesAsync();
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// update player stats
		public static async Task DbUpdatePlayerStats(ulong steamId, DateTime lastTimePlayed , int? kills = null, int? deaths = null, int? wins = null, 
			int? losses = null, int? rank = null, int? exp = null, string? favouriteWeapon = null, decimal? longestKill = null,
			int? totalHeadShots = null, int? totalPlayTime = null)
		{
			await using var dbContext = new MujDbContext();
			var player = await dbContext.Players.FirstAsync(p => p.SteamId == (long)steamId);

			if (player != null)
			{
				if (kills.HasValue)
					player.Kills = kills.Value;
				if (deaths.HasValue)
					player.Deaths = deaths.Value;
				if (wins.HasValue)
					player.Wins = wins.Value;
				if (losses.HasValue)
					player.Losses = losses.Value;
				if (rank.HasValue)
					player.Rank = rank.Value;
				if (exp.HasValue)
					player.Exp = exp.Value;
				if (favouriteWeapon != null)
					player.FavouriteWeapon = favouriteWeapon;
				if (longestKill.HasValue)
					player.LongestKill = longestKill.Value;
				if (totalHeadShots.HasValue)
					player.TotalHeadShots = totalHeadShots.Value;
				if (totalPlayTime.HasValue)
					player.TotalPlayTime = totalPlayTime.Value;
				player.LastTimePlayed = lastTimePlayed;

				await dbContext.SaveChangesAsync();
				log.Info($"{steamId} saved player stats");
			}
			else
			{
				log.Error("Couldn't Find the player");
			}
		}

		// get player warns
		public static async Task<List<PlayerWarnings>> DbGetPlayerWarns(ulong SteamId)
		{
			await using var dbContext = new MujDbContext();

			try
			{
				var Warnings = await dbContext.PlayerWarnings
					.Where(pw => pw.SteamId == (long)SteamId)
					.ToListAsync();
				if (Warnings.Count == 0)
					log.Error("No player warns found");
				return Warnings;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// add player warn
		public static async Task<PlayerWarnings> DbAddPlayerWarn(ulong SteamId, string Warning)
		{
			await using var dbContext = new MujDbContext();
			var newWarn = new PlayerWarnings
			{
				SteamId = (long)SteamId,
				Message = Warning,
				CreatedAt = DateTime.Now
			};
			
			try
			{
				dbContext.PlayerWarnings.Add(newWarn);
				await dbContext.SaveChangesAsync();
				var createdWarn = await dbContext.PlayerWarnings.FindAsync(newWarn.Id);
				return createdWarn;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// get message of the day's from database
		public static async Task<List<Motd>> DbGetMotds()
		{
			await using var dbContext = new MujDbContext();
			try
			{
				var Motds = dbContext.Motd.ToList();
				return Motds;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// add motd to database
		public static async Task<Motd> DbAddMotd(string motdMessage)
		{
			await using var dbContext = new MujDbContext();

			var newMotd = new Motd
			{
				MotdMessage = motdMessage,
				CreatedAt = DateTime.Now
			};

			try
			{
				var addedMotdEntryTask = dbContext.Motd.AddAsync(newMotd);
				await dbContext.SaveChangesAsync();
				var addedMotdEntry = await addedMotdEntryTask;
				log.Info($"Motd Message:{motdMessage} sent to database");
				return addedMotdEntry.Entity;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		// matchdata shit
		// add the matchdata 
		// update the matchdata
		// get the matchdata
		// remove the matchdata

		// team data shit
		// add the team data
		// update the team data
		// get the team data
		// remove the team data

		// team player shit
		// add the team player data
		// update the team player data
		// get the team player data
		// remove the team player data
	}

}
 