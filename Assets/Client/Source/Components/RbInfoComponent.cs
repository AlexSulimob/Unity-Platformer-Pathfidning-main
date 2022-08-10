using UnityEngine;

namespace Client
{
    /// <summary>
    /// utility component there we have informatoin about collision, like there is body collide with others 
    /// </summary>
    public struct RbInfoComponent{
        public bool isBottomContact;
        public bool isTopContact;
        public bool isRightContact;
        public bool isLeftContact;

        public bool isOnSlope;
        public Vector2 slopePerpendicular;

        public bool isLedgeHanging;
    }
}