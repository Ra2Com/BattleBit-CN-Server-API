
namespace BattleBitAPI.Server
{
    public class MapRotation<TPlayer> where TPlayer : Player<TPlayer>
    {
        private GameServer<TPlayer>.Internal mResources;
        public MapRotation(GameServer<TPlayer>.Internal resources)
        {
            mResources = resources;
        }

        public IEnumerable<string> GetMapRotation()
        {
            lock (mResources._MapRotation)
                return new List<string>(mResources._MapRotation);
        }
        // 是否在地图池中
        public bool InRotation(string map)
        {
            map = map.ToUpperInvariant();

            lock (mResources._MapRotation)
                return mResources._MapRotation.Contains(map);
        }
        // 从地图池移除
        public bool RemoveFromRotation(string map)
        {
            map = map.ToUpperInvariant();

            lock (mResources._MapRotation)
                if (!mResources._MapRotation.Remove(map))
                    return false;
            mResources.IsDirtyMapRotation = true;
            return true;
        }
        // 加入地图池
        public bool AddToRotation(string map)
        {
            map = map.ToUpperInvariant();

            lock (mResources._MapRotation)
                if (!mResources._MapRotation.Add(map))
                    return false;
            mResources.IsDirtyMapRotation = true;
            return true;
        }
        // 设置地图池
        public void SetRotation(params string[] maps)
        {
            lock (mResources._MapRotation)
            {
                mResources._MapRotation.Clear();
                foreach (var item in maps)
                    mResources._MapRotation.Add(item);
            }
            mResources.IsDirtyMapRotation = true;
        }
        // 清除地图池
        public void ClearRotation()
        {
            lock (mResources._MapRotation)
            {
                if (mResources._MapRotation.Count == 0)
                    return;

                mResources._MapRotation.Clear();
            }
            mResources.IsDirtyMapRotation = true;
        }

        public void Reset()
        {
        }
    }
}
