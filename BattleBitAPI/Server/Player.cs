using CommunityServerAPI.BattleBitAPI.Common.Data;
using CommunityServerAPI.BattleBitAPI.Common.Enums;
using CommunityServerAPI.BattleBitAPI.Server.Internal;
using System.Net;
using System.Numerics;

namespace CommunityServerAPI.BattleBitAPI.Server
{
    public class Player<TPlayer> where TPlayer : Player<TPlayer>
    {
        private Internal mInternal;

        // ---- 变量 ----
        public ulong SteamID => mInternal.SteamID;
        // 玩家的 Steam64
        public string Name => mInternal.Name;
        // 玩家的昵称
        public IPAddress IP => mInternal.IP;
        // 玩家的IP地址
        public GameServer<TPlayer> GameServer => mInternal.GameServer;
        // 玩家在游戏内玩的角色
        public GameRole Role
        {
            get => mInternal.Role;
            set
            {
                if (value == mInternal.Role)
                    return;
                SetNewRole(value);
            }
        }
        // 玩家在服务器内的团队阵营
        public Team Team
        {
            get => mInternal.Team;
            set
            {
                if (mInternal.Team != value)
                    ChangeTeam(value);
            }
        }
        // 玩家在服务器内的小队名
        public Squads SquadName
        {
            get => mInternal.SquadName;
            set
            {
                if (value == mInternal.SquadName)
                    return;
                if (value == Squads.NoSquad)
                    KickFromSquad();
                else
                    JoinSquad(value);
            }
        }
        // 玩家团队阵营和小队
        public Squad<TPlayer> Squad
        {
            get => GameServer.GetSquad(mInternal.Team, mInternal.SquadName);
            set
            {
                if (value == Squad)
                    return;

                if (value == null)
                    KickFromSquad();
                else
                {
                    if (value.Team != this.Team)
                        ChangeTeam(value.Team);
                    JoinSquad(value.Name);
                }
            }
        }
        // 玩家是否在小队中
        public bool InSquad => mInternal.SquadName != Squads.NoSquad;
        // 玩家的延迟 MS
        public int PingMs => mInternal.PingMs;
        // 玩家当前链接编号
        public long CurrentSessionID => mInternal.SessionID;
        // 玩家是否已链接
        public bool IsConnected => mInternal.SessionID != 0;

        // 玩家的 HP值
        public float HP
       
        {
            get => mInternal.HP;
            set
            {
                if (mInternal.HP > 0)
                {
                    float v = value;
                    if (v <= 0)
                        v = 0.1f;
                    else if (v > 100f)
                        v = 100f;

                    SetHP(v);
                }
            }
        }
        public bool IsAlive => mInternal.HP >= 0f;
        // 玩家是否还活着
        public bool IsUp => mInternal.HP > 0f;
        // 玩家 HP是否大于0
        public bool IsDown => mInternal.HP == 0f;
        // 玩家是否被击倒
        public bool IsDead => mInternal.HP == -1f;
        // 玩家是否已死亡

        public Vector3 Position
        // 玩家的坐标
        {
            get => mInternal.Position;
            set => Teleport(value);
        }
        public PlayerStand StandingState => mInternal.Standing;
        // 玩家的站立状态
        public LeaningSide LeaningState => mInternal.Leaning;
        // 玩家的歪头状态
        public LoadoutIndex CurrentLoadoutIndex => mInternal.CurrentLoadoutIndex;
        // 玩家的武器装备配置数据
        public bool InVehicle => mInternal.InVehicle;
        // 玩家是否在载具中
        public bool IsBleeding => mInternal.IsBleeding;
        // 玩家是否在流血
        public PlayerLoadout CurrentLoadout => mInternal.CurrentLoadout;
        // 玩家当前的武器装备配置
        public PlayerWearings CurrentWearings => mInternal.CurrentWearings;
        // 玩家当前的角色穿着配置
        public PlayerModifications<TPlayer> Modifications => mInternal.Modifications;
        // 玩家的数值修改调整

        // ---- 虚函数 ----

        // 玩家实例化时
        public virtual void OnCreated()
        {

        }

        // 玩家已连接时
        public virtual async Task OnConnected()
        {

        }
        // 玩家已重生时
        public virtual async Task OnSpawned()
        {

        }
        // 玩家倒地时
        public virtual async Task OnDowned()
        {

        }
        // 玩家放弃被救助时
        public virtual async Task OnGivenUp()
        {

        }
        // 玩家被其他玩家救起时
        public virtual async Task OnRevivedByAnotherPlayer()
        {

        }
        // 玩家救起其他玩家时
        public virtual async Task OnRevivedAnotherPlayer()
        {

        }
        // 玩家死亡时
        public virtual async Task OnDied()
        {

        }
        // 玩家更换阵营团队时
        public virtual async Task OnChangedTeam()
        {

        }
        // 玩家成功更换队伍角色时
        public virtual async Task OnChangedRole(GameRole newRole)
        {

        }
        // 玩家加入小队时
        public virtual async Task OnJoinedSquad(Squad<TPlayer> newSquad)
        {

        }
        // 玩家离开小队时
        public virtual async Task OnLeftSquad(Squad<TPlayer> oldSquad)
        {

        }
        // 玩家断开连接时
        public virtual async Task OnDisconnected()
        {

        }
        // 玩家的连接编号状态改变时
        public virtual async Task OnSessionChanged(long oldSessionID, long newSessionID)
        {

        }

