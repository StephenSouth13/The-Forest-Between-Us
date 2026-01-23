using UnityEngine;
using UnityEngine.AI;

namespace MapAndRadarSystem
{
    public class SimpleAIMove : MonoBehaviour
    {
        public Transform[] WalkingPoints;
        private Transform target = null;
        public NavMeshAgent agent;
       
        void Start()
        {
            target = WalkingPoints[Random.Range(0, WalkingPoints.Length)];
            agent.SetDestination(target.position);
        }

        float Distance = 0;
        void Update()
        {
            if(target != null)
            {
                Distance = Vector3.Distance(transform.position, target.position);
            }
            if(Distance < agent.stoppingDistance)
            {
                agent.isStopped = true;
                // Let's assign a new destination:
                target = WalkingPoints[Random.Range(0, WalkingPoints.Length)];
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }
}