using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Client;

public class PlatformerGraph : MonoBehaviour
{
    // Start is called before the first frame update
    public PhysSim physSim;
    public Tilemap tilemap;
    GraphSer graphSer;   
    Dictionary<Node, int> PlatformIds= new Dictionary<Node, int>(); 
    public AIJumpVariants aiJumpVariants;
    public string saveGraphName = "platformerGraph1";
    public bool debug = true;
    void Start()
    {
        graphSer = new GraphSer();
        Create();
        SaveAndLoadGraph.Save(graphSer, saveGraphName);
    }

    void Create()
    {
        //adding point nodes 
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            // tilemap.GetTile(pos).
            bool hasTile = !tilemap.HasTile(position) && tilemap.HasTile(position - new Vector3Int(0,1,0));

            bool isLeft = tilemap.HasTile(position + new Vector3Int(-1, 0, 0)) || !tilemap.HasTile(position + new Vector3Int(-1,-1, 0));
            bool isRight = tilemap.HasTile(position + new Vector3Int(1, 0, 0)) || !tilemap.HasTile(position + new Vector3Int(1,-1, 0));

            if (hasTile && isRight && isLeft)
            {
                graphSer.AddNode(new Vector2Int(position.x, position.y), NodeType.OneTile);        
            }
            else if (hasTile && isLeft)
            {
                graphSer.AddNode(new Vector2Int(position.x, position.y), NodeType.LeftCliff);        
            }
            else if (hasTile && isRight)
            {
                graphSer.AddNode(new Vector2Int(position.x, position.y), NodeType.RightCliff);        
            }
            else if (hasTile && !isRight && !isLeft)
            {
                graphSer.AddNode(new Vector2Int(position.x, position.y), NodeType.Regualar);        
                // Debug.Log(position);
            }
        }

        // node platform ids setup
        // int platformId = 0;
        // foreach (var item in graphSer.nodes)
        // {
        //     if (item.nodeType == NodeType.LeftCliff) 
        //     {
        //         platformId++;
        //         var posNode = item.pos; 
        //         int i = 0;
        //         bool isRightNodeReached = false;
                
        //         while (!isRightNodeReached)
        //         {
        //             var node = graphSer.getNodeAtPos(new Vector2Int(posNode.x + i, posNode.y));
        //             PlatformIds.Add(node, platformId);                               
        //             i++;
        //             if (node.nodeType == NodeType.RightCliff)
        //             {
        //                 isRightNodeReached = true;
        //             }
        //         }
        //     }   
        //     if (item.nodeType == NodeType.OneTile)
        //     {
        //         platformId++;
        //         PlatformIds.Add(item, platformId);
        //     }
        // }
        
