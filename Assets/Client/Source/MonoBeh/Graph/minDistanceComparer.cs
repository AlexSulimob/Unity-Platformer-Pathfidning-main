using System.Collections.Generic;
using Client;

namespace Client
{
    // public struct minDistanceComparer : IComparer<Node>
    // {
    //     public int Compare(Node x, Node y)
    //     {
    //         return x.mDistance.CompareTo(y.mDistance);
    //     }
    // }
    // public struct maxDistanceComparer : IComparer<Node>
    // {
    //     public int Compare(Node x, Node y)
    //     {
    //         return y.mDistance.CompareTo(x.mDistance);
    //     }
    // }
}
    public struct minDistanceComparer : IComparer<NodePriority>
    {
        public int Compare(NodePriority x, NodePriority y)
        {
            return x.cost.CompareTo(y.cost);
        }
    }
    public struct maxDistanceComparer : IComparer<NodePriority>
    {
        public int Compare(NodePriority x, NodePriority y)
        {
            return y.cost.CompareTo(x.cost);
        }
    }
