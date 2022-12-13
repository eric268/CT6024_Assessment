using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;
public class ReproduceAction : Action
{
    PredatorController mController;
    float mReproduceTimer;
    public bool mRecentlyReproduced;

    public ReproduceAction(PredatorController c, GOBScript g, float e, float s, float r, float d) : base(g,e,s,r,d)
    {
        mController = c;
        ResetAction();
    }  
    
    public override void ResetAction()
    {
        mActionTimer = 0.0f;
        mReproduceTimer = 0.0f;
        mRecentlyReproduced = false;
    }

    public override IEnumerator BeginAction()
    {
        mActionColor = Color.white;
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (!mController.gameObject.activeInHierarchy)
                yield break;

            mActionTimer += Time.deltaTime;
            mController.mCurrentTarget = mController.mSensingManager.FindClosestMate();
            if (mController.mCurrentTarget != null && Vector3.Distance(mController.mCurrentTarget.transform.position, mController.transform.position) < mController.mAttributes.mMatingDistance)
            {
                mController.mMate = mController.mCurrentTarget;

                foreach (var item in Reproduce(mController.mCurrentTarget, mController.gameObject))
                {
                    yield return item;
                }
                //mGOB.StopCoroutine(nameof(Reproduce));
                mGOB.SelectNewAction();
                yield break;
            }
            yield return null;
        }
        mGOB.SelectNewAction();
        //If this section is reached it means that no mate was found
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
        mGOB.mActionSuccessful = true;
    }

    public override void StopAction()
    {
        mRecentlyReproduced = true;
    }
}
