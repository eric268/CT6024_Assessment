using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CurrentState
{
    NEWLY_INFECTED,
    NOT_INFECTED,
    INFECTED,
    NUM_STATES
}

public class AIScript : MonoBehaviour
{
    AttributesScript mAttributes;
    MovementScript mMovement;
    NavMeshAgent mAgent;
    // Start is called before the first frame update

    public void Awake()
    {
        mAttributes = GetComponent<AttributesScript>();
        mMovement = GetComponent<MovementScript>();
        mAgent = GetComponent<NavMeshAgent>();  
    }

    void Start()
    {
        InvokeRepeating(nameof(RunStateMachine), 0.0f, mAttributes.mBehaviourUpdateFrequency);
    }

    void RunStateMachine()
    {
        switch (mAttributes.mCurrentState)
        {
            case CurrentState.NEWLY_INFECTED:
                //Do nothing
                break;
            case CurrentState.NOT_INFECTED:
                NonInfectedBehaviour();
                break;
            case CurrentState.INFECTED:
                InfectedBehaviour();
                break;
        }
    }

    void InfectedBehaviour()
    {
        mAgent.isStopped = false;
        mMovement.InfectedMovement();
    }

    void NonInfectedBehaviour()
    {
        mMovement.NonInfectedMovement();
    }
}
