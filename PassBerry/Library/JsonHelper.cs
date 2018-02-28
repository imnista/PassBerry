namespace PassBerry.Library
{
    using Newtonsoft.Json;

    internal class JsonHelper
    {
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}