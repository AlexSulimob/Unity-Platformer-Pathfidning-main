using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    public class AiGoOnPathSystem : IEcsRunSystem
    {
        struct AiBodyInfoComponent
        {
            public bool jumpPerformed;
            public float fallDir;
            public bool fallPerformed;
            public bool lockUpdatePath;
            public float jumpTimeStarted;
        }
        struct CurrentPathState 
        {
            public int currnetPathNode;
            public Node currentNode;
            public Node nextNode;
            public Node lastNode;
            public NodeConnection currentNodeConnection;

            public NodeConnection nextNodeConnection;
        }
        public void Run(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            var filter = 
                world.Filter<PathComponent>().Inc<AiGoOnPathComponent>().Inc<RbComponent>().Inc<AiGroundComponent>()
                    .Exc<AiStopOnPathComponent>().End ();

            var pathPool = world.GetPool<PathComponent>();
            var rbPool = world.GetPool<RbComponent>();
            var rbInfoPool = world.GetPool<RbInfoComponent>();
            var aiGroundPool = world.GetPool<AiGroundComponent>();
            var currentPathStatePool = world.GetPool<CurrentPathState>();
            var aiBodyInfoPool = world.GetPool<AiBodyInfoComponent>();

            var levelGraph = systems.GetShared<SharedData>().graphLevel;

            foreach (var entity in filter)
            {
                if(!currentPathStatePool.Has(entity))
                {
                    currentPathStatePool.Add(entity);
                }
                if(!aiBodyInfoPool.Has(entity))
                {
                    aiBodyInfoPool.Add(entity);
                }

                ref var path_c = ref pathPool.Get(entity);
                ref var currentPathState = ref currentPathStatePool.Get(entity);
                ref var bodyInfo_c = ref aiBodyInfoPool.Get(entity);
                ref var aiGround = ref aiGroundPool.Get(entity);

                ref var rb_c = ref rbPool.Get(entity);
                ref var rbInfo_c = ref rbInfoPool.Get(entity);

                Vector2 entityGroundPos = aiGround.groundPos.position;

                if(path_c.path.Length <= 2)
                {
                    continue;
                }

                //update current path
                if(path_c.pathUpdated 
                && (!bodyInfo_c.jumpPerformed || bodyInfo_c.jumpTimeStarted + 2f < Time.time))
                // && rbInfo_c.isBottomContact) 
                {
                    currentPathState.currnetPathNode = 1;
                    currentPathState.currentNode = levelGraph.Nodes[path_c.path[1]];
                    currentPathState.nextNode = levelGraph.Nodes[path_c.path[2]];
                    currentPathState.lastNode = levelGraph.Nodes[path_c.path [ path_c.path.Length - 2 ] ];
                    currentPathState.currentNodeConnection = levelGraph.nodeConnections[1][2];

                    path_c.pathUpdated = false;
                }

                //is reached end of path
                if (currentPathState.lastNode.index == currentPathState.currentNode.index)
                {
                    //stop rb body if we are grounded
                    if (rbInfo_c.isBottomContact)
                        rb_c.rb.velocity = Vector2.zero;

                    bodyInfo_c.jumpPerformed = false;
                    continue;
                }

                var currentConnectionType = 
                    levelGraph.nodeConnections[currentPathState.currentNode.index][currentPathState.nextNode.index].connectionType;

                Vector2 nextNodePos = currentPathState.nextNode.pos.ToVector2();
                bool isReachedNextNode = Vector2.Distance(entityGroundPos, nextNodePos) < 0.2f ? true:false; 

                var dir = nextNodePos - entityGroundPos;
                dir = dir.normalized;
                var x_dir = 0f;
                if (dir.x> 0f) {x_dir = 1;}
                if (dir.x< 0f) {x_dir = -1;}

                switch (currentConnectionType)
                {
                    case NodeConnectionType.Walk:

                        if( rbInfo_c.isBottomContact)
                        {
                            bodyInfo_c.fallPerformed = false;
                            rb_c.rb.velocity = new Vector2(x_dir * 5f, 0f);
                            bodyInfo_c.jumpPerformed = false;
                        }
                    break;
                    case NodeConnectionType.Jump:
                        bodyInfo_c.fallPerformed = false;
                        //if we had miss jump we just walk to node
                        if (bodyInfo_c.jumpPerformed 
                            && rbInfo_c.isBottomContact 
                            && bodyInfo_c.jumpTimeStarted + 0.2f < Time.time)
                        {
                            rb_c.rb.velocity = new Vector2(x_dir * 5f, 0f);
                        }
                        else //jump behaviour
                        {
                            currentPathState.currentNodeConnection =
                                levelGraph.nodeConnections[currentPathState.currentNode.index][currentPathState.nextNode.index];

                            var jump = currentPathState.currentNodeConnection.jumpAi; 

                            rb_c.rb.velocity = new Vector2(jump.RunSpeed, rb_c.rb.velocity.y);
                            if(!bodyInfo_c.jumpPerformed  && rbInfo_c.isBottomContact){
                                rb_c.rb.velocity = Vector2.zero;
                                rb_c.rb.AddForce(jump.JumpForce * Vector2.up, ForceMode2D.Impulse);
                                bodyInfo_c.jumpPerformed = true;
                                bodyInfo_c.jumpTimeStarted = Time.time;
                            }
                        }
                    break;
                    case NodeConnectionType.Fall:
                        if (!bodyInfo_c.fallPerformed) {
                            bodyInfo_c.fallDir = x_dir;
                            bodyInfo_c.fallPerformed = true;
                        }

                        if (rbInfo_c.isBottomContact) {
                            rb_c.rb.velocity = new Vector2(bodyInfo_c.fallDir * 10f, 0f);
                        } else {
                            rb_c.rb.velocity = new Vector2(0f, rb_c.rb.velocity.y);
                        }
                        bodyInfo_c.jumpPerformed = false;
                    break;
                }

                //update current path if we reached goal node
                if (isReachedNextNode) {
                    bodyInfo_c.jumpPerformed = false;
                    currentPathState.currentNode = currentPathState.nextNode;

                    currentPathState.currnetPathNode++;

                    int tmp = currentPathState.currnetPathNode;
                    if(tmp <= path_c.path.Length -2) {
                        currentPathState.nextNode = levelGraph.Nodes[path_c.path[tmp + 1]];
                    }

                }


                // foreach (var item in currentPathState.pa) {
                //     Color color = Color.magenta; 
                //     switch (item.Value.connectionType) {
                //         case NodeConnectionType.Walk: color = Color.magenta;break;
                //         case NodeConnectionType.Jump: color = Color.cyan;break;
                //         case NodeConnectionType.Fall: color = Color.black;break;
                //     }
                //     if(item.Value.connectionType != NodeConnectionType.Empty) {
                //         Debug.DrawLine(item.Key.pos.ToVector2(), item.Value.node.pos.ToVector2(), color, 1f);
                //     }
                //     Debug.DrawRay(item.Key.pos.ToVector2(), Vector3.up, Color.black, 1f);
                // }

            }
        }
    }
}