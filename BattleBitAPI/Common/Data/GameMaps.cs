namespace CommunityServerAPI.BattleBitAPI.Common.Data
{
	public class GameMaps : IEquatable<string>, IEquatable<GameMaps>
	{
		public string Name { get; private set; }
		public GameMaps(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return this.Name;
		}
		public bool Equals(string other)
		{
			if (other == null)
				return false;
			return this.Name.Equals(other);
		}
		public bool Equals(GameMaps other)
		{
			if (other == null)
				return false;
			return this.Name.Equals(other.Name);
		}

		public static bool operator ==(string left, GameMaps right)
		{
			bool leftNull = object.ReferenceEquals(left, null);
			bool rightNull = object.ReferenceEquals(right, null);
			if (leftNull && rightNull)
				return true;
			if (leftNull || rightNull)
				return false;
			return right.Name.Equals(left);
		}
		public static bool operator !=(string left, GameMaps right)
		{
			bool leftNull = object.ReferenceEquals(left, null);
			bool rightNull = object.ReferenceEquals(right, null);
			if (leftNull && rightNull)
				return true;
			if (leftNull || rightNull)
				return false;
			return right.Name.Equals(left);
		}
		public static bool operator ==(GameMaps right, string left)
		{
			bool leftNull = object.ReferenceEquals(left, null);
			bool rightNull = object.ReferenceEquals(right, null);
			if (leftNull && rightNull)
				return true;
			if (leftNull || rightNull)
				return false;
			return right.Name.Equals(left);
		}
		public static bool operator !=(GameMaps right, string left)
		{
			bool leftNull = object.ReferenceEquals(left, null);
			bool rightNull = object.ReferenceEquals(right, null);
			if (leftNull && rightNull)
				return true;
			if (leftNull || rightNull)
				return false;
			return right.Name.Equals(left);
		}
	}
}
