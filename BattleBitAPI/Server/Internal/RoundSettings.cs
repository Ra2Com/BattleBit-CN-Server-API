using BattleBitAPI.Common;

namespace BattleBitAPI.Server
{
    public class RoundSettings<TPlayer> where TPlayer : Player<TPlayer>
    {
        // ---- 构造 ---- 
        private GameServer<TPlayer>.Internal mResources;
        public RoundSettings(GameServer<TPlayer>.Internal resources)
        {
            mResources = resources;
        }

        // ---- 变量 ---- 
        public GameState State
        {
            get => this.mResources._RoundSettings.State;
        }
        public double TeamATickets
        {
            get => this.mResources._RoundSettings.TeamATickets;
            set
            {
                this.mResources._RoundSettings.TeamATickets = value;
                this.mResources.IsDirtyRoundSettings = true;
            }
        }
        public double TeamBTickets
        {
            get => this.mResources._RoundSettings.TeamBTickets;
            set
            {
                this.mResources._RoundSettings.TeamBTickets = value;
                this.mResources.IsDirtyRoundSettings = true;
            }
        }
        public double MaxTickets
        {
            get => this.mResources._RoundSettings.MaxTickets;
            set
            {
                this.mResources._RoundSettings.MaxTickets = value;
                this.mResources.IsDirtyRoundSettings = true;
            }
        }
        public int PlayersToStart
        {
            get => this.mResources._RoundSettings.PlayersToStart;
            set
            {
                this.mResources._RoundSettings.PlayersToStart = value;
                this.mResources.IsDirtyRoundSettings = true;
            }
        }
        public int SecondsLeft
        {
            get => this.mResources._RoundSettings.SecondsLeft;
            set
            {
                this.mResources._RoundSettings.SecondsLeft = value;
                this.mResources.IsDirtyRoundSettings = true;
            }
        }

        // ---- Reset ---- 
        public void Reset()
        {

        }

        // ---- 类 ---- 
        public class mRoundSettings
        {
            public const int Size = 1 + 8 + 8 + 8 + 4 + 4;

            // 游戏状态，默认设置是等待足够玩家开始对局
            public GameState State = GameState.WaitingForPlayers;
            // A 团队阵营当前人口
            public double TeamATickets = 0;
            // B 团队阵营当前人口
            public double TeamBTickets = 0;
            // 阵营最大人口
            public double MaxTickets = 1;
            // 需要多少玩家才能开始对局
            public int PlayersToStart = 16;
            // 对局剩余时间
            public int SecondsLeft = 60;

            public void Write(Common.Serialization.Stream ser)
            {
                ser.Write((byte)this.State);
                ser.Write(this.TeamATickets);
                ser.Write(this.TeamBTickets);
                ser.Write(this.MaxTickets);
                ser.Write(this.PlayersToStart);
                ser.Write(this.SecondsLeft);
            }
            public void Read(Common.Serialization.Stream ser)
            {
                this.State = (GameState)ser.ReadInt8();
                this.TeamATickets = ser.ReadDouble();
                this.TeamBTickets = ser.ReadDouble();
                this.MaxTickets = ser.ReadDouble();
                this.PlayersToStart = ser.ReadInt32();
                this.SecondsLeft = ser.ReadInt32();
            }

            public void Reset()
            {
                this.State = GameState.WaitingForPlayers;
                this.TeamATickets = 0;
                this.TeamBTickets = 0;
                this.MaxTickets = 1;
                this.PlayersToStart = 16;
                this.SecondsLeft = 60;
            }
        }
    }
}
