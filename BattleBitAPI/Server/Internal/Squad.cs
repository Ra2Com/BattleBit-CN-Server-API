using BattleBitAPI.Common;

namespace BattleBitAPI.Server
{
    public class Squad<TPlayer> where TPlayer : Player<TPlayer>
    {
        // 团队阵营
        public Team Team => @internal.Team;
        // 小队
        public Squads Name => @internal.Name;
        // 服务器
        public GameServer<TPlayer> Server => @internal.Server;
        // 人数
        public int NumberOfMembers => @internal.Members.Count;
        // 是否是空的
        public bool IsEmpty => NumberOfMembers == 0;
        // 迭代成员
        public IEnumerable<TPlayer> Members => @internal.Server.IterateMembersOf(this);
        // 小队分数
        public int SquadPoints
        {
            get => @internal.SquadPoints;

            set
            {
                @internal.SquadPoints = value;
                Server.SetSquadPointsOf(@internal.Team, @internal.Name, value);
            }
        }

        private Internal @internal;
        public Squad(Internal @internal)
        {
            this.@internal = @internal;
        }

        public override string ToString()
        {
            return "Squad " + Name;
        }

        // ---- Internal ----
        public class Internal
        {
            public readonly Team Team;
            public readonly Squads Name;
            public int SquadPoints;
            public GameServer<TPlayer> Server;
            public HashSet<TPlayer> Members;

            public Internal(GameServer<TPlayer> server, Team team, Squads squads)
            {
                this.Team = team;
                this.Name = squads;
                this.Server = server;
                this.Members = new HashSet<TPlayer>(8);
            }

            public void Reset()
            {

            }
        }
    }
}
