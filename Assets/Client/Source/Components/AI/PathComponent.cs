namespace Client
{
    /// <summary>
    /// Path component, there is an array of node's index. 
    /// </summary>
    public struct PathComponent {
        public Unity.Collections.NativeArray<int> path;
        public bool pathUpdated;
        // public bool isModified;
    }
}