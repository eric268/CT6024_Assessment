using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransitionScript : MonoBehaviour
{
    GameManagerScript mGameManager;
    AttributesScript mAttributes;

    float lerp = 0;
    [SerializeField]
    float mTimeForInfectionTransition = 3.0f;

    private void Start()
    {
        mGameManager = FindObjectOfType<GameManagerScript>();
        mAttributes = GetComponent<AttributesScript>();
    }

    public void BeginInfection()
    {
        StartCoroutine(InfectedTransiton());
        StartCoroutine(ChangeInfectedAppearance());
    }

    IEnumerator InfectedTransiton()
    {
        mGameManager.NewAgentInfected(gameObject);
        mAttributes.mInfected = true;
        mAttributes.mCurrentState = CurrentState.NEWLY_INFECTED;

        yield return new WaitForSeconds(mTimeForInfectionTransition);

        mAttributes.mCurrentState = CurrentState.INFECTED;
        Debug.Log("Infection Ended");
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
