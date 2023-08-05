using Dapper;
using dotenv.net.Utilities;
using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using static MujAPI.Common.Database.Models;

namespace MujAPI.Common.Database
{
	public class MujDBConnection
	{
		private static MySqlConnection connection;
		//logger
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MujDBConnection));

		static MujDBConnection()
		{
			connection = new MySqlConnection(EnvReader.GetStringValue("DB_CONNECTION"));
		}
	
	
		public static IEnumerable<DBGameServer> DBAddGameServer(string ServerName, string ServerIPAddress, int ServerPort) 
		{
			// insert server
			var serverParam = new DynamicParameters();
			serverParam.Add("p_ServerName", ServerName , DbType.String, ParameterDirection.Input);
			serverParam.Add("p_IPAddress", ServerIPAddress , DbType.String, ParameterDirection.Input);
			serverParam.Add("p_Port", ServerPort , DbType.Int16, ParameterDirection.Input);
			serverParam.Add("p_Status", "Online", DbType.String, ParameterDirection.Input);

			try
			{
				var Server = connection.Query<DBGameServer>(
				"InsertGameServer",
				serverParam,
				commandType: CommandType.StoredProcedure);
				return Server;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		public static IEnumerable<DBGameServer> DBUpdateServerStatus(string ServerIPAddress, int ServerPort, string NewStatus)
		{
			if (NewStatus != "Online" && NewStatus != "Offline" && NewStatus != "Maintenance")
				return null;

			var serverParam = new DynamicParameters();
			serverParam.Add("p_IPAddress", ServerIPAddress, DbType.String, ParameterDirection.Input);
			serverParam.Add("p_Port", ServerPort, DbType.Int16, ParameterDirection.Input);
			serverParam.Add("p_Status", NewStatus, DbType.String, ParameterDirection.Input);

			try
			{
				var Server = connection.Query<DBGameServer>(
				"UpdateGameServerStatus",
				serverParam,
				commandType: CommandType.StoredProcedure);
				return Server;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		public static IEnumerable<DBGameServer> DBGetServerByPort(int ServerPort)
		{
			var serverParam = new DynamicParameters();
			serverParam.Add("p_Port", ServerPort, DbType.Int16, ParameterDirection.Input);

			try
			{
				var Server = connection.Query<DBGameServer>(
				"GetServerByPort",
				serverParam,
				commandType: CommandType.StoredProcedure);
				return Server;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		public static IEnumerable<DBMotd> DBGetMotds()
		{
			try
			{
				var motd = connection.Query<DBMotd>(
					"GetMotd",
					commandType: CommandType.StoredProcedure);
				return motd;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return Enumerable.Empty<DBMotd>();
			}
		}
	}

}
