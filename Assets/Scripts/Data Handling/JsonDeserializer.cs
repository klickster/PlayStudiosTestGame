using UnityEngine;
using Newtonsoft.Json;

public class JsonDeserializer <T>
{
    public T DeserializeJson(string jsonPath)
    {
        if (System.IO.File.Exists(jsonPath))
        {
            var jsonString = System.IO.File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        else
        {
            Debug.LogError($"No File exists at path: {jsonPath}");
            return default(T);
        }
    }
}
