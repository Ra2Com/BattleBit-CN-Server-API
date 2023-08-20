using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using BattleBitAPI.Common;
using BattleBitAPI.Common.Extentions;
using BattleBitAPI.Networking;
using BattleBitAPI.Pooling;
using CommunityServerAPI.BattleBitAPI;

namespace BattleBitAPI.Server
{
    public class GameServer<TPlayer> : System.IDisposable where TPlayer : Player<TPlayer>
    {
        // ---- 全局变量 ---- 
        public ulong ServerHash => mInternal.ServerHash;
        // 服务器哈希
        public bool IsConnected => mInternal.IsConnected;
        // 已连接
        public IPAddress GameIP => mInternal.GameIP;
        // 服务器IP
        public int GamePort => mInternal.GamePort;
        // 服务器端口

        public TcpClient Socket => mInternal.Socket;
        // TCP端口
        public bool IsPasswordProtected => mInternal.IsPasswordProtected;
        // 服务器是否有密码
        public string ServerName => mInternal.ServerName;
        // 服务器名称
        public string Gamemode => mInternal.Gamemode;
        // 游戏模式
        public string Map => mInternal.Map;
        // 地图
        public MapSize MapSize => mInternal.MapSize;
        // 地图大小
        public MapDayNight DayNight => mInternal.DayNight;
        // 地图日夜
        public int CurrentPlayerCount => mInternal.CurrentPlayerCount;
        // 当前在线玩家
        public int InQueuePlayerCount => mInternal.InQueuePlayerCount;
        // 队列中玩家
        public int MaxPlayerCount => mInternal.MaxPlayerCount;
        // 最大玩家
        public string LoadingScreenText => mInternal.LoadingScreenText;
        // 加载页面文本
        public string ServerRulesText => mInternal.ServerRulesText;
        // 服务器规则文本
        public ServerSettings<TPlayer> ServerSettings => mInternal.ServerSettings;
        // 服务器设置
        public MapRotation<TPlayer> MapRotation => mInternal.MapRotation;
        // 地图池设置
        public GamemodeRotation<TPlayer> GamemodeRotation => mInternal.GamemodeRotation;
        // 游戏模式池设置
        public RoundSettings<TPlayer> RoundSettings => mInternal.RoundSettings;
        // 本局设置
        public string TerminationReason => mInternal.TerminationReason;
        // 服务器关闭原因
        public bool ReconnectFlag => mInternal.ReconnectFlag;
        public IEnumerable<Squad<TPlayer>> TeamASquads
        {
            get
            {
                for (int i = 1; i < this.mInternal.TeamASquads.Length; i++)
                    yield return this.mInternal.TeamASquads[i];
            }
        }
        public IEnumerable<Squad<TPlayer>> TeamBSquads
        {
            get
            {
                for (int i = 1; i < this.mInternal.TeamBSquads.Length; i++)
                    yield return this.mInternal.TeamBSquads[i];
            }
        }
        public IEnumerable<Squad<TPlayer>> AllSquads
        {
            get
            {
                for (int i = 1; i < this.mInternal.TeamASquads.Length; i++)
                    yield return this.mInternal.TeamASquads[i];
                for (int i = 1; i < this.mInternal.TeamBSquads.Length; i++)
                    yield return this.mInternal.TeamBSquads[i];
            }
        }

        // ---- 内部变量 ---- 
        private Internal mInternal;

