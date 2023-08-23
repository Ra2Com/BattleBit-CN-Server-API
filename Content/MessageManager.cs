using Newtonsoft.Json;

namespace CommunityServerAPI.Content
{
    internal static class MessageOfTheDayManager
    {
        public static MotdJson motdJson = new MotdJson();

        public static void Init()
        {
            try
            {
                string motdPath = $"{Environment.CurrentDirectory}\\Config\\MessageOfTheDay.json";
                string content = File.ReadAllText(motdPath);
                motdJson = JsonConvert.DeserializeObject<MotdJson>(content);
            }
            catch (Exception ee)
            {
                Console.WriteLine("解析 MessageOfTheDay 配置出错，请检查" + ee.Message); // Error when JSON file is wrong
                return;
            }
        }
        
        public static string GetMOTD(string motd)
        {
            return motd switch
            {
                "JoinMethodQun" => motdJson.JoinMethodQun,
                "LongAnnounce" => motdJson.LongAnnounce,
                "RegularAnnounce" => motdJson.RegularAnnounce,
                "WelcomeMsg" => motdJson.WelcomeMsg,
                _ => "ERROR - 未找到对应的值",
            };
        }
    }
    public class MotdJson
    {
        public string JoinMethodQun { get; set; }
        public string LongAnnounce { get; set; }
        public string RegularAnnounce { get; set; }
        public string WelcomeMsg { get; set; }
    }


}
