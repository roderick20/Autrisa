using System.Text.Json;

namespace Autrisa.Helpers
{
    public static class SessionExtensions
    {
        public static void SetSettings(this ISession session, Dictionary<string, string> value)
        {
            string strJson = JsonSerializer.Serialize<Dictionary<string, string>>(value);

            session.SetString("Settings", strJson);
        }

        public static Dictionary<string, string> GetSettings(this ISession session)
        {
            var value = session.GetString("Settings");
            return JsonSerializer.Deserialize<Dictionary<string, string>>(value);
        }
    }
}