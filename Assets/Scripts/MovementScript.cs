using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementScript : MonoBehaviour
{
    GameManagerScript mGameManager;
    AttributesScript mAttributes;
    NavMeshAgent mAgent;
    AISensingScript mSensing;

    [SerializeField]
    float pathfindingMinRange;
    [SerializeField]
    float pathfindingMaxRange;

    public GameObject enemy;
    public GameObject target;

    private bool isWandering = true;

    private void Awake()
    {
        mGameManager = FindObjectOfType<GameManagerScript>();
        mAttributes = GetComponent<AttributesScript>();
        mAgent = GetComponent<NavMeshAgent>();
        mSensing = GetComponentInChildren<AISensingScript>();
        mAgent.speed = AttributesScript.mMovementSpeed;
    }
    public void NonInfectedMovement()
    {
        enemy = CheckForCloseInfectedAgent();

        if (enemy)
        {
            FleeFromEnemy();
            isWandering = false;
        }
        else
        {
            Wander();
            isWandering = true;
        }
    }
    
    public void InfectedMovement()
    {
        target = FindTarget();
        ChaseAgent();
    }

    public GameObject CheckForCloseInfectedAgent()
    {
        GameObject closestEnemy = null;
        float distance = float.MaxValue;
        foreach (Collider enemy in mSensing.mSensingArray)
        {
            float d = Vector3.Distance(gameObject.transform.position, enemy.gameObject.transform.position);
            if (enemy.gameObject.GetComponent<AttributesScript>().mInfected && (d < distance))
            {
                closestEnemy = enemy.gameObject;
                distance = d;
            }
        }
        return closestEnemy;
    }

    public GameObject FindTarget()
    {
        float max = float.MaxValue;

        foreach (GameObject obj in mGameManager.mNonInfectedAgents)
        {
            float distance = Vector3.Distance(obj.transform.position, gameObject.transform.position);
            if (distance < max)
            {
                target = obj;
                max = distance;
            }
        }
        return target;
    }

    public void FleeFromEnemy()
    {
        if (!enemy)
            return;

        if (mAgent.remainingDistance < 1.0f || isWandering)
        {
            float dist = float.MinValue;
            for (int i = 0; i < 10; i++)
            {
                NavMeshHit hit;
                float range = Random.Range(pathfindingMinRange, pathfindingMaxRange);
                if (GetRandomPos(transform.position, out hit, pathfindingMinRange, pathfindingMaxRange,pathfindingMaxRange))
                {
                    float f = Vector3.Distance(hit.position, enemy.transform.position);
                    if (f > dist)
                    {
                        dist = f; ;
                        mAgent.SetDestination(hit.position);
                    }
                    Debug.DrawLine(hit.position, hit.position + Vector3.up * 5.0f, Color.red, 5.0f);
                }

            }
        }
    }

    bool GetRandomPos(Vector3 pos, out NavMeshHit hit, float rangeMin, float rangeMax, float maxDistance)
    {
        NavMeshHit h;
        float range = Random.Range(rangeMin, rangeMax);
        Vector3 randomPoint = pos + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPoint, out h, maxDistance, NavMesh.AllAreas))
        {
            hit = h;
            return true;
        }
        hit = h;
        return false;
    }

    public void Wander()
    {
        if (mAgent.remainingDistance < 1.0f || mAgent.isStopped)
        {
            float angleCloseTo45 = 180;
            NavMeshHit hit;

            for (int i = 0; i < 10; i++)
            {
                if (GetRandomPos(transform.position, out hit, pathfindingMinRange, pathfindingMaxRange, pathfindingMaxRange))
                {
                    float angle = Vector3.Angle(transform.forward, (hit.position - transform.position));
                    if (Mathf.Abs(angle - 45.0f) < angleCloseTo45)
                    {
                        mAgent.SetDestination(hit.position);
                        angleCloseTo45 = Mathf.Abs(angle - 45.0f);
                    }
                    Debug.DrawLine(hit.position, hit.position + Vector3.up * 5.0f, Color.red, 5.0f);
                }
            }
        }
    }

    public void ChaseAgent()
    {
        if (target)
        {
            mAgent.SetDestination(target.transform.position);
        }
        else
        {
            mAgent.isStopped = true;
        }
    }
}
