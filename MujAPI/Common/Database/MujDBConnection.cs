using Dapper;
using dotenv.net.Utilities;
using MySql.Data.MySqlClient;
using System.Data;
using static MujAPI.Common.Database.Models;

namespace MujAPI.Common.Database
{
	public class MujDBConnection
	{

		public static void Start() 
		{
			using var connection = new MySqlConnection(EnvReader.GetStringValue("DB_CONNECTION"));

			var param = new DynamicParameters();
			param.Add("identifier", 1234567890, DbType.Int64, ParameterDirection.Input);

			var user = connection.Query<Players>(
				"GetUser", 
				param, 
				commandType: CommandType.StoredProcedure
				);
		
			Console.WriteLine(string.Join(Environment.NewLine,
				user.Select(u => $"{u.SteamId}, {u.Name}, {u.LastTimePlayed}, {u.CreatedAt}, {u.Kills}, {u.Deaths}, {u.Wins}, {u.Losses}, {u.Rank}, {u.Exp}," +
				$" {u.FavouriteWeapon}, {u.LongestKill}, {u.TotalHeadShots}, {u.TotalPlayTime}")
				));

			Console.WriteLine();

			var users = connection.Query<Players>(
				"GetUsers",
				commandType: CommandType.StoredProcedure
				);

			Console.WriteLine(string.Join(Environment.NewLine,
				users.Select(u => $"{u.SteamId}, {u.Name}, {u.LastTimePlayed}, {u.CreatedAt}, {u.Kills}, {u.Deaths}, {u.Wins}, {u.Losses}, {u.Rank}, {u.Exp}," +
				$" {u.FavouriteWeapon}, {u.LongestKill}, {u.TotalHeadShots}, {u.TotalPlayTime}")
				));

			var user2 = connection.Query<PlayerPermissions>(
				"GetUserPermissions",
				param,
				commandType: CommandType.StoredProcedure
				);
			var isadmin = user2.Select(u => u.IsAdmin ? "Yes" : "No").ToList();

			Console.WriteLine(string.Join(Environment.NewLine,
				user2.Select(u => $"{u.SteamId}, Admin? {isadmin.First()}")));
		}

	}
}
