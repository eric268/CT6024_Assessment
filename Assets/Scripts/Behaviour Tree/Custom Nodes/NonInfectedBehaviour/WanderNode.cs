using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderNode : Node
{
    MovementScript mMovementComponent;

    public WanderNode(MovementScript _movementComponent)
    {
        mMovementComponent = _movementComponent;
    }

    public override NodeState Evaluate()
    {
        mMovementComponent.Wander();
        return NodeState.SUCCESS;
    }
}
