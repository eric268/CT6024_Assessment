using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginInfectionNode : Node
{
    AttributesScript mAttributesScript;
    StateTransitionScript mStateTransitionScript;
    bool mTransitionStarted;
    public BeginInfectionNode(AttributesScript _attributesScript, StateTransitionScript _stateTransitionScript)
    {
        mAttributesScript = _attributesScript;
        mStateTransitionScript = _stateTransitionScript;
        mTransitionStarted = false;
    }
    public override NodeState Evaluate()
    {
        if (!mTransitionStarted)
        {
            mTransitionStarted = true;
            mStateTransitionScript.InfectedTransition();
        }
        return NodeState.SUCCESS;
    }
}
