using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class FindPathToPlayerSystem : IEcsRunSystem {        
        public void Run (IEcsSystems systems) {
            EcsWorld world = systems.GetWorld();

            var filter = world.Filter<FindPathToPlayerComponent>().End ();
            var playerFilter = world.Filter<PlayerComponent>().Inc<RbComponent>().End ();

            EcsPool<FindPathToComponent> findPathPool = world.GetPool<FindPathToComponent>();
            EcsPool<RbComponent> rbPool = world.GetPool<RbComponent>();
            EcsPool<AiGroundComponent> AiGroundPosPool = world.GetPool<AiGroundComponent>();
            // EcsPool<FollowT> AiGroundPosPool = world.GetPool<AiGround>();

            Vector2 playerPos = Vector2.zero;
            foreach (var playerE in playerFilter) {
                playerPos = rbPool.Get(playerE).rb.position;
            }

            foreach (var entity in filter){
                if(!findPathPool.Has(entity))
                {
                    findPathPool.Add(entity);
                }
                ref var findPath = ref findPathPool.Get(entity);
                ref var aiGroundPos = ref AiGroundPosPool.Get(entity);
                findPath.endPoint = playerPos;
                findPath.startPoint = aiGroundPos.groundPos.position;
            }
        }
    }
}