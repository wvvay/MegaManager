using Newtonsoft.Json;

namespace MegaManager.Utilities
{
    public static class Helpers
    {
        // Расширение для установки объекта в Session
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Расширение для получения объекта из Session
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
