using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTarget : Node
{
    MovementScript mMovementScript;
    public ChaseTarget(MovementScript _movementScript)
    {
        mMovementScript = _movementScript;
    }
    public override NodeState Evaluate()
    {
        mMovementScript.ChaseAgent();
        return NodeState.SUCCESS;
    }
}
