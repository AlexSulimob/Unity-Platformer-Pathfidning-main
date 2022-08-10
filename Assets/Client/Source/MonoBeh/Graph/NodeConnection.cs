
namespace Client
{
    public struct NodeConnection 
    {
        public Node node;
        public int cost;
        public NodeConnectionType connectionType;
        public JumpAi jumpAi;
        public NodeConnection(Node node, NodeConnectionType connectionType = NodeConnectionType.Walk, int cost = 1)
        {
            this.node = node;
            this.cost = cost;
            this.connectionType = connectionType;
            this.jumpAi = AIJumpVariants.emptyJump;
        }
        public NodeConnection(Node node, JumpAi jumpAi , NodeConnectionType connectionType = NodeConnectionType.Walk, int cost = 1)
        {
            this.node = node;
            this.cost = cost;
            this.connectionType = connectionType;
            this.jumpAi = jumpAi;
        }
        public NodeConnection(bool isEmpty = true)
        {
            this.node = Graph.emptyNode;
            this.cost = 0;
            this.connectionType = NodeConnectionType.Empty;
            this.jumpAi = AIJumpVariants.emptyJump;
        }
    }
    public enum NodeConnectionType 
    {
        Walk,
        Fall,
        Jump,
        Empty
    }
}

