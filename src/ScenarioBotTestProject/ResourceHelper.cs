using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ScenarioBotTestProject
{
    public static class ResourceHelper
    {
        public static T GetDeserializedResourceValue<T>(string resourceName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                return new JsonSerializer().Deserialize<T>(jsonTextReader);
            }
        }
    }
}