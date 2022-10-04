using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyCloseNode : Node
{
    MovementScript mMovementScript;
    public CheckEnemyCloseNode(MovementScript _movementScript)
    {
        mMovementScript = _movementScript;
    }

    public override NodeState Evaluate()
    {
        return (mMovementScript.CheckForCloseInfectedAgent() != null) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
