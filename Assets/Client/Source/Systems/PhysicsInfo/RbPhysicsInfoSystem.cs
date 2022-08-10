using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class RbPhysicsInfoSystem : IEcsRunSystem
    {
        public bool IsBetween(float testValue, float bound1, float bound2)
        {
            if (bound1 > bound2)
                return testValue >= bound2 && testValue <= bound1;
            return testValue >= bound1 && testValue <= bound2;
        }

        struct RbContacts
        {
            public ContactPoint2D[] contactPoints;
        }
        public void Run(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var plSettings = systems.GetShared<SharedData>().playerSettings;
            var filter = world.Filter<RbComponent>().End ();//change filter in future it can broke othere states

            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var rbPool = world.GetPool<RbComponent>();
            var rbContactsPool = world.GetPool<RbContacts>();

            foreach (var entity in filter)
            {
                ref var rb = ref rbPool.Get(entity);

                if (!rbContactsPool.Has(entity))
                    rbContactsPool.Add(entity);

                if (!rbInfoPool.Has(entity))
                    rbInfoPool.Add(entity);

                ref var rbInfo = ref rbInfoPool.Get(entity);
                ref var rbContacts = ref rbContactsPool.Get(entity);

                //init contacts if we didnt do this
                if (rbContacts.contactPoints == null)
                    rbContacts.contactPoints = new ContactPoint2D[10];

                int hits = rb.rb.GetContacts(rbContacts.contactPoints); 
                rbInfo.isBottomContact = rbInfo.isTopContact = rbInfo.isLeftContact = rbInfo.isRightContact = false;
                rbInfo.isOnSlope = false;
                rbInfo.isLedgeHanging = false;
                for (int i = 0; i < hits; i++)
                {
                    var normal = rbContacts.contactPoints[i].normal;
                    // Debug.DrawRay(rbContacts.contactPoints[i].point, rbContacts.contactPoints[i].normal, Color.red, 1f);
                    // Debug.Log(rbContacts.contactPoints[i].normal);

                    if (normal.y > 0.1f)
                    {
                        rbInfo.isBottomContact = true; 
                        //slope info setting up
                        if (normal.y != 1)
                        {
                            rbInfo.slopePerpendicular = Vector2.Perpendicular(normal);
                            rbInfo.isOnSlope = true;
                            // var perp = Vector2.Perpendicular(normal);
                            // Debug.DrawRay(rbContacts.contactPoints[i].point, perp, Color.red, 1f);
                        }
                    }
                    if(normal.y < -0.1f)
                    {
                        rbInfo.isTopContact = true; 
                    }
                    if (normal.x > 0.1f)
                    {
                        rbInfo.isLeftContact = true; 

                        //its all about ledge climbing trigger
                        /*
                        var playerCollider = rb.rb.GetComponent<BoxCollider2D>();//fix in future pls
                        var playerTopLeftPoint = new Vector2(playerCollider.bounds.min.x, playerCollider.bounds.max.y);

                        Vector2 otherTopRightPoint = new Vector2(rbContacts.contactPoints[i].collider.bounds.max.x,
                            rbContacts.contactPoints[i].collider.bounds.max.y);

                        bool inRange = IsBetween(playerTopLeftPoint.y,
                            otherTopRightPoint.y , otherTopRightPoint.y + plSettings.wallHangRange);

                        if ( inRange)
                        {
                            rbInfo.isLedgeHanging = true;
                        }
                        */
                    }
                    if(normal.x < -0.1f)
                    {
                        rbInfo.isRightContact = true; 

                        //its all about ledge climbing trigger but in other side
                        /*
                        var playerCollider = rb.rb.GetComponent<BoxCollider2D>();//fix in future pls
                        var playerTopLeftPoint = new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.max.y);

                        Vector2 otherTopRightPoint = new Vector2(rbContacts.contactPoints[i].collider.bounds.min.x,
                            rbContacts.contactPoints[i].collider.bounds.max.y);

                        bool inRange = IsBetween(playerTopLeftPoint.y,
                            otherTopRightPoint.y , otherTopRightPoint.y + plSettings.wallHangRange);

                        if ( inRange)
                        {
                            rbInfo.isLedgeHanging = true;
                            // Debug.DrawRay(playerTopLeftPoint, Vector3.right, Color.blue, 1f);
                        }
                        */
                    }
                }
            }
        }
    }
}