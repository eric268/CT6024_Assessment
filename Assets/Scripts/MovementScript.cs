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

    public GameObject mCloseEnemy;
    public GameObject mCurrentTarget;

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
        mCloseEnemy = CheckForCloseInfectedAgent();

        if (mCloseEnemy)
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
        mCurrentTarget = FindTarget();
        ChaseAgent();
    }

    public GameObject CheckForCloseInfectedAgent()
    {
        mCloseEnemy = null;
        float distance = float.MaxValue;
        foreach (Collider e in mSensing.mSensingArray)
        {
            float d = Vector3.Distance(gameObject.transform.position, e.gameObject.transform.position);
            if (e.gameObject.GetComponent<AttributesScript>().mInfected && (d < distance))
            {
                mCloseEnemy = e.gameObject;
                distance = d;
            }
        }
        return mCloseEnemy;
    }

    public GameObject FindTarget()
    {
        float max = float.MaxValue;

        foreach (GameObject obj in mGameManager.mNonInfectedAgents)
        {
            float distance = Vector3.Distance(obj.transform.position, gameObject.transform.position);
            if (distance < max)
            {
                mCurrentTarget = obj;
                max = distance;
            }
        }
        return mCurrentTarget;
    }

    public void FleeFromEnemy()
    {
        if (!mCloseEnemy)
            return;

        if (mAgent.remainingDistance < 1.0f || isWandering)
        {
            float dist = float.MinValue;
            isWandering = false;
            for (int i = 0; i < 10; i++)
            {
                NavMeshHit hit;
                float range = Random.Range(pathfindingMinRange, pathfindingMaxRange);
                if (GetRandomPos(transform.position, out hit, pathfindingMinRange, pathfindingMaxRange,pathfindingMaxRange))
                {
                    float f = Vector3.Distance(hit.position, mCloseEnemy.transform.position);
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
            isWandering = true;

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
        if (mCurrentTarget)
        {
            mAgent.SetDestination(mCurrentTarget.transform.position);
        }
        else
        {
            mAgent.isStopped = true;
        }
    }
}
