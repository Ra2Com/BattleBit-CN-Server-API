using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityServerAPI.Tools
{
    public class TimeUtil
    {
        public static string GetPhaseDifference(long oldtime)
        {
            var dif = GetUtcTimeMs() - oldtime;
            return (dif / 1000 / 60).ToString() + "分钟";
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
