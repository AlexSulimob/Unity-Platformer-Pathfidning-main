using System.Collections;
using System.Collections.Generic;
using Client;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using Unity.Mathematics;
using System;

public class GridGraph : MonoBehaviour
{
    
    public Tilemap tilemap; //tilemap whitch we based to create graph
    GraphSer graphSer;   
    public string graphName = "graph1";

    void Start()
    {
        graphSer = new GraphSer();
        CreateTileGraph();

        SaveAndLoadGraph.Save(graphSer, graphName);

        // var graphTest = SaveAndLoadGraph.Load("graph1");
        // for (int i = 0; i < graphTest.graphContainer.Count()- 1; i++)
        // {
        //     graphTest.graphContainer.
        //     Debug.DrawRay(graphTest.Nodes[i].pos.ToVector2(), Vector3.up * 0.2f, Color.red, 100f); 
        // }
    }

    void CreateTileGraph()
    {
        // int count = 0; 
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            // count +=1;
            graphSer.AddNode( (Vector2Int)position );
        }
        // graphSer.InitGraphContainer();
        // Debug.Log(count);

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var currentNode = graphSer.getNodeAtPos((Vector2Int)position );

            var topNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(0, 1));
            var bottomNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(0, -1));
            var rightNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(1, 0));
            var leftNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(-1, 0));
            
            var topLeftNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(-1, 1));
            var topRightNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(1, 1));
            var bottomLeftNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(-1, -1));
            var bottomRightNeighbour = graphSer.getNodeAtPos((Vector2Int)position +  new Vector2Int(1, -1));

            graphSer.AddConnection(currentNode, new NodeConnection(topLeftNeighbour, cost:2));
            graphSer.AddConnection(currentNode, new NodeConnection(topRightNeighbour, cost:2));
            graphSer.AddConnection(currentNode, new NodeConnection(bottomLeftNeighbour, cost:2));
            graphSer.AddConnection(currentNode, new NodeConnection(bottomRightNeighbour, cost:2));

            graphSer.AddConnection(currentNode, new NodeConnection(topNeighbour));
            graphSer.AddConnection(currentNode, new NodeConnection(bottomNeighbour));
            graphSer.AddConnection(currentNode, new NodeConnection(rightNeighbour));
            graphSer.AddConnection(currentNode, new NodeConnection(leftNeighbour));

        }
    }

    
}
public class int2JsonConverter : JsonConverter<int2> {
    
    public override void WriteJson (JsonWriter writer, int2 value, JsonSerializer serializer) {
        writer.WriteStartObject();
        
        writer.WritePropertyName(nameof(value.x));
        writer.WriteValue(value.x);
        
        writer.WritePropertyName(nameof(value.y));
        writer.WriteValue(value.y);
        
        writer.WriteEndObject();
    }
    
    public override int2 ReadJson (JsonReader reader, Type objectType, int2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        var result = int2.zero;
        
        while (reader.TokenType != JsonToken.EndObject) {
            reader.Read();
            if (reader.TokenType != JsonToken.PropertyName) continue;
    
            var property = reader.Value;
            if (property is nameof(int2.x)) result.x = reader.ReadAsInt32() ?? 0;
            if (property is nameof(int2.y)) result.y = reader.ReadAsInt32() ?? 0;
        }
        
        return result;
    }

}
