namespace MujAPI.Common
{
	public class DotEnv
	{
		public static void Load(string path)
		{
			if (!File.Exists(path))
				return;

			foreach (var line in File.ReadAllLines(path))
			{
				var parts = line.Split(
					'=',
					StringSplitOptions.RemoveEmptyEntries);

				if (parts.Length != 2)
					continue;

				Environment.SetEnvironmentVariable(parts[0], parts[1]);
			}
		}
	}
}
