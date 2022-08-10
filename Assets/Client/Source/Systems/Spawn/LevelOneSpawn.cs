using Leopotam.EcsLite;
using UnityEngine;

namespace Client {
    public class LevelOneSpawn : IEcsInitSystem {
        public void Init (IEcsSystems systems) {
            EcsWorld world = systems.GetWorld ();

            EcsPool<PlayerComponent> playersPool = world.GetPool<PlayerComponent>();
            EcsPool<EnemyComponent> enemyPool = world.GetPool<EnemyComponent>();

            EcsPool<AiGoOnPathComponent> aiGoOnPathPool = world.GetPool<AiGoOnPathComponent>();
            EcsPool<AiStopOnPathComponent> aiStopOnPathPool = world.GetPool<AiStopOnPathComponent>();

            EcsPool<AiGroundComponent> AiGroundPosPool = world.GetPool<AiGroundComponent>();
            EcsPool<FindPathToPlayerComponent> findPathToPlayerPool = world.GetPool<FindPathToPlayerComponent>();

            EcsPool<RbComponent> rbPool = world.GetPool<RbComponent>();
            EcsPool<RbDragComponent> rbDragPool = world.GetPool<RbDragComponent>();
            EcsPool<InputComponent> inputPool = world.GetPool<InputComponent>();

            var playerEnitity = world.NewEntity();

            var PlayerRes = Resources.Load<GameObject>("Character");
            var go = GameObject.Instantiate(PlayerRes, Vector3.zero, Quaternion.identity);
            playersPool.Add(playerEnitity);
            rbPool.Add(playerEnitity).rb = go.GetComponent<Rigidbody2D>();
            rbDragPool.Add(playerEnitity).xDrag = 20;
            rbDragPool.Get(playerEnitity).yDrag = 0;
            inputPool.Add(playerEnitity);


            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var enemyEntity = world.NewEntity();
                enemyPool.Add(enemyEntity);

                rbPool.Add(enemyEntity).rb = enemy.GetComponent<Rigidbody2D>();
                AiGroundPosPool.Add(enemyEntity).groundPos = enemy.GetComponent<EnemyAiInfo>().groundPos;
                findPathToPlayerPool.Add(enemyEntity);
                aiGoOnPathPool.Add(enemyEntity);
                // aiStopOnPathPool.Add(enemyEntity);
            }


            // var EnemyRes = Resources.Load<GameObject>("Enemy");
            // var goEnemy = GameObject.Instantiate(EnemyRes, Vector3.zero, Quaternion.identity);


        }
    }
}