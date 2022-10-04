using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeNode : Node
{
    MovementScript mMovementScript;
    public FleeNode(MovementScript _movementScript)
    {
        mMovementScript = _movementScript;
    }

    public override NodeState Evaluate()
    {
        mMovementScript.FleeFromEnemy();
        return NodeState.SUCCESS;
    }
}
