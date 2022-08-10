using UnityEngine;

namespace Client
{
    /// <summary>
    /// Component with transofrm witch represent grount position of object. without this, pathfinding wont be work
    /// </summary>
    public struct AiGroundComponent {
        public Transform groundPos;
    }
}