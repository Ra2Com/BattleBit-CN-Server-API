using System.Numerics;

namespace BattleBitAPI.Common
{
    /// <summary>
    /// 当玩家被击杀时的参数
    /// </summary>
    /// <typeparam name="TPlayer"></typeparam> <summary>
    /// 玩家被击杀时可以获取到击杀着、玩家，2人的位置，击杀武器或道具，伤害部位，伤害来源
    /// </summary>
    public struct OnPlayerKillArguments<TPlayer> where TPlayer : Player<TPlayer>
    {
        public TPlayer Killer;
        public Vector3 KillerPosition;

        public TPlayer Victim;
        public Vector3 VictimPosition;

        public string KillerTool;
        public PlayerBody BodyPart;
        public ReasonOfDamage SourceOfDamage;
    }
}
