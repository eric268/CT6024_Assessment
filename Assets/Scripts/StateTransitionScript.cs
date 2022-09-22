using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionScript : MonoBehaviour
{
    GameManagerScript mGameManager;
    AttributesScript mAttributes;

    private void Start()
    {
        mGameManager = FindObjectOfType<GameManagerScript>();
        mAttributes = GetComponent<AttributesScript>();
    }

    public void InfectedTransiton()
    {
        mGameManager.NewAgentInfected(gameObject);
        mAttributes.mInfected = true;
        GetComponent<MeshRenderer>().material = mAttributes.mInfectedMaterial;
        mAttributes.mCurrentState = CurrentState.INFECTED;
    }
}
