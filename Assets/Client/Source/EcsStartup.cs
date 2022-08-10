using Leopotam.EcsLite;
using UnityEngine;

namespace Client {
    sealed class EcsStartup : MonoBehaviour {
        EcsSystems _update_systems;
        EcsSystems _fixed_systems;
        public SharedData sharedData;

        void Start () {        
            // Application.targetFrameRate = 60;
            // register your shared data here, for example:
            // var shared = new Shared ();
            // systems = new EcsSystems (new EcsWorld (), shared);
            EcsWorld _world = new EcsWorld();
            _update_systems = new EcsSystems (_world, sharedData);
            _fixed_systems = new EcsSystems (_world, sharedData);

            _update_systems
                .Add(new LevelOneSpawn())
                .Add(new PlayerInputSystem())
                .Add(new PlayerJumpSystem())
                .Add(new FindPathSystem())
                .Add(new FindPathToPlayerSystem())
                // register your systems here, for example:
                // .Add (new TestSystem1 ())
                // .Add (new TestSystem2 ()Application.targetFrameRate = 300;)
                
                // register additional worlds here, for example:
                // .AddWorld (new EcsWorld (), "events")
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
                .Init ();

            _fixed_systems
                .Add(new RbPhysicsInfoSystem())
                .Add(new PlayerGroundMovementSystem())
                .Add(new RbDragSystem())
                .Add(new PlayerGravityControlSystem())
                .Add(new PlayerWallSlideSystem())

                .Add(new AiGoOnPathSystem())
                .Init();

        }

        void Update () {
            _update_systems?.Run ();
        }
        private void FixedUpdate() {
           _fixed_systems?.Run(); 
        }

        void OnDestroy () {
            if (_update_systems != null) {
                _fixed_systems.Destroy();
                _update_systems.Destroy ();
                
                // add here cleanup for custom worlds, for example:
                // _systems.GetWorld ("events").Destroy ();
                _fixed_systems.GetWorld ().Destroy ();
                _update_systems.GetWorld ().Destroy ();

                _fixed_systems = null;
                _update_systems = null;
            }
        }
    }
}