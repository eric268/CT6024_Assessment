using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsNewelyInfectedNode : Node
{
    private AttributesScript mAttributes;
    public IsNewelyInfectedNode(AttributesScript _attributes)
    {
        mAttributes = _attributes;
    }
    public override NodeState Evaluate()
    {
        return (mAttributes.mCurrentState == CurrentState.NEWLY_INFECTED) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
