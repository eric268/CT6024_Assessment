using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateTransitionScript : MonoBehaviour
{
    GameManagerScript mGameManager;
    AttributesScript mAttributes;
    NavMeshAgent mAgent;

    float lerp = 0;
    [SerializeField]
    float mTimeForInfectionTransition = 3.0f;

    private void Awake()
    {
        mGameManager = FindObjectOfType<GameManagerScript>();
        mAttributes = GetComponent<AttributesScript>();
        mAgent = GetComponent<NavMeshAgent>();
    }

    public void InfectedTransition()
    {
        if (mAgent)
        {
            mAgent.isStopped = true;
        }
        StartCoroutine(InfectedTransiton());
        StartCoroutine(ChangeInfectedAppearance());
    }

    IEnumerator InfectedTransiton()
    {
        mGameManager.NewAgentInfected(gameObject);
        mAttributes.mInfected = true;
        mAttributes.mCurrentState = CurrentState.NEWLY_INFECTED;
        mAgent.speed = AttributesScript.mInfectedMovementSpeed;

        yield return new WaitForSeconds(mTimeForInfectionTransition);

        mAttributes.mCurrentState = CurrentState.INFECTED;
        mAgent.isStopped = false;
    }



    IEnumerator ChangeInfectedAppearance()
    {
        Renderer rend = GetComponent<Renderer>();
        while (lerp < 1.0f)
        {
            lerp += Time.deltaTime / mTimeForInfectionTransition;
            rend.material.Lerp(mAttributes.mNonInfectedMaterial, mAttributes.mInfectedMaterial, lerp);
            yield return null;
        }
        yield return 0;
    }
}
