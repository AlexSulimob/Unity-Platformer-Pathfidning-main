//using Priority_Queue;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Client
{
    [System.Serializable]
    public class GraphSer
    {
        public List<Node> nodes = new List<Node>();
        public Dictionary<int, List<NodeConnection>> graph = new Dictionary<int, List<NodeConnection>>();
        public Node getNodeAtPos(Vector2Int pos)
        {
            
            foreach (var node in nodes)
            {
                if ( node.pos.ToVector2().Equals(pos) )
                {
                    return node;
                }
            }
            return Graph.emptyNode;
        }
        public Node getNearestNodeAtPos(Vector2 pos)
        {
            float distance = float.MaxValue;
            Node returnValue = Graph.emptyNode; 
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var nodeDistance = math.distance(pos, nodes[i].pos);
                if (nodeDistance < distance && nodes[i].index != -1)
                {
                    returnValue = nodes[i];
                    distance = nodeDistance;
                }
            }
            return returnValue;
        }
        public void AddNode(Vector2Int pos, NodeType nodeType = NodeType.Regualar)
        {
            var node = new Node(nodes.Count(), pos.ToInt2(), nodeType);
            nodes.Add(node);
            graph.Add(node.index, new List<NodeConnection>());
        }
        public void AddConnection(Node FromNode, NodeConnection toNodeCon)
        {
            if (toNodeCon.node.index != -1 && FromNode.index != -1)
            {
                var cost = (int)(toNodeCon.node.pos.ToVector2() - FromNode.pos.ToVector2()).magnitude;
                toNodeCon.cost = cost;
                graph[FromNode.index].Add(toNodeCon);
                // graphContainerOnInt.Add(FromNode.index, toNodeCon.node.index);
            }
        }
        public bool isNodeHasConnection(Node node, Node endNode)
        {
            foreach (var connection in graph[node.index])
            {
                if( connection.node.index == endNode.index)
                {
                    return true;
                }        
            }
            return false;
        }
    }
    
}


