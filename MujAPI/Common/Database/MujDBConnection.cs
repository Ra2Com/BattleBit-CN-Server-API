using dotenv.net.Utilities;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using static MujAPI.Common.Database.Models;

namespace MujAPI.Common.Database
{
	public class MujDBConnection
	{
		private static readonly MySqlConnection Connection;
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MujDBConnection));

		private static readonly string DbConnection = EnvReader.GetStringValue("DB_CONNECTION");

		static MujDBConnection()
		{
			Connection = new MySqlConnection(DbConnection);
		}

		public static async Task<GameServer> dbAddGameServer(string ServerName, string ServerIPAddress, int ServerPort)
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
				return newGameServer;
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
				.FirstOrDefaultAsync(gs => gs.IPAddress == ServerIPAddress && gs.Port == ServerPort);

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
		public static async Task<List<GameServer>> DbGetServerByPort(int ServerPort)
		{
			await using var dbContext = new MujDbContext();
			var gameServers = dbContext.GameServer
				.Where(gs => gs.Port == ServerPort)
				.ToList();
			return gameServers;
		}


		// player database shit
		// get the player permissions from database
		public static async Task<PlayerPermissions> DbGetPlayerPermissions(ulong SteamId)
		{
			await using var dbContext = new MujDbContext();
			var playerPermissions = dbContext.PlayerPermissions
				.Include(pp => pp.Player)
				.Where(pp => pp.SteamId == (long)SteamId)
				.FirstOrDefaultAsync();
			return await playerPermissions;
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
		// add player to database
		public static async void DbAddPlayer(ulong steamId, string name)
		{
			await using var dbContext = new MujDbContext();

			var newUser = new Player
			{
				SteamId = (long)steamId,
				Name = name
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
		public static async Task DbUpdatePlayerStats(ulong SteamId, DateTime LastTimePlayed , int? Kills = null, int? Deaths = null, int? Wins = null, 
			int? Losses = null, int? Rank = null, int? Exp = null, string? FavouriteWeapon = null, decimal? LongestKill = null,
			int? TotalHeadShots = null, int? TotalPlayTime = null)
		{
			await using var dbContext = new MujDbContext();
			var player = await dbContext.Players.FirstOrDefaultAsync(p => p.SteamId == (long)SteamId);

			if (player != null)
			{
				if (Kills.HasValue)
					player.Kills = Kills.Value;
				
				if (Deaths.HasValue)
					player.Deaths = Deaths.Value;

				if (Wins.HasValue)
					player.Wins = Wins.Value;

				if (Losses.HasValue)
					player.Losses = Losses.Value;

				if (Rank.HasValue)
					player.Rank = Rank.Value;

				if (Exp.HasValue)
					player.Exp = Exp.Value;

				if (FavouriteWeapon != null)
					player.FavouriteWeapon = FavouriteWeapon;

				if (LongestKill.HasValue)
					player.LongestKill = LongestKill.Value;

				if (TotalHeadShots.HasValue)
					player.TotalHeadShots = TotalHeadShots.Value;
				
				if (TotalPlayTime.HasValue)
					player.TotalPlayTime = TotalPlayTime.Value;


				await dbContext.SaveChangesAsync();

			}
			else
			{
				log.Error("Couldn't Find the player");
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
				MotdMessage = motdMessage
			};

			try
			{
				var AddedMotdEntryTask = dbContext.Motd.AddAsync(newMotd);
				await dbContext.SaveChangesAsync();
				var addedMotdEntry = await AddedMotdEntryTask;
				return addedMotdEntry.Entity;
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

	}

}
 