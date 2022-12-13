using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;

public class HuntAction : Action
{
    private PredatorController mController;
    public bool mRecentlyEatten;
    public HuntAction(PredatorController c,GOBScript g, float e, float s, float r, float d) : base(g,e,s,r,d)
    {
        mController = c;
        ResetAction();
    }

    public override void ResetAction()
    {
        mActionTimer = 0.0f;
        mRecentlyEatten = false;
    }
    public override IEnumerator BeginAction()
    {
        mActionColor = Color.red;
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration && !mRecentlyEatten)
        {
            if (!mController.gameObject.activeInHierarchy)
                yield break;

            mActionTimer += Time.deltaTime;
            mController.mCurrentTarget = mController.mSensingManager.FindClosestPrey();
            yield return null;
        }
        //If this section is reached it means no food was found
        mGOB.SelectNewAction();
    }

    public override void StopAction()
    {
        mRecentlyEatten = true;
        mGOB.mActionSuccessful = true;
        mGOB.StopAllCoroutines();
    }
}
