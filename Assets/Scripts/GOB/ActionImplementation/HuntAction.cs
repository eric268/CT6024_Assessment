using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;

public class HuntAction
{
    private PredatorController mController;
    private float mActionTimer;
    public bool mRecentlyEatten;
    public HuntAction(PredatorController c)
    {
        mController = c;
        ResetAction();
    }

    public void ResetAction()
    {
        mActionTimer = 0.0f;
        mRecentlyEatten = false;
    }
    public IEnumerator Hunt()
    {
        while (mActionTimer < mController.mGOB.mCurrentAction.mActionDuration)
        {
            if (mRecentlyEatten || !mController.gameObject.activeInHierarchy)
                yield break;
            mActionTimer += Time.deltaTime;
            mController.Move(mController.FFindPreyTarget);
            if (mController.mCurrentTarget)
            {
                mController.mAttributes.mSprintMultiplier = 1.25f;
                mController.mNavMeshAgent.speed = mController.mAttributes.mSpeed * mController.mAttributes.mSprintMultiplier;
            }
            else
            {
                mController.mAttributes.mSprintMultiplier = 1.0f;
            }
            Debug.Log("Hunt");
            yield return null;
        }
        //If this section is reached it means no food was found
        mController.mGOB.mActionSuccessful = false;
        mController.mGOB.ChooseAction();
    }
}