        // ---- Tick ----
        public async Task Tick()
        {
            if (!this.IsConnected)
                return;

            if (this.mInternal.IsDirtyRoomSettings)
            {
                this.mInternal.IsDirtyRoomSettings = false;

                // 发送新的设置
                using (var pck = Common.Serialization.Stream.Get())
                {
                    pck.Write((byte)NetworkCommunication.SetNewRoomSettings);
                    this.mInternal._RoomSettings.Write(pck);
                    WriteToSocket(pck);
                }
            }
            if (this.mInternal.IsDirtyMapRotation)
            {
                this.mInternal.IsDirtyMapRotation = false;
                this.mInternal.mBuilder.Clear();

                this.mInternal.mBuilder.Append("setmaprotation ");
                lock (this.mInternal._MapRotation)
                    foreach (var map in this.mInternal._MapRotation)
                    {
                        this.mInternal.mBuilder.Append(map);
                        this.mInternal.mBuilder.Append(',');
                    }
                this.ExecuteCommand(this.mInternal.mBuilder.ToString());
            }
            if (this.mInternal.IsDirtyGamemodeRotation)
            {
                this.mInternal.IsDirtyGamemodeRotation = false;
                this.mInternal.mBuilder.Clear();

                this.mInternal.mBuilder.Append("setgamemoderotation ");
                lock (this.mInternal._GamemodeRotation)
                {
                    foreach (var gamemode in this.mInternal._GamemodeRotation)
                    {
                        this.mInternal.mBuilder.Append(gamemode);
                        this.mInternal.mBuilder.Append(',');
                    }
                }
                this.ExecuteCommand(this.mInternal.mBuilder.ToString());
            }
            if (this.mInternal.IsDirtyRoundSettings)
            {
                this.mInternal.IsDirtyRoundSettings = false;

                //发送新的本局设置
                using (var pck = Common.Serialization.Stream.Get())
                {
                    pck.Write((byte)NetworkCommunication.SetNewRoundState);
                    this.mInternal._RoundSettings.Write(pck);
                    WriteToSocket(pck);
                }
            }

            //Gather all changes.
            this.mInternal.mChangedModifications.Clear();
            lock (this.mInternal.Players)
            {
                foreach (var steamid in this.mInternal.Players.Keys)
                {
                    var @internal = this.mInternal.mGetInternals(steamid);
                    if (@internal._Modifications.IsDirtyFlag)
                        this.mInternal.mChangedModifications.Enqueue((steamid, @internal._Modifications));
                }
            }

            //Send all changes.
            while (this.mInternal.mChangedModifications.Count > 0)
            {
                (ulong steamID, PlayerModifications<TPlayer>.mPlayerModifications modifications) item = this.mInternal.mChangedModifications.Dequeue();

                item.modifications.IsDirtyFlag = false;

                //Send new settings
                using (var pck = Common.Serialization.Stream.Get())
                {
                    pck.Write((byte)NetworkCommunication.SetPlayerModifications);
                    pck.Write(item.steamID);
                    item.modifications.Write(pck);
                    WriteToSocket(pck);
                }
            }

            try
            {
                //Are we still connected on socket level?
                if (!Socket.Connected)
                {
                    mClose("Connection was terminated.");
                    return;
                }

                //Did user requested to close connection?
                if (this.mInternal.mWantsToCloseConnection)
                {
                    mClose(this.TerminationReason);
                    return;
                }

                var networkStream = Socket.GetStream();

                //Read network packages.
                while (Socket.Available > 0)
                {
                    this.mInternal.mLastPackageReceived = Extentions.TickCount;

                    //Do we know the package size?
                    if (this.mInternal.mReadPackageSize == 0)
                    {
                        const int sizeToRead = 4;
                        int leftSizeToRead = sizeToRead - this.mInternal.mReadStream.WritePosition;

                        int read = await networkStream.ReadAsync(this.mInternal.mReadStream.Buffer, this.mInternal.mReadStream.WritePosition, leftSizeToRead);
                        if (read <= 0)
                            throw new Exception("Connection was terminated.");

                        this.mInternal.mReadStream.WritePosition += read;

                        //Did we receive the package?
                        if (this.mInternal.mReadStream.WritePosition >= 4)
                        {
                            //Read the package size
                            this.mInternal.mReadPackageSize = this.mInternal.mReadStream.ReadUInt32();

                            if (this.mInternal.mReadPackageSize > Const.MaxNetworkPackageSize)
                                throw new Exception("Incoming package was larger than 'Conts.MaxNetworkPackageSize'");

                            this.mInternal.mReadStream.Reset();
                        }
                    }
                    else
                    {
                        int sizeToRead = (int)this.mInternal.mReadPackageSize;
                        int leftSizeToRead = sizeToRead - this.mInternal.mReadStream.WritePosition;

                        int read = await networkStream.ReadAsync(this.mInternal.mReadStream.Buffer, this.mInternal.mReadStream.WritePosition, leftSizeToRead);
                        if (read <= 0)
                            throw new Exception("Connection was terminated.");

                        this.mInternal.mReadStream.WritePosition += read;

                        //Do we have the package?
                        if (this.mInternal.mReadStream.WritePosition >= this.mInternal.mReadPackageSize)
                        {
                            this.mInternal.mReadPackageSize = 0;

                            await this.mInternal.mExecutionFunc(this, this.mInternal, this.mInternal.mReadStream);

                            //Reset
                            this.mInternal.mReadStream.Reset();
                        }
                    }
                }

                //Send the network packages.
                if (this.mInternal.mWriteStream.WritePosition > 0)
                {
                    lock (this.mInternal.mWriteStream)
                    {
                        if (this.mInternal.mWriteStream.WritePosition > 0)
                        {
                            networkStream.WriteAsync(this.mInternal.mWriteStream.Buffer, 0, this.mInternal.mWriteStream.WritePosition);
                            this.mInternal.mWriteStream.WritePosition = 0;

                            this.mInternal.mLastPackageSent = Extentions.TickCount;
                        }
                    }
                }

                //Are we timed out?
                if ((Extentions.TickCount - this.mInternal.mLastPackageReceived) > Const.NetworkTimeout)
                    throw new Exception("Timedout.");

                //Send keep alive if needed
                if ((Extentions.TickCount - this.mInternal.mLastPackageSent) > Const.NetworkKeepAlive)
                {
                    //Send keep alive.
                    await networkStream.WriteAsync(this.mInternal.mKeepAliveBuffer, 0, 4);
                    await networkStream.FlushAsync();
                    this.mInternal.mLastPackageSent = Extentions.TickCount;
                }
            }
            catch (Exception e)
            {
                mClose(e.Message);
            }
        }

        // ---- 团队 ----
        public IEnumerable<TPlayer> AllPlayers
        {
            get
            {
                using (var list = this.mInternal.PlayerPool.Get())
                {
                    //Get A copy of players to our list
                    lock (this.mInternal.Players)
                        list.ListItems.AddRange(this.mInternal.Players.Values);

                    //Iterate our list.
                    for (int i = 0; i < list.ListItems.Count; i++)
                        yield return (TPlayer)list.ListItems[i];
                }
            }
        }
        public IEnumerable<TPlayer> AllTeamAPlayers
        {
            get
            {
                using (var list = this.mInternal.PlayerPool.Get())
                {
                    //Get A copy of players to our list
                    lock (this.mInternal.Players)
                        list.ListItems.AddRange(this.mInternal.Players.Values);

                    //Iterate our list.
                    for (int i = 0; i < list.ListItems.Count; i++)
                    {
                        var item = list.ListItems[i];
                        if (item.Team == Team.TeamA)
                            yield return (TPlayer)item;
                    }
                }
            }
        }
        public IEnumerable<TPlayer> AllTeamBPlayers
        {
            get
            {
                using (var list = this.mInternal.PlayerPool.Get())
                {
                    //Get A copy of players to our list
                    lock (this.mInternal.Players)
                        list.ListItems.AddRange(this.mInternal.Players.Values);

                    //Iterate our list.
                    for (int i = 0; i < list.ListItems.Count; i++)
                    {
                        var item = list.ListItems[i];
                        if (item.Team == Team.TeamB)
                            yield return (TPlayer)item;
                    }
                }
            }
        }
        public IEnumerable<TPlayer> PlayersOf(Team team)
        {
            using (var list = this.mInternal.PlayerPool.Get())
            {
                //Get A copy of players to our list
                lock (this.mInternal.Players)
                    list.ListItems.AddRange(this.mInternal.Players.Values);

                //Iterate our list.
                for (int i = 0; i < list.ListItems.Count; i++)
                {
                    var item = list.ListItems[i];
                    if (item.Team == team)
                        yield return (TPlayer)item;
                }
            }
        }
        public IEnumerable<TPlayer> SearchPlayerByName(string keyword)
        {
            keyword = keyword.ToLower().Replace(" ", "");

            using (var list = this.mInternal.PlayerPool.Get())
            {
                //Get A copy of players to our list
                lock (this.mInternal.Players)
                    list.ListItems.AddRange(this.mInternal.Players.Values);

                //Iterate our list.
                for (int i = 0; i < list.ListItems.Count; i++)
                {
                    var item = list.ListItems[i];
                    if (item.Name.ToLower().Replace(" ", "").Contains(keyword))
                        yield return (TPlayer)item;
                }
            }
        }
        public IEnumerable<TPlayer> SearchPlayerByName(params string[] keywords)
        {
            for (int i = 0; i < keywords.Length; i++)
                keywords[i] = keywords[i].ToLower().Replace(" ", "");

            using (var list = this.mInternal.PlayerPool.Get())
            {
                //Get A copy of players to our list
                lock (this.mInternal.Players)
                    list.ListItems.AddRange(this.mInternal.Players.Values);

                //Iterate our list.
                for (int i = 0; i < list.ListItems.Count; i++)
                {
                    var item = list.ListItems[i];
                    var lowerName = item.Name.ToLower().Replace(" ", "");

                    for (int x = 0; x < keywords.Length; x++)
                    {
                        if (lowerName.Contains(keywords[x]))
                        {
                            yield return (TPlayer)item;
                            break;
                        }
                    }
                }
            }
        }

