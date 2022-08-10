using Leopotam.EcsLite;
using UnityEngine;

namespace Client {
    public class PlayerInputSystem : IEcsRunSystem {        
        public void Run (IEcsSystems systems) {
            
            EcsWorld world = systems.GetWorld();
            var inputFilter = world.Filter<InputComponent>().End ();

            var inputPool = world.GetPool<InputComponent>();

            foreach (var entity in inputFilter)
            {
                ref var input = ref inputPool.Get(entity);

                input.x_input =  Input.GetAxisRaw("Horizontal");
                input.y_input =  Input.GetAxisRaw("Vertical");

                input.jump = Input.GetButtonDown("Jump");
                input.holdingJump = Input.GetButton("Jump");
                input.jumpRelease = Input.GetButtonUp("Jump");
            }
        }
    }
}