        // ---- 功能方法 ----
        // 踢出游戏，理由
        public void Kick(string reason = "")
        {
            if (IsConnected)
                GameServer.Kick(this, reason);
        }
        // 封禁，理由
        public void Ban(string reason = "")
        {
            GameServer.Ban(this, reason);
        }
        // 立刻杀死
        public void Kill()
        {
            if (IsConnected)
                GameServer.Kill(this);
        }
        // 更换到对立团队阵营
        public void ChangeTeam()
        {
            if (IsConnected)
                GameServer.ChangeTeam(this);
        }
        // 更换到指定团队阵营
        public void ChangeTeam(Team team)
        {
            if (IsConnected)
                GameServer.ChangeTeam(this, team);
        }
        // 踢出当前小队
        public void KickFromSquad()
        {
            if (IsConnected)
                GameServer.KickFromSquad(this);
        }
        // 加入指定小队
        public void JoinSquad(Squads targetSquad)
        {
            if (IsConnected)
                GameServer.JoinSquad(this, targetSquad);
        }
        // 解散所在小队
        public void DisbandTheSquad()
        {
            if (IsConnected)
                GameServer.DisbandPlayerCurrentSquad(this);
        }
        // 晋升为小队长
        public void PromoteToSquadLeader()
        {
            if (IsConnected)
                GameServer.PromoteSquadLeader(this);
        }
        // 发送警告信息
        public void WarnPlayer(string msg)
        {
            if (IsConnected)
                GameServer.WarnPlayer(this, msg);
        }
        // 发送普通信息
        public void Message(string msg)
        {
            if (IsConnected)
                GameServer.MessageToPlayer(this, msg);
        }
        // 发送聊天栏信息
        public void SayToChat(string msg)
        {
            if (IsConnected)
                GameServer.SayToChat(msg, this);
        }

        // 发送普通信息,一定时间后关闭
        public void Message(string msg, float fadeoutTime)
        {
            if (IsConnected)
                GameServer.MessageToPlayer(this, msg, fadeoutTime);
        }
        // 设置新的游戏内角色
        public void SetNewRole(GameRole role)
        {
            if (IsConnected)
                GameServer.SetRoleTo(this, role);
        }
        // 传送到坐标
        public void Teleport(Vector3 target)
        {

        }
        // 复活玩家
        public void SpawnPlayer(PlayerLoadout loadout, PlayerWearings wearings, Vector3 position, Vector3 lookDirection, PlayerStand stand, float spawnProtection)
        {
            if (IsConnected)
                GameServer.SpawnPlayer(this, loadout, wearings, position, lookDirection, stand, spawnProtection);
        }
        // 设置 HP
        public void SetHP(float newHP)
        {
            if (IsConnected)
                GameServer.SetHP(this, newHP);
        }
        // 给予伤害
        public void GiveDamage(float damage)
        {
            if (IsConnected)
                GameServer.GiveDamage(this, damage);
        }
        // 治疗玩家
        public void Heal(float hp)
        {
            if (IsConnected)
                GameServer.Heal(this, hp);
        }
        // 设置主要武器
        public void SetPrimaryWeapon(WeaponItem item, int extraMagazines, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetPrimaryWeapon(this, item, extraMagazines, clear);
        }
        // 设置次要武器
        public void SetSecondaryWeapon(WeaponItem item, int extraMagazines, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetSecondaryWeapon(this, item, extraMagazines, clear);
        }
        // 设置绷带
        public void SetFirstAidGadget(string item, int extra, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetFirstAid(this, item, extra, clear);
        }
        // 设置轻型道具
        public void SetLightGadget(string item, int extra, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetLightGadget(this, item, extra, clear);
        }
        // 设置重型道具
        public void SetHeavyGadget(string item, int extra, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetHeavyGadget(this, item, extra, clear);
        }
        // 设置投掷物
        public void SetThrowable(string item, int extra, bool clear = false)
        {
            if (IsConnected)
                GameServer.SetThrowable(this, item, extra, clear);
        }

        // ---- Static ----
        internal static void SetInstance(TPlayer player, Player<TPlayer>.Internal @internal)
        {
            player.mInternal = @internal;
        }

        // ---- Overrides ----
        public override string ToString()
        {
            return Name + " (" + SteamID + ")";
        }

        // ---- Internal ----
        public class Internal
        {
            public ulong SteamID;
            public string Name;
            public IPAddress IP;
            public GameServer<TPlayer> GameServer;
            public GameRole Role;
            public Team Team;
            public Squads SquadName;
            public int PingMs = 999;
            public long PreviousSessionID = 0;
            public long SessionID = 0;

            public bool IsAlive;
            public float HP;
            public Vector3 Position;
            public PlayerStand Standing;
            public LeaningSide Leaning;
            public LoadoutIndex CurrentLoadoutIndex;
            public bool InVehicle;
            public bool IsBleeding;
            public PlayerLoadout CurrentLoadout;
            public PlayerWearings CurrentWearings;

            public PlayerModifications<TPlayer>.mPlayerModifications _Modifications;
            public PlayerModifications<TPlayer> Modifications;

            public Internal()
            {
                this._Modifications = new PlayerModifications<TPlayer>.mPlayerModifications();
                this.Modifications = new PlayerModifications<TPlayer>(this);
            }

            public void OnDie()
            {
                IsAlive = false;
                HP = -1f;
                Position = default;
                Standing = PlayerStand.Standing;
                Leaning = LeaningSide.None;
                CurrentLoadoutIndex = LoadoutIndex.Primary;
                InVehicle = false;
                IsBleeding = false;
                CurrentLoadout = new PlayerLoadout();
                CurrentWearings = new PlayerWearings();
            }
        }
    }
}
