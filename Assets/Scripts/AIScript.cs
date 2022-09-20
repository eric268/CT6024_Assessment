using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MovementStates
{
    Wander,
    Chase,
    NUM_OF_STATES
}

public class AIScript : MonoBehaviour
{
    NavMeshAgent agent;
    bool isInfected = false;
    GameObject target = null;
    MovementStates movementStates = MovementStates.Wander;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        RunStateMachine(movementStates);
    }

    void RunStateMachine(MovementStates states)
    {
        switch(states)
        {
            case MovementStates.Wander:
                Wander();
                break;
            case MovementStates.Chase:
                Chase();
                break;
        }
    }

    public void RecentlyInfected()
    {
        isInfected = true;


    }

    void Wander()
    {

    }

    void Chase()
    {

    }
}
