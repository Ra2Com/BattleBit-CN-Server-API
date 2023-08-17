namespace BattleBitAPI.Server
{
    public class ServerSettings<TPlayer> where TPlayer : Player<TPlayer>
    {
        // ---- 构造 ---- 
        private GameServer<TPlayer>.Internal mResources;
        public ServerSettings(GameServer<TPlayer>.Internal resources)
        {
            mResources = resources;
        }

        // ---- 变量 ---- 
        public float DamageMultiplier
        {
            get => mResources._RoomSettings.DamageMultiplier;
            set
            {
                if (mResources._RoomSettings.DamageMultiplier == value)
                    return;
                mResources._RoomSettings.DamageMultiplier = value;
                mResources.IsDirtyRoomSettings = true;
            }
        }
        public bool FriendlyFireEnabled
        {
            get => mResources._RoomSettings.FriendlyFireEnabled;
            set
            {
                if (mResources._RoomSettings.FriendlyFireEnabled == value)
                    return;
                mResources._RoomSettings.FriendlyFireEnabled = value;
                mResources.IsDirtyRoomSettings = true;
            }
        }
        public bool OnlyWinnerTeamCanVote
        {
            get => mResources._RoomSettings.OnlyWinnerTeamCanVote;
            set
            {
                if (mResources._RoomSettings.OnlyWinnerTeamCanVote == value)
                    return;
                mResources._RoomSettings.OnlyWinnerTeamCanVote = value;
                mResources.IsDirtyRoomSettings = true;
            }
        }
        public bool PlayerCollision
        {
            get => mResources._RoomSettings.PlayerCollision;
            set
            {
                if (mResources._RoomSettings.PlayerCollision == value)
                    return;
                mResources._RoomSettings.PlayerCollision = value;
                mResources.IsDirtyRoomSettings = true;
            }
        }

        // ---- Reset ---- 
        public void Reset()
        {

        }

        // ---- 类型 ---- 
        // 房间设置
        public class mRoomSettings
        {
            // 伤害增幅倍数
            public float DamageMultiplier = 1.0f;
            // 是否友伤
            public bool FriendlyFireEnabled = false;
            // 是否隐藏对局结算地图投票
            public bool HideMapVotes = true;
            // 是否只有胜利团队可以投票
            public bool OnlyWinnerTeamCanVote = false;
            // 是否玩家有碰撞体积
            public bool PlayerCollision = false;
            // 每小队医疗限制
            public byte MedicLimitPerSquad = 8;
            // 每小队工程限制
            public byte EngineerLimitPerSquad = 8;
            // 每小队支援限制
            public byte SupportLimitPerSquad = 8;
            // 每小队侦查限制
            public byte ReconLimitPerSquad = 8;

            public void Write(Common.Serialization.Stream ser)
            {
                ser.Write(this.DamageMultiplier);
                ser.Write(this.FriendlyFireEnabled);
                ser.Write(this.HideMapVotes);
                ser.Write(this.OnlyWinnerTeamCanVote);
                ser.Write(this.PlayerCollision);

                ser.Write(this.MedicLimitPerSquad);
                ser.Write(this.EngineerLimitPerSquad);
                ser.Write(this.SupportLimitPerSquad);
                ser.Write(this.ReconLimitPerSquad);
            }
            public void Read(Common.Serialization.Stream ser)
            {
                this.DamageMultiplier = ser.ReadFloat();
                this.FriendlyFireEnabled = ser.ReadBool();
                this.HideMapVotes = ser.ReadBool();
                this.OnlyWinnerTeamCanVote = ser.ReadBool();
                this.PlayerCollision=ser.ReadBool();

                this.MedicLimitPerSquad = ser.ReadInt8();
                this.EngineerLimitPerSquad = ser.ReadInt8();
                this.SupportLimitPerSquad = ser.ReadInt8();
                this.ReconLimitPerSquad = ser.ReadInt8();
            }
            public void Reset()
            {
                this.DamageMultiplier = 1.0f;
                this.FriendlyFireEnabled = false;
                this.HideMapVotes = true;
                this.OnlyWinnerTeamCanVote = false;
                this.PlayerCollision = false;

                this.MedicLimitPerSquad = 8;
                this.EngineerLimitPerSquad = 8;
                this.SupportLimitPerSquad = 8;
                this.ReconLimitPerSquad = 8;
            }
        }
    }
}
