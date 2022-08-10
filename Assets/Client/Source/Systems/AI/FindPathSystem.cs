using Leopotam.EcsLite;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// System witch get find path component and search path 
    /// </summary>
    public class FindPathSystem : IEcsRunSystem, IEcsDestroySystem {        
        private struct FindPathContainers
        {
            public NativeList<int> result;
            public NativeHeap<NodePriority, minDistanceComparer> frontier;
            public JobHandle handle;
            public bool isFinding;
            public float foundedTime;
        }
        public void Run (IEcsSystems systems) {
            EcsWorld world = systems.GetWorld();

            var findPathFilter = world.Filter<FindPathToComponent>().End ();

            var levelGraph = systems.GetShared<SharedData>().graphLevel;
            var findPathPool = world.GetPool<FindPathToComponent>();
            var findPathContainersPool = world.GetPool<FindPathContainers>();
            var pathPool = world.GetPool<PathComponent>();

            float cooldownMarginBetweenEntites = 0f; 
            foreach (var entity in findPathFilter)
            {
                /* setting up components or get if we already have done this*/
                ref var findPath = ref findPathPool.Get(entity);

                if(!findPathContainersPool.Has(entity))
                {
                    findPathContainersPool.Add(entity).isFinding = false;
                    findPathContainersPool.Get(entity).foundedTime = Time.time;
                }
                ref var findPathContainers = ref findPathContainersPool.Get(entity);

                //check our flag and start search path
                if(!findPathContainers.isFinding && findPathContainers.foundedTime + 0.2f < Time.time)
                {
                    findPathContainers.frontier = new NativeHeap<NodePriority, minDistanceComparer>(Allocator.Persistent);
                    findPathContainers.result = new NativeList<int>(Allocator.Persistent);

                    FindPathStarJob findPathJob = new FindPathStarJob()
                    {
                        frontier = findPathContainers.frontier,
                        m_graph = levelGraph.graphContainer,
                        nodes = levelGraph.nodesContainer,
                        startPos = (int2)vector2_to_float2(findPath.startPoint),
                        endPos = (int2)vector2_to_float2(findPath.endPoint),
                        nodesCount = levelGraph.nodesContainer.Length,
                        Result = findPathContainers.result
                    };
                    findPathContainers.handle = findPathJob.Schedule();
                    findPathContainers.isFinding = true;
                }

                if(findPathContainers.isFinding && findPathContainers.handle.IsCompleted)
                {
                    findPathContainers.handle.Complete(); //there our job has already completed but anyway we need to complete for avoid errors

                    //set our path result to entity
                    var ResultLenght = findPathContainers.result.Length;


                    if(!pathPool.Has(entity))
                    {
                        if (ResultLenght != 0) {
                            ref var pathComp = ref pathPool.Add(entity);
                            pathComp.path = new NativeArray<int>(findPathContainers.result, Allocator.Persistent);
                            // pathComp.path = ;
                            pathComp.pathUpdated = true;
                            findPathContainers.foundedTime = Time.time + cooldownMarginBetweenEntites; 
                            cooldownMarginBetweenEntites += 0.005f;
                        }
                    }
                    else
                    {
                        if (ResultLenght != 0){
                            ref var pathComp = ref pathPool.Get(entity);
                            pathComp.path.Dispose(); 
                            pathComp.path = new NativeArray<int>(findPathContainers.result, Allocator.Persistent);
                            pathComp.pathUpdated = true;
                            findPathContainers.foundedTime = Time.time + cooldownMarginBetweenEntites;
                            cooldownMarginBetweenEntites += 0.005f;
                        }
                    }

                    //dispos containers and set flag
                    findPathContainers.frontier.Dispose();
                    findPathContainers.result.Dispose();

                    findPathContainersPool.Del(entity);
                    
                    findPathContainers.isFinding = false;
                }

            }
        }
        private float2 vector2_to_float2(Vector2 vector)
        {
            return new float2(vector.x, vector.y);
        }

        public void Destroy(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();
            var findPathFilter = world.Filter<FindPathContainers>().End();
            var pathFilter = world.Filter<PathComponent>().End();

            var findPathContainersPool = world.GetPool<FindPathContainers>();
            var pathPool = world.GetPool<PathComponent>();

            foreach (var entity in pathFilter)
            {
                ref var path_c = ref pathPool.Get(entity);
                path_c.path.Dispose();
            }
            foreach (var entity in findPathFilter)
            {
                ref var findPathContainers = ref findPathContainersPool.Get(entity);
                findPathContainers.handle.Complete();
                findPathContainers.result.Dispose();
                findPathContainers.frontier.Dispose();

                findPathContainersPool.Del(entity);
            }
        }
    }
    [BurstCompile]
    public struct FindPathStarJob : IJob
    {
        [ReadOnly]
        public NativeMultiHashMap<int, NodeConnection> m_graph;
        [ReadOnly]
        public NativeList<Node> nodes;
        public int2 startPos;
        public int2 endPos;
        // public NativeArray<int> ResultArray;
        public NativeList<int> Result;
        
        public NativeHeap<NodePriority, minDistanceComparer> frontier;
        public int nodesCount;
        bool hasExit;

        public void Execute()
        {
            hasExit = false;
            Node StartNode = FindNearestNode(startPos, nodes);
            Node EndNode = FindNearestNode(endPos, nodes);

            frontier.Insert(new NodePriority(StartNode.index, 0));

            // NativeList<int> Result = new NativeList<int>(Allocator.Temp);
            NativeArray<bool> visited = new NativeArray<bool>(nodesCount, Allocator.Temp);
            NativeArray<int> came_from = new NativeArray<int>(nodesCount, Allocator.Temp);
            NativeArray<int> costSoFar = new NativeArray<int>(nodesCount, Allocator.Temp);

            visited[StartNode.index] = true;
            costSoFar[StartNode.index] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Pop();

                if (current.index == EndNode.index)
                {
                    hasExit = true;
                    break;
                }

                var conncetions = m_graph.GetValuesForKey(current.index);
                
                foreach (var con in conncetions)
                {
                    var newCost = costSoFar[current.index] + con.cost;
                    if( !visited[con.node.index] )
                    {
                        //do calculate priority
                        //calculate manhetan distance and most closed con must be enqueue last
                        costSoFar[con.node.index] = newCost;
                        int priority = newCost + ManhattanDistance(EndNode.pos, con.node.pos);

                        frontier.Insert(new NodePriority(con.node.index, priority));//needs ad priority

                        visited[con.node.index] = true;

                        came_from[con.node.index] = current.index;
                    }
                }
            }


            if (hasExit)
            {
                //fill result array
                int currentPathNode = EndNode.index;
                Result.Add(currentPathNode);
                while (currentPathNode != StartNode.index)
                {
                    currentPathNode = came_from[currentPathNode];
                    Result.Add(currentPathNode);
                }
                Result.Add(StartNode.index);

                //reverse result array
                for (int i = 0; i < Result.Length /2; i++)
                {
                    int tmp = Result[i];
                    Result[i] = Result[Result.Length - i -1];
                    Result[Result.Length - i -1] = tmp;
                }
            }
            // Result.Dispose();
            // came_from.Dispose();

        }
        int ManhattanDistance(int2 pointOne, int2 pointTwo)
        {
            return math.abs(pointOne.x - pointTwo.x) + math.abs(pointOne.y - pointTwo.y);
        }

        Node FindNearestNode(int2 pos,NativeArray<Node> nodes)
        {
            float distance = float.MaxValue;
            Node returnValue = Graph.emptyNode; 
            for (int i = 0; i < nodes.Length -1; i++)
            {
                var nodeDistance = math.distance(pos, nodes[i].pos);
                if (nodeDistance < distance && nodes[i].index != -1)
                {
                    returnValue = nodes[i];
                    distance = nodeDistance;
                }
            }
            return returnValue;
        }
    }
}