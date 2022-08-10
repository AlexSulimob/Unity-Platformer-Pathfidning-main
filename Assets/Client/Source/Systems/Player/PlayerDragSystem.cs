using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class RbDragSystem : IEcsRunSystem
    {
        // EcsFilter filterCache; 
        public void Run(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            // var plSettings = systems.GetShared<SharedData>().playerSettings;
            
            var filter = world.Filter<RbComponent>().Inc<RbDragComponent>().End ();
            // filter.Equals(filter);

            var rbPool = world.GetPool<RbComponent>();
            var rbDragPool = world.GetPool<RbDragComponent>();

            foreach (var entity in filter)
            {
                ref var rb = ref rbPool.Get(entity).rb;   
                ref var rbDrag = ref rbDragPool.Get(entity);   
                
                rb.velocity = new Vector2(rb.velocity.x * Mathf.Clamp01(1f - rbDrag.xDrag * Time.deltaTime), rb.velocity.y);
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * Mathf.Clamp01(1f - rbDrag.yDrag * Time.deltaTime));
            }
        }
    }
}