namespace CommunityServerAPI.BattleBitAPI.Common.Enums
{
    public enum GameState : byte
    {
        // 等待足够玩家开始游戏
        WaitingForPlayers = 0,
        // 开始游戏倒计时
        CountingDown = 1,
        // 对局中
        Playing = 2,
        // 对局结算中
        EndingGame = 3
    }
}
