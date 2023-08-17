namespace BattleBitAPI.Server
{
    public class PlayerModifications<TPlayer> where TPlayer : Player<TPlayer>
    {
        // ---- 构建 ---- 
        private Player<TPlayer>.Internal @internal;
        public PlayerModifications(Player<TPlayer>.Internal @internal)
        {
            this.@internal = @internal;
        }

        // ---- 变量 ---- 
        public float RunningSpeedMultiplier
        {
            get => @internal._Modifications.RunningSpeedMultiplier;
            set
            {
                if (@internal._Modifications.RunningSpeedMultiplier == value)
                    return;
                @internal._Modifications.RunningSpeedMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float ReceiveDamageMultiplier
        {
            get => @internal._Modifications.ReceiveDamageMultiplier;
            set
            {
                if (@internal._Modifications.ReceiveDamageMultiplier == value)
                    return;
                @internal._Modifications.ReceiveDamageMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float GiveDamageMultiplier
        {
            get => @internal._Modifications.GiveDamageMultiplier;
            set
            {
                if (@internal._Modifications.GiveDamageMultiplier == value)
                    return;
                @internal._Modifications.GiveDamageMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float JumpHeightMultiplier
        {
            get => @internal._Modifications.JumpHeightMultiplier;
            set
            {
                if (@internal._Modifications.JumpHeightMultiplier == value)
                    return;
                @internal._Modifications.JumpHeightMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float FallDamageMultiplier
        {
            get => @internal._Modifications.FallDamageMultiplier;
            set
            {
                if (@internal._Modifications.FallDamageMultiplier == value)
                    return;
                @internal._Modifications.FallDamageMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float ReloadSpeedMultiplier
        {
            get => @internal._Modifications.ReloadSpeedMultiplier;
            set
            {
                if (@internal._Modifications.ReloadSpeedMultiplier == value)
                    return;
                @internal._Modifications.ReloadSpeedMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool CanUseNightVision
        {
            get => @internal._Modifications.CanUseNightVision;
            set
            {
                if (@internal._Modifications.CanUseNightVision == value)
                    return;
                @internal._Modifications.CanUseNightVision = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float DownTimeGiveUpTime
        {
            get => @internal._Modifications.DownTimeGiveUpTime;
            set
            {
                if (@internal._Modifications.DownTimeGiveUpTime == value)
                    return;
                @internal._Modifications.DownTimeGiveUpTime = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool AirStrafe
        {
            get => @internal._Modifications.AirStrafe;
            set
            {
                if (@internal._Modifications.AirStrafe == value)
                    return;
                @internal._Modifications.AirStrafe = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool CanDeploy
        {
            get => @internal._Modifications.CanDeploy;
            set
            {
                if (@internal._Modifications.CanDeploy == value)
                    return;
                @internal._Modifications.CanDeploy = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool CanSpectate
        {
            get => @internal._Modifications.CanSpectate;
            set
            {
                if (@internal._Modifications.CanSpectate == value)
                    return;
                @internal._Modifications.CanSpectate = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool IsTextChatMuted
        {
            get => @internal._Modifications.IsTextChatMuted;
            set
            {
                if (@internal._Modifications.IsTextChatMuted == value)
                    return;
                @internal._Modifications.IsTextChatMuted = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool IsVoiceChatMuted
        {
            get => @internal._Modifications.IsVoiceChatMuted;
            set
            {
                if (@internal._Modifications.IsVoiceChatMuted == value)
                    return;
                @internal._Modifications.IsVoiceChatMuted = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float RespawnTime
        {
            get => @internal._Modifications.RespawnTime;
            set
            {
                if (@internal._Modifications.RespawnTime == value)
                    return;
                @internal._Modifications.RespawnTime = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool CanSuicide
        {
            get => @internal._Modifications.CanSuicide;
            set
            {
                if (@internal._Modifications.CanSuicide == value)
                    return;
                @internal._Modifications.CanSuicide = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float MinimumDamageToStartBleeding
        {
            get => @internal._Modifications.MinDamageToStartBleeding;
            set
            {
                if (@internal._Modifications.MinDamageToStartBleeding == value)
                    return;
                @internal._Modifications.MinDamageToStartBleeding = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float MinimumHpToStartBleeding
        {
            get => @internal._Modifications.MinHpToStartBleeding;
            set
            {
                if (@internal._Modifications.MinHpToStartBleeding == value)
                    return;
                @internal._Modifications.MinHpToStartBleeding = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float HpPerBandage
        {
            get => @internal._Modifications.HPperBandage;
            set
            {
                if (value >= 100f)
                    value = 100f;
                else if (value < 0)
                    value = 0f;

                if (@internal._Modifications.HPperBandage == value)
                    return;
                @internal._Modifications.HPperBandage = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool StaminaEnabled
        {
            get => @internal._Modifications.StaminaEnabled;
            set
            {
                if (@internal._Modifications.StaminaEnabled == value)
                    return;
                @internal._Modifications.StaminaEnabled = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool HitMarkersEnabled
        {
            get => @internal._Modifications.HitMarkersEnabled;
            set
            {
                if (@internal._Modifications.HitMarkersEnabled == value)
                    return;
                @internal._Modifications.HitMarkersEnabled = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool FriendlyHUDEnabled
        {
            get => @internal._Modifications.FriendlyHUDEnabled;
            set
            {
                if (@internal._Modifications.FriendlyHUDEnabled == value)
                    return;
                @internal._Modifications.FriendlyHUDEnabled = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public float CaptureFlagSpeedMultiplier
        {
            get => @internal._Modifications.CaptureFlagSpeedMultiplier;
            set
            {
                if (@internal._Modifications.CaptureFlagSpeedMultiplier == value)
                    return;
                @internal._Modifications.CaptureFlagSpeedMultiplier = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool PointLogHudEnabled
        {
            get => @internal._Modifications.PointLogHudEnabled;
            set
            {
                if (@internal._Modifications.PointLogHudEnabled == value)
                    return;
                @internal._Modifications.PointLogHudEnabled = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }
        public bool KillFeed
        {
            get => @internal._Modifications.KillFeed;
            set
            {
                if (@internal._Modifications.KillFeed == value)
                    return;
                @internal._Modifications.KillFeed = value;
                @internal._Modifications.IsDirtyFlag = true;
            }
        }

        // 快速关闭流血
        public void DisableBleeding()
        {
            this.MinimumDamageToStartBleeding = 100f;
            this.MinimumHpToStartBleeding = 0f;
        }

        // 快速开启流血
        public void EnableBleeding(float minimumHP = 40f, float minimumDamage = 10f)
        {
            this.MinimumDamageToStartBleeding = minimumDamage;
            this.MinimumHpToStartBleeding = minimumHP;
        }

        // ---- 类 ---- 
        public class mPlayerModifications
        {
            // 移动速度加成倍数
            public float RunningSpeedMultiplier = 1f;
            // 受到伤害加成倍数
            public float ReceiveDamageMultiplier = 1f;
            // 输出伤害加成倍数
            public float GiveDamageMultiplier = 1f;
            // 跳跃高度加成倍数
            public float JumpHeightMultiplier = 1f;
            // 跌落伤害加成倍数
            public float FallDamageMultiplier = 1f;
            // 换弹速度加成倍数
            public float ReloadSpeedMultiplier = 1f;
            // 是否可使用夜视仪
            public bool CanUseNightVision = true;
            // 倒地后等待救援初始时间
            public float DownTimeGiveUpTime = 60f;
            // 跳跃转向不丢失速度
            public bool AirStrafe = true;
            // 可以手动部署
            public bool CanDeploy = true;
            // 可以切换到观战
            public bool CanSpectate = true;
            // 聊天禁言
            public bool IsTextChatMuted = false;
            // 语音禁言
            public bool IsVoiceChatMuted = false;
            // 重生等待时间
            public float RespawnTime = 10f;
            // 可以自杀
            public bool CanSuicide = true;
            // 最小受到多少伤害开始流血
            public float MinDamageToStartBleeding = 10f;
            // 血量低于多少开始流血
            public float MinHpToStartBleeding = 40f;
            // 每个绷带能治疗血量
            public float HPperBandage = 40f;
            // 是否开启体力槽
            public bool StaminaEnabled = false;
            // 是否允许使用击中指示器
            public bool HitMarkersEnabled = true;
            // 屏幕上是否显示友军
            public bool FriendlyHUDEnabled = true;
            // 夺旗模式移动速度加成
            public float CaptureFlagSpeedMultiplier = 1f;
            // 不确定 —— 被击中指示器是否开启
            public bool PointLogHudEnabled = true;
            // 击杀通知
            public bool KillFeed = false;
            // 是否是纯净设置
            public bool IsDirtyFlag = false;
            public void Write(BattleBitAPI.Common.Serialization.Stream ser)
            {
                ser.Write(this.RunningSpeedMultiplier);
                ser.Write(this.ReceiveDamageMultiplier);
                ser.Write(this.GiveDamageMultiplier);
                ser.Write(this.JumpHeightMultiplier);
                ser.Write(this.FallDamageMultiplier);
                ser.Write(this.ReloadSpeedMultiplier);
                ser.Write(this.CanUseNightVision);
                ser.Write(this.DownTimeGiveUpTime);
                ser.Write(this.AirStrafe);
                ser.Write(this.CanDeploy);
                ser.Write(this.CanSpectate);
                ser.Write(this.IsTextChatMuted);
                ser.Write(this.IsVoiceChatMuted);
                ser.Write(this.RespawnTime);
                ser.Write(this.CanSuicide);

                ser.Write(this.MinDamageToStartBleeding);
                ser.Write(this.MinHpToStartBleeding);
                ser.Write(this.HPperBandage);
                ser.Write(this.StaminaEnabled);
                ser.Write(this.HitMarkersEnabled);
                ser.Write(this.FriendlyHUDEnabled);
                ser.Write(this.CaptureFlagSpeedMultiplier);
                ser.Write(this.PointLogHudEnabled);
                ser.Write(this.KillFeed);
            }
            public void Read(BattleBitAPI.Common.Serialization.Stream ser)
            {
                this.RunningSpeedMultiplier = ser.ReadFloat();
                if (this.RunningSpeedMultiplier <= 0f)
                    this.RunningSpeedMultiplier = 0.01f;

                this.ReceiveDamageMultiplier = ser.ReadFloat();
                this.GiveDamageMultiplier = ser.ReadFloat();
                this.JumpHeightMultiplier = ser.ReadFloat();
                this.FallDamageMultiplier = ser.ReadFloat();
                this.ReloadSpeedMultiplier = ser.ReadFloat();
                this.CanUseNightVision = ser.ReadBool();
                this.DownTimeGiveUpTime = ser.ReadFloat();
                this.AirStrafe = ser.ReadBool();
                this.CanDeploy = ser.ReadBool();
                this.CanSpectate = ser.ReadBool();
                this.IsTextChatMuted = ser.ReadBool();
                this.IsVoiceChatMuted = ser.ReadBool();
                this.RespawnTime = ser.ReadFloat();
                this.CanSuicide = ser.ReadBool();

                this.MinDamageToStartBleeding = ser.ReadFloat();
                this.MinHpToStartBleeding = ser.ReadFloat();
                this.HPperBandage = ser.ReadFloat();
                this.StaminaEnabled = ser.ReadBool();
                this.HitMarkersEnabled = ser.ReadBool();
                this.FriendlyHUDEnabled = ser.ReadBool();
                this.CaptureFlagSpeedMultiplier = ser.ReadFloat();
                this.PointLogHudEnabled = ser.ReadBool();
                this.KillFeed = ser.ReadBool();
            }
            public void Reset()
            {
                this.RunningSpeedMultiplier = 1f;
                this.ReceiveDamageMultiplier = 1f;
                this.GiveDamageMultiplier = 1f;
                this.JumpHeightMultiplier = 1f;
                this.FallDamageMultiplier = 1f;
                this.ReloadSpeedMultiplier = 1f;
                this.CanUseNightVision = true;
                this.DownTimeGiveUpTime = 60f;
                this.AirStrafe = true;
                this.CanDeploy = true;
                this.CanSpectate = true;
                this.IsTextChatMuted = false;
                this.IsVoiceChatMuted = false;
                this.RespawnTime = 10f;
                this.CanSuicide = true;
                this.MinDamageToStartBleeding = 10f;
                this.MinHpToStartBleeding = 40f;
                this.HPperBandage = 40f;
                this.StaminaEnabled = false;
                this.HitMarkersEnabled = true;
                this.FriendlyHUDEnabled = true;
                this.CaptureFlagSpeedMultiplier = 1f;
                this.PointLogHudEnabled = true;
                this.KillFeed = false;
            }
        }
    }
}
