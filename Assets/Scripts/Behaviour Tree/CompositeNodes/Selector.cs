using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    protected List<Node> childNodes = new List<Node>();
    public Selector(List<Node> nodes) {
        {
            childNodes = nodes;
        }
    }

    public override NodeState Evaluate()
        {
            //bool isAnyNodeRunning = false;

            foreach (Node node in childNodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.RUNNING:
                        nodeState = NodeState.RUNNING;
                        return nodeState;
                    case NodeState.SUCCESS:
                        nodeState = NodeState.SUCCESS;
                        return nodeState;
                    case NodeState.FAILURE:
                        break;
                    default:
                        break;
                }
            }
            nodeState = NodeState.FAILURE;
            return nodeState;
        }
}
