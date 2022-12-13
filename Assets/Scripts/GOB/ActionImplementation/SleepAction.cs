using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepAction
{
    float mActionTimer;
    private PredatorController mController;
    public SleepAction(PredatorController c)
    {
        mController = c;
        ResetAction();
    }

    public void ResetAction()
    {
        mActionTimer = 0.0f;
    }

    public IEnumerator Sleep()
    {
        if (!mController.gameObject.activeInHierarchy)
            yield break;
        mController.mNavMeshAgent.isStopped = true;
        Debug.Log("Sleep");
        mController.mCurrentTarget = null;
        yield return new WaitForSeconds(mController.mGOB.mCurrentAction.mActionDuration);
        mController.mGOB.mActionSuccessful = true;
        mController.mGOB.ChooseAction();
    }
}