        public bool TryGetPlayer(ulong steamID, out TPlayer player)
        {
            lock (this.mInternal.Players)
            {
                if (this.mInternal.Players.TryGetValue(steamID, out var _player))
                {
                    player = (TPlayer)_player;
                    return true;
                }
            }

            player = default;
            return false;
        }

        // ---- 虚函数 ---- 
        // 服务器链接成功时
        public virtual async Task OnConnected() 
        {

        }
        // 服务器通信时
        public virtual async Task OnTick() 
        {

        }
        // 服务器重连时
        public virtual async Task OnReconnected() 
        {

        }
        // 服务器离线时
        public virtual async Task OnDisconnected() 
        {

        }
        // 当某个玩家成功连接时
        public virtual async Task OnPlayerConnected(TPlayer player) 
        {

        }
        // 当某个玩家离线时
        public virtual async Task OnPlayerDisconnected(TPlayer player) 
        {

        }
        // 当某个玩家发送聊天信息时
        public virtual async Task<bool> OnPlayerTypedMessage(TPlayer player, ChatChannel channel, string msg) 
        {
            return true;
        }
        // 当某个玩家加入服务器时
        public virtual async Task OnPlayerJoiningToServer(ulong steamID, PlayerJoiningArguments args) 
        {
        }
        // 当储存玩家进度信息时
        public virtual async Task OnSavePlayerStats(ulong steamID, PlayerStats stats) 
        {

        }
        // 当玩家请求更换小队角色时
        public virtual async Task<bool> OnPlayerRequestingToChangeRole(TPlayer player, GameRole requestedRole) 
        {
            return true;
        }
        // 当玩家请求更换游戏内阵营（团队）时
        public virtual async Task<bool> OnPlayerRequestingToChangeTeam(TPlayer player, Team requestedTeam) 
        {
            return true;
        }
        // 当玩家成功更换小队角色时
        public virtual async Task OnPlayerChangedRole(TPlayer player, GameRole role) 
        {

        }
        // 当玩家成功加入小队时
        public virtual async Task OnPlayerJoinedSquad(TPlayer player, Squad<TPlayer> squad) 
        {

        }
        // 当玩家成功离开小队时
        public virtual async Task OnPlayerLeftSquad(TPlayer player, Squad<TPlayer> squad) 
        {

        }
        // 当玩家成功更换游戏内团队阵营时
        public virtual async Task OnPlayerChangeTeam(TPlayer player, Team team) 
        {

        }
        // 当小队分数变化时
        public virtual async Task OnSquadPointsChanged(Squad<TPlayer> squad, int newPoints)
        {

        }
        // 当玩家正在重生时
        public virtual async Task<OnPlayerSpawnArguments?> OnPlayerSpawning(TPlayer player, OnPlayerSpawnArguments request)  
        {
            return request;
        }
        // 当玩家重生成功时
        public virtual async Task OnPlayerSpawned(TPlayer player) 
        {

        }
        // 当玩家死亡时
        public virtual async Task OnPlayerDied(TPlayer player) 
        {

        }
        // 当玩家放弃被救助时
        public virtual async Task OnPlayerGivenUp(TPlayer player) 
        {

        }
        // 当玩家被其它玩家击倒时
        public virtual async Task OnAPlayerDownedAnotherPlayer(OnPlayerKillArguments<TPlayer> args) 
        {

        }
        // 当玩家被其它玩家救助时
        public virtual async Task OnAPlayerRevivedAnotherPlayer(TPlayer from, TPlayer to) 
        {

        }
        // 当玩家被其他人举报时
        public virtual async Task OnPlayerReported(TPlayer from, TPlayer to, ReportReason reason, string additional) 
        {

        }
        // 当本局游戏状态发生变化时
        public virtual async Task OnGameStateChanged(GameState oldState, GameState newState) 
        {

        }
        // 当本局刚开始时
        public virtual async Task OnRoundStarted() 
        {

        }
        // 当本局结束进入结算时
        public virtual async Task OnRoundEnded() 
        {

        }

        // ---- 功能方法 ----
        public void WriteToSocket(Common.Serialization.Stream pck)
        {
            lock (this.mInternal.mWriteStream)
            {
                this.mInternal.mWriteStream.Write((uint)pck.WritePosition);
                this.mInternal.mWriteStream.Write(pck.Buffer, 0, pck.WritePosition);
            }
        }
        public void ExecuteCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
                return;

