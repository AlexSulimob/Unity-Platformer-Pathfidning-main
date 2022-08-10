//using Priority_Queue;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Newtonsoft.Json;

namespace Client
{
    public class Graph 
    {
        public NativeList<Node> nodesContainer = new NativeList<Node>(Allocator.Persistent);
        public NativeMultiHashMap<int, NodeConnection> graphContainer;

        public Dictionary<int, Node> Nodes { get; private set; }
        public Dictionary<int, Dictionary<int, NodeConnection>> nodeConnections = new Dictionary<int, Dictionary<int, NodeConnection>>();

        public static readonly Node emptyNode = new Node(-1, Vector2Int.zero.ToInt2());
        public Graph(GraphSer graphSer)
        {
            graphContainer = new NativeMultiHashMap<int, NodeConnection>(nodesContainer.Length , Allocator.Persistent);
            foreach (var node in graphSer.nodes)
            {
                nodesContainer.Add(node);
            }
            for (int i = 0; i < graphSer.graph.Count -1; i++)
            {
                // nodeConnections.Add(i, new Dictionary<int, NodeConnection>());
                for (int x = 0; x < graphSer.graph[i].Count -1; x++)
                {
                    int conIndex = graphSer.graph[i][x].node.index;
                    bool tmp = false;

                    var values = graphContainer.GetValuesForKey(i);
                    foreach (var item in values) {
                        if(item.node.index == conIndex) {
                            tmp = true;
                        }
                    }
                    if(!tmp)
                        graphContainer.Add(i, graphSer.graph[i][x]);       
                }
            }

            //nodes setup
            Nodes = nodesContainer.ToArray().ToDictionary(x=> x.index, x => x);

            //node connecions setup
            foreach (var node in Nodes)
            {
                nodeConnections.Add(node.Key, new Dictionary<int, NodeConnection>());
                var connections = graphContainer.GetValuesForKey(node.Key);
                foreach (var connection in connections)
                {
                    nodeConnections[node.Key].Add(connection.node.index, connection);
                }
            }

        }
        public void DisposeContainer()
        {
            nodesContainer.Dispose();
            graphContainer.Dispose();
            // graphContainerOnInt.Dispose();
        }
        
    }
    
}


