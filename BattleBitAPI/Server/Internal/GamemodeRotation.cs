namespace CommunityServerAPI.BattleBitAPI.Server.Internal
{
    public class GamemodeRotation<TPlayer> where TPlayer : Player<TPlayer>
    {
        private GameServer<TPlayer>.Internal mResources;
        public GamemodeRotation(GameServer<TPlayer>.Internal resources)
        {
            mResources = resources;
        }

        public IEnumerable<string> GetGamemodeRotation()
        {
            lock (mResources._GamemodeRotation)
                return new List<string>(mResources._GamemodeRotation);
        }
        // 是否在模式池中
        public bool InRotation(string gamemode)
        {
            lock (mResources._GamemodeRotation)
                return mResources._GamemodeRotation.Contains(gamemode);
        }
        // 移出模式池
        public bool RemoveFromRotation(string gamemode)
        {
            lock (mResources._GamemodeRotation)
                if (!mResources._GamemodeRotation.Remove(gamemode))
                    return false;
            mResources.IsDirtyGamemodeRotation = true;
            return true;
        }
        // 加入模式池
        public bool AddToRotation(string gamemode)
        {
            lock (mResources._GamemodeRotation)
                if (!mResources._GamemodeRotation.Add(gamemode))
                    return false;
            mResources.IsDirtyGamemodeRotation = true;
            return true;
        }
        // 设置模式池
        public void SetRotation(params string[] gamemodes)
        {
            lock (mResources._GamemodeRotation)
            {
                mResources._GamemodeRotation.Clear();
                foreach (var item in gamemodes)
                    mResources._GamemodeRotation.Add(item);
            }
            mResources.IsDirtyGamemodeRotation = true;
        }
        // 清除模式池
        public void ClearRotation()
        {
            lock (mResources._GamemodeRotation)
            {
                if (mResources._GamemodeRotation.Count == 0)
                    return;

                mResources._GamemodeRotation.Clear();
            }
            mResources.IsDirtyGamemodeRotation = true;
        }

        public void Reset()
        {
        }
    }
}
