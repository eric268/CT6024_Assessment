using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CurrentState
{
    NOT_INFECTED,
    INFECTED,
    NUM_STATES
}

public class AIScript : MonoBehaviour
{
    AttributesScript mAttributes;
    MovementScript mMovement;
    // Start is called before the first frame update

    public void Awake()
    {
        mAttributes = GetComponent<AttributesScript>();
        mMovement = GetComponent<MovementScript>();
    }

    void Start()
    {
        InvokeRepeating(nameof(RunStateMachine), 0.0f, mAttributes.mBehaviourUpdateFrequency);
    }

    void RunStateMachine()
    {
        switch (mAttributes.mCurrentState)
        {
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
        mMovement.InfectedMovement();
    }

    void NonInfectedBehaviour()
    {
        mMovement.NonInfectedMovement();
    }
}
