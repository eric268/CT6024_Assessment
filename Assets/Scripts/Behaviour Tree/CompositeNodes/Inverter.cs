using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Inverter : Node
    {
        protected Node node;

        public Inverter(Node node)
        {
            this.node = node;
        }

        public override NodeState Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    nodeState = NodeState.RUNNING;
                    break;
                case NodeState.SUCCESS:
                    nodeState = NodeState.FAILURE;
                    break;
                case NodeState.FAILURE:
                    nodeState = NodeState.SUCCESS;
                    break;
                default:
                    break;

            }
            return nodeState;
        }
    }

