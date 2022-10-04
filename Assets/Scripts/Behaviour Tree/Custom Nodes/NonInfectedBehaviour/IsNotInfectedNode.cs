using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsNotInfectedNode : Node
{
    AttributesScript mAttributeScript;
    public IsNotInfectedNode(AttributesScript _attributeScript)
    {
        mAttributeScript = _attributeScript;
    }

    public override NodeState Evaluate()
    {
        return (mAttributeScript.mCurrentState == CurrentState.NOT_INFECTED) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
