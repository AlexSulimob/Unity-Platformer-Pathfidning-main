using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public struct PlayerIsTouchingWall
    {

    }
    public class PlayerWallSlideSystem : IEcsRunSystem
    {
    
        public void Run(IEcsSystems systems)
        {
            
            EcsWorld world = systems.GetWorld();
            var plSettings = systems.GetShared<SharedData>().playerSettings;
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states

            var inputPool = world.GetPool<InputComponent>();
            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var rbPool = world.GetPool<RbComponent>();
            var coyoteTimePool = world.GetPool<CoyoteTime>();
            var playerIsTouchingWallPool = world.GetPool<PlayerIsTouchingWall>();

            foreach (var entity in filter)
            {
                ref var rbInfo = ref rbInfoPool.Get(entity);
                ref var rb = ref rbPool.Get(entity);
                ref var input = ref inputPool.Get(entity);
                ref var coyoteTime = ref coyoteTimePool.Get(entity);

                bool isTouchingWall = rbInfo.isLeftContact || rbInfo.isRightContact;
                bool isFalling = rb.rb.velocity.y < 0f; 
                
                int wallDir = 1; 
                if(isTouchingWall)
                {
                    wallDir = rbInfo.isLeftContact ? 1 : -1;
                }

                if(isTouchingWall && !playerIsTouchingWallPool.Has(entity))
                {
                   playerIsTouchingWallPool.Add(entity); 
                }                
                if(!isTouchingWall && playerIsTouchingWallPool.Has(entity))
                {
                    playerIsTouchingWallPool.Del(entity);
                }

                if(isTouchingWall && isFalling)
                {
                    rb.rb.velocity = new Vector2(rb.rb.velocity.x, plSettings.wallSlideSpeed);
                    
                }
                if(isTouchingWall && coyoteTime.jumpBufferCounter > 0)
                {
                    Vector2 force = new Vector2(plSettings.wallJumpAngle.x * plSettings.x_wallJumpforce * wallDir,
                        plSettings.wallJumpAngle.y * plSettings.y_wallJumpforce);

                    rb.rb.AddForce(force, ForceMode2D.Impulse);
                    coyoteTime.jumpBufferCounter = 0f;
                }
                
            }
        }
    }
}