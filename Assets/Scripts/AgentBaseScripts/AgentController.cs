using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentType
{
    Prey,
    Predator,
    NUM_OF_AGENTS
}

public abstract class AgentController : MonoBehaviour
{
    public Rigidbody mRigidBody;
    protected AgentSpawner mAgentSpawner;
    protected SensingManager mSensingManager;
    public AgentType mAgentType;
    protected abstract GameObject SpawnAgent();
    protected abstract void ObjectConsumed(float val);
}
