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

			// insert server
			var serverParam = new DynamicParameters();
			serverParam.Add("p_ServerName", "UK#1 Muj Test Server", DbType.String, ParameterDirection.Input);
			serverParam.Add("p_IPAddress", "255.255.255.255", DbType.String, ParameterDirection.Input);
			serverParam.Add("p_Port", 30000, DbType.Int16, ParameterDirection.Input);
			serverParam.Add("p_Status", "Offline", DbType.String, ParameterDirection.Input);

			var InsertServer = connection.Query<GameServer>(
				"InsertGameServer",
				serverParam,
				commandType: CommandType.StoredProcedure
				);

			//get server by port
			var serverPortParam = new DynamicParameters();
			serverPortParam.Add("p_Port", 30000, DbType.Int16, ParameterDirection.Input);
			var Server = connection.Query<GameServer>(
			"GetServerByPort",
			serverPortParam,
			commandType: CommandType.StoredProcedure
			);


			Console.WriteLine(string.Join(Environment.NewLine,
				Server.Select(u => $"{u.GameServerId}, {u.ServerName}, {u.IPAddress}, {u.Port}, {u.Status}, {u.CreatedAt}")));
		}

	}
}
