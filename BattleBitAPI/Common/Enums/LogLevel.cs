namespace CommunityServerAPI.BattleBitAPI.Common
{
    [System.Flags]
    public enum LogLevel : ulong
    {
        None = 0,

        /// <summary>
        /// 底层 Socket 的日志 
        /// </summary>
        Sockets = 1 << 0,

        /// <summary>
        /// 游戏服务器的错误日志 —— 建议开启
        /// </summary>
        GameServerErrors = 1 << 1,
        
        /// <summary>
        /// 游戏服务器的链接、重连日志
        /// </summary>
        GameServers = 1 << 2,

        /// <summary>
        /// 玩家的链接、离线日志
        /// </summary>
        Players = 1 << 3,

        /// <summary>
        /// 小队的变化的日志（加入、离开等）
        /// </summary>
        Squads = 1 << 4,

        /// <summary>
        /// 击倒、放弃、复活、重生等日志
        /// </summary>
        KillsAndSpawns = 1 << 5,

        /// <summary>
        /// 玩家更换游戏角色的日志（更换成突击兵、医疗兵等）
        /// </summary>
        Roles = 1 << 6,

        /// <summary>
        /// 玩家生命值的变化日志（收到伤害、治疗等）
        /// </summary>
        HealtChanges = 1 << 7,

        /// <summary>
        /// Output everything.
        /// </summary>
        All = ulong.MaxValue,
    }
}
