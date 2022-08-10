using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class PlayerGroundMovementSystem : IEcsRunSystem, IEcsInitSystem {        
        struct PlayerSpeed
        {
            public float value;
        }
        public void Run (IEcsSystems systems) {

            EcsWorld world = systems.GetWorld();
            var plSettings = systems.GetShared<SharedData>().playerSettings;
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states

            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var inputPool = world.GetPool<InputComponent>();
            var rbPool = world.GetPool<RbComponent>();
            var rbDragPool = world.GetPool<RbDragComponent>();
            var jumpCdPool = world.GetPool<JumpCd>();

            var playerSpeedPool = world.GetPool<PlayerSpeed>();

            foreach (var entity in filter)
            {
                ref var input = ref inputPool.Get(entity);
                ref var rb = ref rbPool.Get(entity);
                ref var rbInfo = ref rbInfoPool.Get(entity);
                ref var rbDrag= ref rbDragPool.Get(entity);
                ref var jumpCd = ref jumpCdPool.Get(entity);

                if (!playerSpeedPool.Has(entity))
                {
                    playerSpeedPool.Add(entity);
                }
                ref var playerSpeed= ref playerSpeedPool.Get(entity);

                rbDrag.yDrag = 0f; 
                if (rbInfo.isBottomContact)
                {
                    playerSpeed.value = plSettings.groundSpeed;
                    rbDrag.xDrag = plSettings.groundDrag; 
                    var force = playerSpeed.value * input.x_input;

                    if(rbInfo.isOnSlope)
                    {
                        bool jumpCdDone = Time.time >= jumpCd.jumpCdStartTime + plSettings.jumpCd;
                        if (jumpCdDone)
                        {
                            rbDrag.yDrag = rbDrag.xDrag;
                            force = plSettings.slopeSpeed * input.x_input;
                            rb.rb.AddForce(force * -rbInfo.slopePerpendicular.normalized);
                        }
                        else 
                        {
                            rb.rb.AddForce(force * Vector2.right);
                        }
                    }
                    else 
                    {
                        rb.rb.AddForce(force * Vector2.right);
                    }
                }
                else 
                {
                    bool isChangeDir = (input.x_input < -0.1 && rb.rb.velocity.x > 0.1) || (input.x_input > 0.1 && rb.rb.velocity.x < -0.1);
                    if (input.x_input == 0f || isChangeDir)
                    {
                        rbDrag.xDrag = plSettings.airDrag; 
                        playerSpeed.value = plSettings.airSpeed;
                    }
                    var force = playerSpeed.value* input.x_input;
                    rb.rb.AddForce(force * Vector2.right);
                }
                
            }
        }

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states
            
            var playerSpeedPool = world.GetPool<PlayerSpeed>();

            foreach (var entity in filter)
            {
                playerSpeedPool.Add(entity);
                
            }
        }
    }
}