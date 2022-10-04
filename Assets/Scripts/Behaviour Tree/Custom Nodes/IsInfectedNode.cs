using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInfectedNode : Node
{
    private AttributesScript mAttributes;
    public IsInfectedNode(AttributesScript _attributes)
    {
        mAttributes = _attributes;
    }
    public override NodeState Evaluate()
    {
        return (mAttributes.mCurrentState == CurrentState.INFECTED) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
