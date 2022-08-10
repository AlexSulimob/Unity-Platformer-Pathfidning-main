using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class PlayerGravityControlSystem : IEcsRunSystem
    {

        public void Run(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var plSettings = systems.GetShared<SharedData>().playerSettings;
            var filter = world.Filter<PlayerComponent>().End ();//change filter in future it can broke othere states

            var inputPool = world.GetPool<InputComponent>();
            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var rbPool = world.GetPool<RbComponent>();

            foreach (var entity in filter)
            {
                var input = inputPool.Get(entity);
                var rb = rbPool.Get(entity);
                rb.rb.gravityScale = plSettings.jumpGravity;
                if (rb.rb.velocity.y < plSettings.gravityThreshold)
                {
                    rb.rb.gravityScale = plSettings.fallingGravity;
                }
                if(input.y_input == -1 && rb.rb.velocity.y < plSettings.gravityThreshold)
                {
                    rb.rb.gravityScale = plSettings.speedUpFallingGravity;
                }

                if (rb.rb.velocity.y <= plSettings.maxFallingSpeed)
                {
                    rb.rb.velocity = new Vector2(rb.rb.velocity.x, plSettings.maxFallingSpeed);
                }
            }

        }
    }
}