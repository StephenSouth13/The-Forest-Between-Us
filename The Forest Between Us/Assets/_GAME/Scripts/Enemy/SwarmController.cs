using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{
    [Header("Swarm Members")]
    public List<GameObject> swarmMembers = new List<GameObject>();
    public float swarmRadius = 3f;
    public float swarmSpeed = 3f;

    [Header("Patrol Area")]
    public Transform centerPoint;
    public float wanderRadius = 15f;
    public float changeTargetInterval = 8f;

    private Vector3 currentTargetPos;
    private float timer;

    void Start()
    {
        if (centerPoint == null) centerPoint = transform;
        SetNewRandomTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeTargetInterval)
        {
            timer = 0f;
            SetNewRandomTarget();
        }

        MoveSwarm();
    }

    void SetNewRandomTarget()
    {
        Vector2 randCircle = Random.insideUnitCircle * wanderRadius;
        currentTargetPos = centerPoint.position + new Vector3(randCircle.x, 0, randCircle.y);
    }

    void MoveSwarm()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, swarmSpeed * Time.deltaTime);

        for (int i = 0; i < swarmMembers.Count; i++)
        {
            if (swarmMembers[i] == null) continue;

            float angle = (i * 360f / swarmMembers.Count) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * swarmRadius;
            Vector3 memberTarget = transform.position + offset;

            swarmMembers[i].transform.position = Vector3.Lerp(
                swarmMembers[i].transform.position, 
                memberTarget, 
                Time.deltaTime * 2f
            );
        }
    }
}
