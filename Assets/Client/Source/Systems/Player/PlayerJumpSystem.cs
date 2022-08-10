using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public struct AirJumpLeft
    {
        public int value;
    }
    public struct CoyoteTime
    {
        public float coyoteTimeCounter;
        public float jumpBufferCounter;
        public bool jumpReleased;
    }
    public class PlayerJumpSystem : IEcsRunSystem,IEcsInitSystem {        
        
        public void Run (IEcsSystems systems) {

            EcsWorld world = systems.GetWorld();
            var plSettings = systems.GetShared<SharedData>().playerSettings;
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states

            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var inputPool = world.GetPool<InputComponent>();
            var rbPool = world.GetPool<RbComponent>();
            var rbDragPool = world.GetPool<RbDragComponent>();
            var playerIsTouchingWallPool = world.GetPool<PlayerIsTouchingWall>();

            var coyoteTimePool = world.GetPool<CoyoteTime>();
            var jumpCdPool = world.GetPool<JumpCd>();

            var airJumpLeftPool = world.GetPool<AirJumpLeft>();

            foreach (var entity in filter)
            {
                ref var input = ref inputPool.Get(entity);
                ref var rb = ref rbPool.Get(entity);
                ref var rbInfo = ref rbInfoPool.Get(entity);
                ref var rbDrag = ref rbDragPool.Get(entity);
                ref var airJumpLeft = ref airJumpLeftPool.Get(entity);

                //setting up coyoteTime counter if entity doesnt have that
                if (!coyoteTimePool.Has(entity))
                {
                    coyoteTimePool.Add(entity);
                }
                if (!jumpCdPool.Has(entity))
                {
                    jumpCdPool.Add(entity);
                }
                ref var jumpCd = ref jumpCdPool.Get(entity);
                ref var coyoteTime = ref coyoteTimePool.Get(entity);

                #region coyoteTime counter
                if (rbInfo.isBottomContact)
                {
                    coyoteTime.coyoteTimeCounter = plSettings.coyoteTime;
                    airJumpLeft.value = plSettings.amountOfAirJumps;
                }
                else 
                {
                    coyoteTime.coyoteTimeCounter -= Time.deltaTime;
                }
                #endregion

                #region Jump buffer counter
                if(input.jump)
                {
                    coyoteTime.jumpBufferCounter = plSettings.jumpBuffer;
                    coyoteTime.jumpReleased = false;
                }
                else 
                {
                    coyoteTime.jumpBufferCounter -= Time.deltaTime;
                }
                if(input.jumpRelease)
                {
                    coyoteTime.jumpReleased = true;
                }
                #endregion

                bool jumpCdDone = Time.time >= jumpCd.jumpCdStartTime + plSettings.jumpCd;//these check pervent from situation when player can jump twice
                //jump. It depend on jump buffer counter, not on jump button down
                if (coyoteTime.jumpBufferCounter > 0f 
                    && (coyoteTime.coyoteTimeCounter > 0f || airJumpLeft.value > 0) 
                    && jumpCdDone
                    && !playerIsTouchingWallPool.Has(entity))
                {
                    var force = plSettings.JumpForce;
                    if (coyoteTime.jumpReleased)
                    {
                        force = plSettings.LowJumpForce;
                    }
                    
                    rbDrag.yDrag = 0f;
                    rb.rb.velocity = new Vector2(rb.rb.velocity.x, 0f);
                    // rb.rb.velocity = Vector2.zero;
                    rb.rb.AddForce(force * Vector2.up, ForceMode2D.Impulse); //jump
                    coyoteTime.jumpBufferCounter = 0f;
                    airJumpLeft.value -= 1;
                    jumpCd.jumpCdStartTime = Time.time;
                }
                //jump cut for variable jump height 
                if (input.jumpRelease && rb.rb.velocity.y > 0f)
                {
                    rb.rb.velocity = new Vector2(rb.rb.velocity.x, rb.rb.velocity.y * plSettings.JumpCutValue); //jump cut

                    coyoteTime.coyoteTimeCounter = 0f; //it pervent from situation when player can jump twice
                }

            }
        }

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states

            var jumpCdPool = world.GetPool<JumpCd>();
            var coyoteTimePool = world.GetPool<CoyoteTime>();
            var airJumpLeftPool = world.GetPool<AirJumpLeft>();

            foreach (var entity in filter)
            {
                jumpCdPool.Add(entity);
                coyoteTimePool.Add(entity);
                airJumpLeftPool.Add(entity);
            }
        }
    }
    
}