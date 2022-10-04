using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : Node
{
    protected List<Node> childNode = new List<Node>();
    public Sequencer(List<Node> node) {
        {
            childNode = node;
        }
    }
    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;

        foreach (Node node in childNode)
        {
            switch(node.Evaluate())
            {
                case NodeState.RUNNING:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.SUCCESS:
                    break;
                case NodeState.FAILURE:
                    nodeState = NodeState.FAILURE;
                    return nodeState;
                default: 
                break;
            }
        }
        nodeState = isAnyNodeRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return nodeState;
    }
}
