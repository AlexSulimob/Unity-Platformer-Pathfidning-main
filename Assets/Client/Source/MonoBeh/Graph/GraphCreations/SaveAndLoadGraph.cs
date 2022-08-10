using Client;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveAndLoadGraph
{
    public static void Save(GraphSer graphSer, string graphName)
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new int2JsonConverter());

        // using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + graphName +".json"))
        using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Client/Resources/GraphsJson/" + graphName +".json"))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, graphSer);
        }
    }
    public static Graph Load(TextAsset jsonAsset)
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new int2JsonConverter());
        // Debug.Log(Application.persistentDataPath);
        using (StringReader sr = new StringReader(jsonAsset.text))
        using (JsonTextReader reader = new JsonTextReader(sr))
        {
            // string readText = await reader.ReadAsync()
            var graphSer = serializer.Deserialize<GraphSer>(reader);
            return new Graph(graphSer);
        }
    }

}
