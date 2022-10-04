using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCheckNode : Node
{
    private AttributesScript mAttributes;
    private CurrentState mState;
    public StateCheckNode(AttributesScript mAttributes, CurrentState mState)
    {
        this.mAttributes = mAttributes;
        this.mState = mState;
    }
    public override NodeState Evaluate()
    {
        return (mAttributes.mCurrentState == mState) ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
