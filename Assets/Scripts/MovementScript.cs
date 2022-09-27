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

    private void Awake()
    {
        mGameManager = FindObjectOfType<GameManagerScript>();
        mAttributes = GetComponent<AttributesScript>();
        mAgent = GetComponent<NavMeshAgent>();
        mSensing = GetComponentInChildren<AISensingScript>();
    }
    public void NonInfectedMovement()
    {
        GameObject enemy = CheckForCloseInfectedAgent();

        if (enemy)
        {
            FleeFromEnemy(enemy);
        }
        else
        {
            Wander();
        }
    }
    
    public void InfectedMovement()
    {
        GameObject target = FindTarget();
        ChaseAgent(target);
    }

    GameObject CheckForCloseInfectedAgent()
    {
        GameObject closestEnemy = null;
        float distance = float.MaxValue;

        foreach (Collider enemy in mSensing.mSensingArray)
        {
            if (enemy.gameObject.GetComponent<AttributesScript>().mInfected && (Vector3.Distance(gameObject.transform.position, enemy.gameObject.transform.position) < distance))
            {
                closestEnemy = enemy.gameObject;
            }
        }

        return closestEnemy;
    }

    GameObject FindTarget()
    {
        GameObject target = null;
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

    public void FleeFromEnemy(GameObject pos)
    {
        
    }   
    
    public void Wander()
    {

    }

    public void ChaseAgent(GameObject target)
    {
        if (target)
        {
            mAgent.SetDestination(target.transform.position);
        }

    }
}