        // adding connections 
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {

            bool hasTile = !tilemap.HasTile(position) && tilemap.HasTile(position - new Vector3Int(0,1, 0));
            bool isLeft = tilemap.HasTile(position + new Vector3Int(-1, 0, 0)) || !tilemap.HasTile(position + new Vector3Int(-1,-1, 0));
            bool isRight = tilemap.HasTile(position + new Vector3Int(1, 0, 0)) || !tilemap.HasTile(position + new Vector3Int(1,-1, 0));

            //connection in between nodes
            if (hasTile && !isRight && !isLeft)
            {
                var node = graphSer.getNodeAtPos((Vector2Int)position);

                var rightNode = graphSer.getNodeAtPos(new Vector2Int(position.x + 1, position.y));
                var leftNode = graphSer.getNodeAtPos(new Vector2Int(position.x - 1, position.y));

                graphSer.AddConnection(node, new NodeConnection(rightNode));
                graphSer.AddConnection(node, new NodeConnection(leftNode));
                graphSer.AddConnection(rightNode, new NodeConnection(node));
                graphSer.AddConnection(leftNode, new NodeConnection(node));
            }

            //one step connection from left to right clif of platform
            if (hasTile && isLeft && !isRight)
            {
                var node = graphSer.getNodeAtPos((Vector2Int)position);
                var rightNode = graphSer.getNodeAtPos(new Vector2Int(position.x + 1, position.y ));
                // Debug.Log("it works");

                if (rightNode.nodeType == NodeType.RightCliff)
                {
                    graphSer.AddConnection(node, new NodeConnection(rightNode));
                    graphSer.AddConnection(rightNode, new NodeConnection(node));
            }
            }

            //fall links 
            //left fall links
            if (hasTile && isLeft)
            {
                //if nones blocks from left
                if (!tilemap.HasTile(position + new Vector3Int(-1, 0, 0)))
                {
                    bool ground = tilemap.HasTile(position + new Vector3Int(-1,-1, 0));
                    int i = -1; 
                    int howMuch = 0;
                    while (!ground)
                    {
                        i-=1; 
                        ground = tilemap.HasTile(position + new Vector3Int(-1, i, 0));
                        howMuch = i;
                    }
                    var leftNode = graphSer.getNodeAtPos((Vector2Int)position);
                    var downNode = graphSer.getNodeAtPos(new Vector2Int(position.x - 1, position.y + howMuch + 1));
                    
                    // graphSer.AddConnection(new NodeConnection(dow))
                    graphSer.AddConnection(leftNode, new NodeConnection(downNode, NodeConnectionType.Fall));
                    // leftNode.m_Connections.Add(new NodeConnection(downNode, ConnectionsTypes.Fall));
                }
            }

            //right fall links
            if (hasTile && isRight)
            {
                //if nones blocks from left
                if (!tilemap.HasTile(position + new Vector3Int(1, 0, 0)))
                {
                    bool ground = tilemap.HasTile(position + new Vector3Int(1,-1, 0));
                    int i = -1; 
                    int howMuch =0;
                    while (!ground)
                    {
                        i-=1; 
                        ground = tilemap.HasTile(position + new Vector3Int(1, i, 0));
                        howMuch = i;
                    }
                    var rightnode = graphSer.getNodeAtPos((Vector2Int)position);
                    var downNode = graphSer.getNodeAtPos(new Vector2Int(position.x + 1, position.y + howMuch + 1));
                    
                    graphSer.AddConnection(rightnode, new NodeConnection(downNode, NodeConnectionType.Fall));
                }
            }
            
            //jump links
            Vector2 landingPoint;
            if (hasTile)
            {
                foreach (var item in aiJumpVariants.aiJumpList) 
                {
                    landingPoint = Vector2.zero;
                    bool goodJump = physSim.SimulateJump(
                        (Vector3)position,
                        item,
                        ref landingPoint);

                    if (goodJump)
                    {
                        var startJumpNode = graphSer.getNearestNodeAtPos(((Vector2Int)position));
                        var landingJumpNode = graphSer.getNearestNodeAtPos(landingPoint); 
                        if (startJumpNode.index != landingJumpNode.index)
                        {
                            graphSer.AddConnection(startJumpNode, new NodeConnection(landingJumpNode,item, NodeConnectionType.Jump));
                        }
                        goodJump = false;
                    }    
                }
            }


            if( debug)
            {
                foreach (var item in graphSer.graph)
                {
                    foreach (var con in item.Value)
                    {
                        Color conColor = Color.black;
                        switch (con.connectionType)
                        {
                            case NodeConnectionType.Walk:conColor = Color.blue; break;    
                            case NodeConnectionType.Fall: conColor = Color.red;break;    
                            case NodeConnectionType.Jump: conColor = Color.cyan;break;    
                        }
                        // Color conColor = con.connectionType == NodeConnectionType.Walk ? Color.cyan : Color.magenta;
                        Debug.DrawLine(graphSer.nodes[item.Key].pos.ToVector2(), (Vector2)con.node.pos.ToVector2(), conColor, 1000f);
                    }
                }
            }

        }
    }
}
