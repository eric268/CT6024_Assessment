using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;

//Class which controls predators sleeping GOB action
public class SleepAction : Action
{
    private PredatorController mController;
    public SleepAction(PredatorController c, GOBScript g, float e, float s, float r, float d) : base(g, e,s,r,d)
    {
        mController = c;
        ResetAction();
    }

    public override void ResetAction()
    {
        //Do not need anything here as of right now as there are not variables to reset.
    }
    //Coroutine which begins the timer for the sleeping action
    //There is no possibility of this action ending early or failing so will always be successful
    public override IEnumerator BeginAction()
    {
        if (!mController.gameObject.activeInHierarchy)
            yield break;

        mActionColor = Color.blue;
        mController.mNavMeshAgent.isStopped = true;
        mController.mCurrentTarget = null;
        yield return new WaitForSeconds(mController.mGOB.mCurrentAction.mActionDuration);
        mGOB.mActionSuccessful = true;
        mGOB.StopAllCoroutines();
        mGOB.SelectNewAction();
    }

    public override void StopAction()
    {
        //Do not need anything here as of right now because this action cannot be completed early and therefore stops itself
    }
}
