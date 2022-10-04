using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetNode : Node
{
    MovementScript mMovementScript;
    public FindTargetNode(MovementScript _movementScript)
    {
        this.mMovementScript = _movementScript;
    }
    public override NodeState Evaluate()
    {
        return (mMovementScript.FindTarget() != null) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