            int bytesLong = System.Text.Encoding.UTF8.GetByteCount(cmd);
            lock (this.mInternal.mWriteStream)
            {
                this.mInternal.mWriteStream.Write((uint)(1 + 2 + bytesLong));
                this.mInternal.mWriteStream.Write((byte)NetworkCommunication.ExecuteCommand);
                this.mInternal.mWriteStream.Write(cmd);
            }
        }

        // 设置新的服务器密码
        public void SetNewPassword(string newPassword)
        {
            ExecuteCommand("setpass " + newPassword);
        }

        // 设置 Ping 高于多少踢出
        public void SetPingLimit(int newPing)
        {
            ExecuteCommand("setmaxping " + newPing);
        }

        // 发布一条短公告消息在屏幕上方
        public void AnnounceShort(string msg)
        {
            ExecuteCommand("an " + msg);
        }

        // 发布一条长公告在屏幕中间偏上
        public void AnnounceLong(string msg)
        {
            ExecuteCommand("ann " + msg);
        }
        
        // 写一条服务端的 Log
        public void UILogOnServer(string msg, float messageLifetime)
        {
            ExecuteCommand("serverlog " + msg + " " + messageLifetime);
        }

        // 即使不满足条件也强行开始本局游戏 
        public void ForceStartGame()
        {
            ExecuteCommand("forcestart");
        }

         // 即使不满足条件也强行结束本局游戏 
        public void ForceEndGame()
        {
            ExecuteCommand("endgame");
        }

         // 发布聊天栏内容
        public void SayToChat(string msg)
        {
            ExecuteCommand("say " + msg);
        }

        // 夭寿，服务器药丸
        public void StopServer()
        {
            ExecuteCommand("stop");
        }

        // 通知大家服务器药丸
        public void CloseServer()
        {
            ExecuteCommand("notifyend");
        }

        // 踢出所有玩家
        public void KickAllPlayers()
        {
            ExecuteCommand("kick all");
        }

        // 通过 steamID 踢出某个玩家，需要填写原因
        public void Ban(ulong steamID, string reason)
        {
            ExecuteCommand("ban " + steamID + " " + reason);
        }

        // 通过昵称封禁某个玩家，需要填写原因
        public void Ban(Player<TPlayer> player, string reason)
        {
            Ban(player.SteamID, reason);
        }

        // 通过 steamID 踢出某个玩家，需要填写原因
        public void Kick(ulong steamID, string reason)
        {
            ExecuteCommand("kick " + steamID + " " + reason);
        }

        // 通过昵称踢出某个玩家，需要填写原因
        public void Kick(Player<TPlayer> player, string reason)
        {
            Kick(player.SteamID, reason);
        }

        // 通过 steamID 杀死某个玩家
        public void Kill(ulong steamID)
        {
            ExecuteCommand("kill " + steamID);
        }

        // 通过昵称杀死某个玩家
        public void Kill(Player<TPlayer> player)
        {
            Kill(player.SteamID);
        }

        // 通过 steamID 给某个玩家换边
        public void ChangeTeam(ulong steamID)
        {
            ExecuteCommand("changeteam " + steamID);
        }

        // 通过昵称给某个玩家换边
        public void ChangeTeam(Player<TPlayer> player)
        {
            ChangeTeam(player.SteamID);
        }

        // 通过 steamID 给某个玩家指定团队
        public void ChangeTeam(ulong steamID, Team team)
        {
            if (team == Team.TeamA)
                ExecuteCommand("changeteam " + steamID + " a");
            else if (team == Team.TeamB)
                ExecuteCommand("changeteam " + steamID + " b");
        }

        // 通过昵称给某个玩家指定团队
        public void ChangeTeam(Player<TPlayer> player, Team team)
        {
            ChangeTeam(player.SteamID, team);
        }

        // 通过 steamID 给某个玩家踢出小队
        public void KickFromSquad(ulong steamID)
        {
            ExecuteCommand("squadkick " + steamID);
        }

        // 通过昵称给某个玩家踢出小队
        public void KickFromSquad(Player<TPlayer> player)
        {
            KickFromSquad(player.SteamID);
        }

        // 通过 steamID 给某个玩家加入小队
        public void JoinSquad(ulong steamID, Squads targetSquad)
        {
            ExecuteCommand("setsquad " + steamID + " " + ((int)targetSquad));
        }

        // 通过昵称给某个玩家加入小队
        public void JoinSquad(Player<TPlayer> player, Squads targetSquad)
        {
            JoinSquad(player.SteamID, targetSquad);
        }

        // 通过 steamID 解散某个玩家所在小队
        public void DisbandPlayerSquad(ulong steamID)
        {
            ExecuteCommand("squaddisband " + steamID);
        }

        // 通过昵称解散某个玩家所在小队
        public void DisbandPlayerCurrentSquad(Player<TPlayer> player)
        {
            DisbandPlayerSquad(player.SteamID);
        }

        // 通过 steamID 晋升某个玩家为队长
        public void PromoteSquadLeader(ulong steamID)
        {
            ExecuteCommand("squadpromote " + steamID);
        }

        // 通过昵称晋升某个玩家为队长
        public void PromoteSquadLeader(Player<TPlayer> player)
        {
            PromoteSquadLeader(player.SteamID);
        }

        // 通过 steamID 给某个玩家发送警告 Warn 弹窗，需要手动关闭
        public void WarnPlayer(ulong steamID, string msg)
        {
            ExecuteCommand("warn " + steamID + " " + msg);
        }

        // 通过昵称给某个玩家发送警告 Warn 弹窗，需要手动关闭
        public void WarnPlayer(Player<TPlayer> player, string msg)
        {
            WarnPlayer(player.SteamID, msg);
        }

        // 通过 steamID 给某个玩家发送消息 Message 弹窗，需要手动关闭
        public void MessageToPlayer(ulong steamID, string msg)
        {
            ExecuteCommand("msg " + steamID + " " + msg);
        }

        // 通过昵称给某个玩家发送消息 Message 弹窗，需要手动关闭
        public void MessageToPlayer(Player<TPlayer> player, string msg)
        {
            MessageToPlayer(player.SteamID, msg);
        }

