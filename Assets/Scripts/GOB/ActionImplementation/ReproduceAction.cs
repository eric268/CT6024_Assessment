using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproduceAction 
{
    PredatorController mController;
    float mActionTimer;
    float mReproduceTimer;
    public bool mRecentlyReproduced;

    public ReproduceAction(PredatorController c)
    {
        mController = c;
        ResetAction();
    }  
    
    public void ResetAction()
    {
        mActionTimer = 0.0f;
        mReproduceTimer = 0.0f;
        mRecentlyReproduced = false;
    }

    public IEnumerator SearchForMate()
    {
        while (mActionTimer < mController.mGOB.mCurrentAction.mActionDuration)
        {
            if (!mController.gameObject.activeInHierarchy || mRecentlyReproduced)
                yield break;
            mActionTimer += Time.deltaTime;
            if (mController.mCurrentTarget != null && Vector3.Distance(mController.mCurrentTarget.transform.position, mController.gameObject.transform.position) < mController.mAttributes.mMatingDistance)
            {
                mController.mMate = mController.mCurrentTarget;
                mController.mAttributes.mMateFound = true;
                foreach (var item in Reproduce(mController.mCurrentTarget, mController.gameObject))
                {
                    yield return null;
                }
            }
            else
            {
                mController.Move(mController.FFindMateTarget);
            }
            Debug.Log("Reproduce");
            yield return null;
        }
        //If this section is reached it means that no mate was found
        mController.mGOB.mActionSuccessful = false;
        mController.mGOB.ChooseAction();
    }

    IEnumerable Reproduce(GameObject p1, GameObject p2)
    {
        mController.mNavMeshAgent.isStopped = true;
        mController.mNavMeshAgent.velocity = Vector3.zero;
        while (mReproduceTimer < mController.mAttributes.mTimeToReproduce)
        {
            if (!mController.gameObject.activeInHierarchy || mRecentlyReproduced)
                yield break;

            mReproduceTimer += Time.deltaTime;
            yield return null;
        }
        mController.SpawnAgent(p1, p2);
    }
}
