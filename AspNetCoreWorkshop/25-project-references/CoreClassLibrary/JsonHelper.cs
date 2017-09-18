using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreClassLibrary
{
    public static class JsonHelper
    {
        public static JObject Parse(string content)
        {
            return (JObject)JsonConvert.DeserializeObject(content);
        }
    }
}
