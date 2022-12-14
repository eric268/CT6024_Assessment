using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the types of agents that can exist in the simulation
public enum AgentType
{
    Prey,
    Predator,
    NUM_OF_AGENTS
}
//Base class for all Agent's controller.
//Child classes consist of PreyController and PredatorController
public abstract class AgentController : MonoBehaviour
{
    public Rigidbody mRigidBody;
    public AgentSpawner mAgentSpawner;
    public AgentType mAgentType;
    protected string mEnergyTag;

    //Abstract class for updating the energy level of the agents.
    //Energy level is what determines when an agent dies
    protected abstract void UpdateEnergyLevels();
    //Abstract class for when an agent consumes an object
    protected abstract void ObjectConsumed(float val);
}
