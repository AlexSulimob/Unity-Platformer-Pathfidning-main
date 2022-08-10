using Unity.Mathematics;

namespace Client
{
    public struct NodePriority
    {
        public int index;
        public int cost;
        public NodePriority(int index, int cost)
        {
            this.index = index;
            this.cost = cost;
        }
    }
    public struct Node 
    {
        public int index;
        public int2 pos;
        public NodeType nodeType;

        public Node(int index, int2 pos, NodeType nodeType = NodeType.Regualar)
        {
            this.index = index;
            this.pos = pos;
            this.nodeType = nodeType;
        }
    }
    public enum NodeType
    {
        Regualar,
        LeftCliff,
        RightCliff,
        OneTile
    }
}
