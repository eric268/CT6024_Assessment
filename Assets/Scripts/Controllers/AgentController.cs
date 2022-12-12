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
    public AgentSpawner mAgentSpawner;
    public AgentType mAgentType;
    protected string mEnergyTag;

    protected abstract void UpdateEnergyLevels();
    protected abstract GameObject SpawnAgent();
    protected abstract void ObjectConsumed(float val);
}