        // 通过 steamID 给某个玩家发送消息 Message 弹窗，并带有自动消失秒数
        public void MessageToPlayer(ulong steamID, string msg, float fadeOutTime)
        {
            ExecuteCommand("msgf " + steamID + " " + fadeOutTime + " " + msg);
        }

        // 通过昵称给某个玩家发送消息 Message 弹窗，并带有自动消失秒数
        public void MessageToPlayer(Player<TPlayer> player, string msg, float fadeOutTime)
        {
            MessageToPlayer(player.SteamID, msg, fadeOutTime);
        }

        // 通过 steamID 给某个玩家指定服务器内角色
        public void SetRoleTo(ulong steamID, GameRole role)
        {
            ExecuteCommand("setrole " + steamID + " " + role);
        }

        // 通过昵称给某个玩家指定服务器内角色
        public void SetRoleTo(Player<TPlayer> player, GameRole role)
        {
            SetRoleTo(player.SteamID, role);
        }

        // 通过 Steam64 让玩家重生
        public void SpawnPlayer(ulong steamID, PlayerLoadout loadout, PlayerWearings wearings, Vector3 position, Vector3 lookDirection, PlayerStand stand, float spawnProtection)
        {
            var request = new OnPlayerSpawnArguments()
            {
                Loadout = loadout,
                Wearings = wearings,
                RequestedPoint = PlayerSpawningPosition.Null,
                SpawnPosition = position,
                LookDirection = lookDirection,
                SpawnStand = stand,
                SpawnProtection = spawnProtection,
            };

            // 回调
            using (var response = Common.Serialization.Stream.Get())
            {
                response.Write((byte)NetworkCommunication.SpawnPlayer);
                response.Write(steamID);
                request.Write(response);
                response.Write((ushort)0);

                WriteToSocket(response);
            }
        }

        // 通过昵称让玩家重生
        public void SpawnPlayer(Player<TPlayer> player, PlayerLoadout loadout, PlayerWearings wearings, Vector3 position, Vector3 lookDirection, PlayerStand stand, float spawnProtection)
        {
            SpawnPlayer(player.SteamID, loadout, wearings, position, lookDirection, stand, spawnProtection);
        }

        // 通过 Steam64 设置玩家生命
        public void SetHP(ulong steamID, float newHP)
        {
            ExecuteCommand("sethp " + steamID + " " + newHP);
        }

        // 通过昵称设置玩家生命
        public void SetHP(Player<TPlayer> player, float newHP)
        {
            SetHP(player.SteamID, newHP);
        }
        // 通过 Steam64 给予玩家伤害
        public void GiveDamage(ulong steamID, float damage)
        {
            ExecuteCommand("givedamage " + steamID + " " + damage);
        }
        // 通过昵称给予玩家伤害
        public void GiveDamage(Player<TPlayer> player, float damage)
        {
            GiveDamage(player.SteamID, damage);
        }
        // 通过 Steam64 给玩家回血
        public void Heal(ulong steamID, float heal)
        {
            ExecuteCommand("heal " + steamID + " " + heal);
        }
        // 通过昵称给玩家回血
        public void Heal(Player<TPlayer> player, float heal)
        {
            Heal(player.SteamID, heal);
        }
        // 设置团队阵营中某小队的分数
        public void SetSquadPointsOf(Team team, Squads squad, int points)
        {
            ExecuteCommand("setsquadpoints " + ((int)(team)) + " " + ((int)squad) + " " + points);
        }

