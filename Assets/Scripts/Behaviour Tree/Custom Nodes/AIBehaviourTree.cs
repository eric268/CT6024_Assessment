using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviourTree : MonoBehaviour
{
    AttributesScript mAttributesScript;
    StateTransitionScript mStateTransitionScript;
    MovementScript mMovementScript;
    Selector mTopNode;

    [SerializeField]
    float mBehaviourTreeUpdateFreq = 0.05f;
    // Start is called before the first frame update

    private void Awake()
    {
        mAttributesScript = GetComponent<AttributesScript>();   
        mStateTransitionScript = GetComponent<StateTransitionScript>();
        mMovementScript = GetComponent<MovementScript>();
    }
    void Start()
    {
        ConstructBehaviourTree();
        InvokeRepeating(nameof(RunBehaviourTree), 0.0f, mBehaviourTreeUpdateFreq);
    }

    public void ConstructBehaviourTree()
    {
        //Newely-infected Behaviour
        BeginInfectionNode beginInfectionNode = new BeginInfectionNode(mAttributesScript, mStateTransitionScript);

        //Non-infected Behaviour
        WanderNode wanderNode = new WanderNode(mMovementScript);
        FleeNode fleeNode = new FleeNode(mMovementScript);
        CheckEnemyCloseNode checkEnemyCloseNode = new CheckEnemyCloseNode(mMovementScript);
        StateCheckNode isNotInfectedNode = new StateCheckNode(mAttributesScript, CurrentState.NOT_INFECTED);
        Sequencer nonInfectedFleeSequencer = new Sequencer(new List<Node> { checkEnemyCloseNode, fleeNode });
        Selector nonInfectedMovementSequencer = new Selector(new List<Node> { nonInfectedFleeSequencer, wanderNode });
        Sequencer nonInfectedBehaviourSelector = new Sequencer(new List<Node> {isNotInfectedNode, nonInfectedMovementSequencer});

        //InfectedBehaviour
        FindTargetNode findTargetNode = new FindTargetNode(mMovementScript);
        ChaseTarget chaseTargetNode = new ChaseTarget(mMovementScript);
        StateCheckNode isInfectedNode = new StateCheckNode(mAttributesScript, CurrentState.INFECTED);
        Sequencer infectedBehaviourSequencer = new Sequencer(new List<Node> { isInfectedNode, findTargetNode, chaseTargetNode });

        mTopNode = new Selector(new List<Node> { infectedBehaviourSequencer, nonInfectedBehaviourSelector, beginInfectionNode });
    }
    private void RunBehaviourTree()
    {
        mTopNode.Evaluate();
    }
}
