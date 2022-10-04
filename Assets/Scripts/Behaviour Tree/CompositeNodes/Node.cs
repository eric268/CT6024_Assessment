using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    SUCCESS,
    FAILURE,
    RUNNING,
    NUM_NODE_STATES
}
[System.Serializable]
public abstract class Node 
{
    protected NodeState nodeState;
    public abstract NodeState Evaluate();
}