        // 通过 Steam64 设置主武器
        public void SetPrimaryWeapon(ulong steamID, WeaponItem item, int extraMagazines, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerWeapon);
                packet.Write(steamID);
                packet.Write((byte)0);//Primary
                item.Write(packet);
                packet.Write((byte)extraMagazines);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }
        
        // 通过昵称设置主武器
        public void SetPrimaryWeapon(Player<TPlayer> player, WeaponItem item, int extraMagazines, bool clear = false)
        {
            SetPrimaryWeapon(player.SteamID, item, extraMagazines, clear);
        }

        // 通过 Steam64 设置手枪
        public void SetSecondaryWeapon(ulong steamID, WeaponItem item, int extraMagazines, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerWeapon);
                packet.Write(steamID);
                packet.Write((byte)1);//Secondary
                item.Write(packet);
                packet.Write((byte)extraMagazines);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }

        // 通过昵称设置手枪
        public void SetSecondaryWeapon(Player<TPlayer> player, WeaponItem item, int extraMagazines, bool clear = false)
        {
            SetSecondaryWeapon(player.SteamID, item, extraMagazines, clear);
        }

        // 通过 Steam64 设置绷带
        public void SetFirstAid(ulong steamID, string tool, int extra, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerGadget);
                packet.Write(steamID);
                packet.Write((byte)2);//first aid
                packet.Write(tool);
                packet.Write((byte)extra);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }

        // 通过昵称设置绷带
        public void SetFirstAid(Player<TPlayer> player, string tool, int extra, bool clear = false)
        {
            SetFirstAid(player.SteamID, tool, extra, clear);
        }

        // 通过 Steam64 设置主道具
        public void SetLightGadget(ulong steamID, string tool, int extra, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerGadget);
                packet.Write(steamID);
                packet.Write((byte)3);//Tool A
                packet.Write(tool);
                packet.Write((byte)extra);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }

        // 通过昵称设置主道具
        public void SetLightGadget(Player<TPlayer> player, string tool, int extra, bool clear = false)
        {
            SetLightGadget(player.SteamID, tool, extra, clear);
        }

        // 通过 Steam64 设置次要道具
        public void SetHeavyGadget(ulong steamID, string tool, int extra, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerGadget);
                packet.Write(steamID);
                packet.Write((byte)4);//Tool A
                packet.Write(tool);
                packet.Write((byte)extra);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }

        // 通过昵称设置次要道具
        public void SetHeavyGadget(Player<TPlayer> player, string tool, int extra, bool clear = false)
        {
            SetHeavyGadget(player.SteamID, tool, extra, clear);
        }

        // 通过 Steam64 设置投掷物
        public void SetThrowable(ulong steamID, string tool, int extra, bool clear = false)
        {
            using (var packet = Common.Serialization.Stream.Get())
            {
                packet.Write((byte)NetworkCommunication.SetPlayerGadget);
                packet.Write(steamID);
                packet.Write((byte)5);//Tool A
                packet.Write(tool);
                packet.Write((byte)extra);
                packet.Write(clear);

                WriteToSocket(packet);
            }
        }

        // 通过昵称设置投掷物
        public void SetThrowable(Player<TPlayer> player, string tool, int extra, bool clear = false)
        {
            SetThrowable(player.SteamID, tool, extra, clear);
        }

        // ---- Squads ---- 
        public IEnumerable<TPlayer> IterateMembersOf(Squad<TPlayer> squad)
        {
            using (var list = this.mInternal.PlayerPool.Get())
            {
                var rsquad = this.mInternal.GetSquadInternal(squad);

                //Get A copy of players to our list
                lock (rsquad.Members)
                    list.ListItems.AddRange(rsquad.Members);

                //Iterate our list.
                for (int i = 0; i < list.ListItems.Count; i++)
                    yield return (TPlayer)list.ListItems[i];
            }
        }
        public Squad<TPlayer> GetSquad(Team team, Squads name)
        {
            if (team == Team.TeamA)
                return this.mInternal.TeamASquads[(int)name];
            if (team == Team.TeamB)
                return this.mInternal.TeamBSquads[(int)name];
            return null;
        }

        // ---- 关闭链接 ----
        public void CloseConnection(string additionInfo = "")
        {
            if (string.IsNullOrWhiteSpace(additionInfo))
                this.mInternal.TerminationReason = additionInfo;
            else
                this.mInternal.TerminationReason = "User requested to terminate the connection";
            this.mInternal.mWantsToCloseConnection = true;
        }
        private void mClose(string reason)
        {
            if (this.IsConnected)
            {
                this.mInternal.TerminationReason = reason;
                this.mInternal.IsConnected = false;
            }
        }

        // ---- Disposing ----
        public void Dispose()
        {
            if (this.mInternal.Socket != null)
            {
                this.mInternal.Socket.SafeClose();
                this.mInternal.Socket = null;
            }
        }

        // ---- Overrides ----
        public override string ToString()
        {
            return
                this.GameIP + ":" + this.GamePort + " - " +
                this.ServerName;
        }

        // ---- Static ----
        public static void SetInstance(GameServer<TPlayer> server, Internal @internal)
        {
            server.mInternal = @internal;
        }

        // ---- Internal ----
        public class Internal
        {
            // ---- Variables ---- 
            public ulong ServerHash;
            public bool IsConnected;
            public IPAddress GameIP;
            public int GamePort;
            public TcpClient Socket;
            public Func<GameServer<TPlayer>, Internal, Common.Serialization.Stream, Task> mExecutionFunc;
            public Func<ulong, Player<TPlayer>.Internal> mGetInternals;
            public bool IsPasswordProtected;
            public string ServerName;
            public string Gamemode;
            public string Map;
            public MapSize MapSize;
            public MapDayNight DayNight;
            public int CurrentPlayerCount;
            public int InQueuePlayerCount;
            public int MaxPlayerCount;
            public string LoadingScreenText;
            public string ServerRulesText;
            public ServerSettings<TPlayer> ServerSettings;
            public MapRotation<TPlayer> MapRotation;
            public GamemodeRotation<TPlayer> GamemodeRotation;
            public RoundSettings<TPlayer> RoundSettings;
            public string TerminationReason;
            public bool ReconnectFlag;
            public Squad<TPlayer>.Internal[] TeamASquadInternals;
            public Squad<TPlayer>.Internal[] TeamBSquadInternals;
            public Squad<TPlayer>[] TeamASquads;
            public Squad<TPlayer>[] TeamBSquads;
            public ItemPooling<Player<TPlayer>> PlayerPool;

            // ---- Private Variables ---- 
            public byte[] mKeepAliveBuffer;
            public Common.Serialization.Stream mWriteStream;
            public Common.Serialization.Stream mReadStream;
            public uint mReadPackageSize;
            public long mLastPackageReceived;
            public long mLastPackageSent;
            public bool mWantsToCloseConnection;
            public StringBuilder mBuilder;
            public Queue<(ulong steamID, PlayerModifications<TPlayer>.mPlayerModifications)> mChangedModifications;

            public Internal(GameServer<TPlayer> server)
            {
                this.TerminationReason = string.Empty;
                this.mWriteStream = new Common.Serialization.Stream()
                {
                    Buffer = new byte[Const.MaxNetworkPackageSize],
                    InPool = false,
                    ReadPosition = 0,
                    WritePosition = 0,
                };
                this.mReadStream = new Common.Serialization.Stream()
                {
                    Buffer = new byte[Const.MaxNetworkPackageSize],
                    InPool = false,
                    ReadPosition = 0,
                    WritePosition = 0,
                };
                this.mKeepAliveBuffer = new byte[4]
                {
                0,0,0,0,
                };
                this.mLastPackageReceived = Extentions.TickCount;
                this.mLastPackageSent = Extentions.TickCount;
                this.mBuilder = new StringBuilder(4096);

                this.ServerSettings = new ServerSettings<TPlayer>(this);
                this.MapRotation = new MapRotation<TPlayer>(this);
                this.GamemodeRotation = new GamemodeRotation<TPlayer>(this);
                this.RoundSettings = new RoundSettings<TPlayer>(this);
                this.mChangedModifications = new Queue<(ulong steamID, PlayerModifications<TPlayer>.mPlayerModifications)>(254);

                this.TeamASquadInternals = new Squad<TPlayer>.Internal[]
                {
                    null,
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Alpha),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Bravo),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Charlie),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Delta ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Echo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Foxtrot ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Golf ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Hotel ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.India),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Juliett ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Kilo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Lima ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Mike ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.November),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Oscar ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Papa ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Quebec),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Romeo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Sierra),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Tango ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Uniform ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Whiskey ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Xray ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Yankee ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Zulu ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Ash ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Baker ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Cast ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Diver),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Eagle),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Fisher),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.George),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Hanover),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Ice ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Jake),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.King),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Lash),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Mule),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Neptune ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Ostend),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Page ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Quail ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Raft ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Scout ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Tare ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Unit ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.William ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Xaintrie ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Yoke ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Zebra ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Ace ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Beer ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Cast2 ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Duff ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Edward ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Freddy),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Gustav),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Henry ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Ivar ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Jazz ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Key ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Lincoln ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Mary ),
                    new Squad<TPlayer>.Internal(server,Team.TeamA, Squads.Nora ),
                };
                this.TeamBSquadInternals = new Squad<TPlayer>.Internal[]
                {
                    null,
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Alpha),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Bravo),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Charlie),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Delta ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Echo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Foxtrot ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Golf ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Hotel ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.India),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Juliett ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Kilo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Lima ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Mike ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.November),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Oscar ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Papa ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Quebec),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Romeo ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Sierra),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Tango ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Uniform ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Whiskey ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Xray ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Yankee ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Zulu ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Ash ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Baker ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Cast ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Diver),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Eagle),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Fisher),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.George),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Hanover),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Ice ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Jake),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.King),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Lash),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Mule),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Neptune ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Ostend),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Page ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Quail ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Raft ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Scout ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Tare ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Unit ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.William ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Xaintrie ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Yoke ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Zebra ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Ace ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Beer ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Cast2 ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Duff ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Edward ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Freddy),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Gustav),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Henry ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Ivar ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Jazz ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Key ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Lincoln ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Mary ),
                    new Squad<TPlayer>.Internal(server,Team.TeamB, Squads.Nora ),
                };

                this.TeamASquads = new Squad<TPlayer>[]
                {
                    null,
                    new Squad<TPlayer>(this.TeamASquadInternals[01]),
                    new Squad<TPlayer>(this.TeamASquadInternals[02]),
                    new Squad<TPlayer>(this.TeamASquadInternals[03]),
                    new Squad<TPlayer>(this.TeamASquadInternals[04]),
                    new Squad<TPlayer>(this.TeamASquadInternals[05]),
                    new Squad<TPlayer>(this.TeamASquadInternals[06]),
                    new Squad<TPlayer>(this.TeamASquadInternals[07]),
                    new Squad<TPlayer>(this.TeamASquadInternals[08]),
                    new Squad<TPlayer>(this.TeamASquadInternals[09]),
                    new Squad<TPlayer>(this.TeamASquadInternals[10]),
                    new Squad<TPlayer>(this.TeamASquadInternals[11]),
                    new Squad<TPlayer>(this.TeamASquadInternals[12]),
                    new Squad<TPlayer>(this.TeamASquadInternals[13]),
                    new Squad<TPlayer>(this.TeamASquadInternals[14]),
                    new Squad<TPlayer>(this.TeamASquadInternals[15]),
                    new Squad<TPlayer>(this.TeamASquadInternals[16]),
                    new Squad<TPlayer>(this.TeamASquadInternals[17]),
                    new Squad<TPlayer>(this.TeamASquadInternals[18]),
                    new Squad<TPlayer>(this.TeamASquadInternals[19]),
                    new Squad<TPlayer>(this.TeamASquadInternals[20]),
                    new Squad<TPlayer>(this.TeamASquadInternals[21]),
                    new Squad<TPlayer>(this.TeamASquadInternals[22]),
                    new Squad<TPlayer>(this.TeamASquadInternals[23]),
                    new Squad<TPlayer>(this.TeamASquadInternals[24]),
                    new Squad<TPlayer>(this.TeamASquadInternals[25]),
                    new Squad<TPlayer>(this.TeamASquadInternals[26]),
                    new Squad<TPlayer>(this.TeamASquadInternals[27]),
                    new Squad<TPlayer>(this.TeamASquadInternals[28]),
                    new Squad<TPlayer>(this.TeamASquadInternals[29]),
                    new Squad<TPlayer>(this.TeamASquadInternals[30]),
                    new Squad<TPlayer>(this.TeamASquadInternals[31]),
                    new Squad<TPlayer>(this.TeamASquadInternals[32]),
                    new Squad<TPlayer>(this.TeamASquadInternals[33]),
                    new Squad<TPlayer>(this.TeamASquadInternals[34]),
                    new Squad<TPlayer>(this.TeamASquadInternals[35]),
                    new Squad<TPlayer>(this.TeamASquadInternals[36]),
                    new Squad<TPlayer>(this.TeamASquadInternals[37]),
                    new Squad<TPlayer>(this.TeamASquadInternals[38]),
                    new Squad<TPlayer>(this.TeamASquadInternals[39]),
                    new Squad<TPlayer>(this.TeamASquadInternals[40]),
                    new Squad<TPlayer>(this.TeamASquadInternals[41]),
                    new Squad<TPlayer>(this.TeamASquadInternals[42]),
                    new Squad<TPlayer>(this.TeamASquadInternals[43]),
                    new Squad<TPlayer>(this.TeamASquadInternals[44]),
                    new Squad<TPlayer>(this.TeamASquadInternals[45]),
                    new Squad<TPlayer>(this.TeamASquadInternals[46]),
                    new Squad<TPlayer>(this.TeamASquadInternals[47]),
                    new Squad<TPlayer>(this.TeamASquadInternals[48]),
                    new Squad<TPlayer>(this.TeamASquadInternals[49]),
                    new Squad<TPlayer>(this.TeamASquadInternals[50]),
                    new Squad<TPlayer>(this.TeamASquadInternals[51]),
                    new Squad<TPlayer>(this.TeamASquadInternals[52]),
                    new Squad<TPlayer>(this.TeamASquadInternals[53]),
                    new Squad<TPlayer>(this.TeamASquadInternals[54]),
                    new Squad<TPlayer>(this.TeamASquadInternals[55]),
                    new Squad<TPlayer>(this.TeamASquadInternals[56]),
                    new Squad<TPlayer>(this.TeamASquadInternals[57]),
                    new Squad<TPlayer>(this.TeamASquadInternals[58]),
                    new Squad<TPlayer>(this.TeamASquadInternals[59]),
                    new Squad<TPlayer>(this.TeamASquadInternals[60]),
                    new Squad<TPlayer>(this.TeamASquadInternals[61]),
                    new Squad<TPlayer>(this.TeamASquadInternals[62]),
                    new Squad<TPlayer>(this.TeamASquadInternals[63]),
                    new Squad<TPlayer>(this.TeamASquadInternals[64]),
                };
                this.TeamBSquads = new Squad<TPlayer>[]
                {
                    null,
                    new Squad<TPlayer>(this.TeamBSquadInternals[01]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[02]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[03]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[04]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[05]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[06]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[07]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[08]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[09]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[10]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[11]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[12]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[13]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[14]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[15]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[16]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[17]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[18]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[19]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[20]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[21]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[22]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[23]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[24]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[25]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[26]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[27]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[28]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[29]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[30]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[31]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[32]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[33]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[34]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[35]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[36]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[37]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[38]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[39]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[40]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[41]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[42]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[43]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[44]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[45]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[46]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[47]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[48]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[49]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[50]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[51]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[52]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[53]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[54]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[55]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[56]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[57]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[58]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[59]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[60]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[61]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[62]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[63]),
                    new Squad<TPlayer>(this.TeamBSquadInternals[64]),
                };
                this.PlayerPool = new ItemPooling<Player<TPlayer>>(254);
            }

            // ---- 服务器中的玩家字典 ---- 
            public Dictionary<ulong, Player<TPlayer>> Players = new Dictionary<ulong, Player<TPlayer>>(254);

            // ---- 房间设置 ---- 
            public ServerSettings<TPlayer>.mRoomSettings _RoomSettings = new ServerSettings<TPlayer>.mRoomSettings();
            public bool IsDirtyRoomSettings;

            // ---- 对局设置 ---- 
            public RoundSettings<TPlayer>.mRoundSettings _RoundSettings = new RoundSettings<TPlayer>.mRoundSettings();
            public bool IsDirtyRoundSettings;

            // ---- 地图池 ---- 
            public HashSet<string> _MapRotation = new HashSet<string>(8);
            public bool IsDirtyMapRotation = false;

            // ---- 游戏模式池 ---- 
            public HashSet<string> _GamemodeRotation = new HashSet<string>(8);
            public bool IsDirtyGamemodeRotation = false;

            // ---- Public Functions ---- 
            public void Set (
                Func<GameServer<TPlayer>, Internal, Common.Serialization.Stream, Task> func,
                Func<ulong, Player<TPlayer>.Internal> internalGetFunc,
                TcpClient socket,
                IPAddress iP,
                int port,
                bool isPasswordProtected,
                string serverName,
                string gamemode,
                string map,
                MapSize mapSize,
                MapDayNight dayNight,
                int currentPlayers,
                int inQueuePlayers,
                int maxPlayers,
                string loadingScreenText,
                string serverRulesText
                )
            {
                this.ServerHash = ((ulong)port << 32) | (ulong)iP.ToUInt();
                this.IsConnected = true;
                this.GameIP = iP;
                this.GamePort = port;
                this.Socket = socket;
                this.mExecutionFunc = func;
                this.mGetInternals = internalGetFunc;
                this.IsPasswordProtected = isPasswordProtected;
                this.ServerName = serverName;
                this.Gamemode = gamemode;
                this.Map = map;
                this.MapSize = mapSize;
                this.DayNight = dayNight;
                this.CurrentPlayerCount = currentPlayers;
                this.InQueuePlayerCount = inQueuePlayers;
                this.MaxPlayerCount = maxPlayers;
                this.LoadingScreenText = loadingScreenText;
                this.ServerRulesText = serverRulesText;

                this.ServerSettings.Reset();
                this._RoomSettings.Reset();
                this.IsDirtyRoomSettings = false;

                this.MapRotation.Reset();
                this._MapRotation.Clear();
                this.IsDirtyMapRotation = false;

                this.GamemodeRotation.Reset();
                this._GamemodeRotation.Clear();
                this.IsDirtyGamemodeRotation = false;

                this.RoundSettings.Reset();
                this._RoundSettings.Reset();
                this.IsDirtyRoundSettings = false;

                this.TerminationReason = string.Empty;
                this.ReconnectFlag = false;

                this.mWriteStream.Reset();
                this.mReadStream.Reset();
                this.mReadPackageSize = 0;
                this.mLastPackageReceived = Extentions.TickCount;
                this.mLastPackageSent = Extentions.TickCount;
                this.mWantsToCloseConnection = false;
                this.mBuilder.Clear();
                this.mChangedModifications.Clear();
            }

            public void AddPlayer(Player<TPlayer> player)
            {
                lock (Players)
                {
                    Players.Remove(player.SteamID);
                    Players.Add(player.SteamID, player);
                }
            }
            public void RemovePlayer<TPlayer>(TPlayer player) where TPlayer : Player<TPlayer>
            {
                lock (Players)
                    Players.Remove(player.SteamID);
            }
            public bool TryGetPlayer(ulong steamID, out Player<TPlayer> result)
            {
                lock (Players)
                    return Players.TryGetValue(steamID, out result);
            }
            public Squad<TPlayer>.Internal GetSquadInternal(Team team, Squads squad)
            {
                if (team == Team.TeamA)
                    return this.TeamASquadInternals[(int)squad];
                if (team == Team.TeamB)
                    return this.TeamBSquadInternals[(int)squad];
                return null;
            }
            public Squad<TPlayer>.Internal GetSquadInternal(Squad<TPlayer> squad)
            {
                return GetSquadInternal(squad.Team, squad.Name);
            }
        }
    }
}
