namespace CommunityServerAPI.Utils
{
    public class TimeUtil
    {
        public static string GetPhaseDifference(long oldtime)
        {
            var dif = GetUtcTimeMs() - oldtime;
            return (dif / 1000 / 60).ToString();
        }

        public static long GetUtcTimeMs()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        public static long GetUtcTime(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }
}