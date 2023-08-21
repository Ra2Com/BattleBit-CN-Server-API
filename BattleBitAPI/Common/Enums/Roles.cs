namespace CommunityServerAPI.BattleBitAPI.Common.Enums
{
	public enum Roles : ulong
	{
		None = 0,

		// 超级管理员
		Admin = 1 << 0,
		// 管理员
		Moderator = 1 << 1,
		// 内部人员
		Special = 1 << 2,
		// VIP玩家
		Vip = 1 << 3,
	}
}
