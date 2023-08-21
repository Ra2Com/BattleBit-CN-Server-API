namespace CommunityServerAPI.BattleBitAPI.Common
{
	public enum ReportReason
	{
		// 作弊
		Cheating_WallHack = 0,
		Cheating_Aimbot = 1,
		Cheating_Speedhack = 2,

		// 种族主义
		Racism_Discrimination_Text = 3,
		Racism_Discrimination_Voice = 4,

		// 聊天、语音刷屏占用
		Spamming_Text = 5,
		Spamming_Voice = 6,

		// 聊天、语音骚扰、辱骂
		Bad_Language_Text = 7,
		Bad_Language_Voice = 8,

		// 刷分
		Griefing = 9,
		// 使用BUG
		Exploiting = 10,
		// 其它问题
		OtherToxicBehaviour = 11,
		// 用户头像不合适
		UserProfileNamePicture = 12,
	}
}
