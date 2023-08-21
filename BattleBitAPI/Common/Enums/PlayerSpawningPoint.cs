namespace CommunityServerAPI.BattleBitAPI.Common
{
	public enum PlayerSpawningPosition : byte
	{
		// 在指定点
		SpawnAtPoint, 
		// 在小队长放置的集结点
		SpawnAtRally, 
		// 在队友附近
		SpawnAtFriend,
		// 在载具中 
		SpawnAtVehicle, 
		Null
	}
}
