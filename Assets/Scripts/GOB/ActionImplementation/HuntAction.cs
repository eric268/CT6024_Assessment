using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;

//Class for implementing the predators hunt GOB action
public class HuntAction : Action
{
    private PredatorController mController;
    public bool mRecentlyEatten;
    public HuntAction(PredatorController c,GOBScript g, float e, float s, float r, float d) : base(g,e,s,r,d)
    {
        mController = c;
        ResetAction();
    }

    //Resets the actions variables to intial values
    public override void ResetAction()
    {
        mActionTimer = 0.0f;
        mRecentlyEatten = false;
    }
    //Beings the action. If the time expires before the action is completed the action is set as a failure
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
        //When timer has expired a new action will be selected
        mGOB.SelectNewAction();
    }
    //Breaks out of the current action
    //This is usually called if the action is completed before the timer.
    //For example the predator hunts for 4 seconds at a time. If the predator catches a prey agent before then it should choose a new action (which may be to hunt again)
    public override void StopAction()
    {
        mRecentlyEatten = true;
        mGOB.mActionSuccessful = true;
        mGOB.StopAllCoroutines();
    }
}
