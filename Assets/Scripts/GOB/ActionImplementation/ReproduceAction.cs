using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIGOAP;

//Class which controls the reproduce GOB predator action
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
    //Resets the classes variables to inital values
    public override void ResetAction()
    {
        mActionTimer = 0.0f;
        mReproduceTimer = 0.0f;
        mRecentlyReproduced = false;
    }

    //Starts the predators search for a mate.
    //Predators can only mate with predators that are also in a mating action
    //FindClosestMate will only return a GameObject that is also performing a mate action
    public override IEnumerator BeginAction()
    {
        mController.mAttributes.mIsLookingForMate = true;
        mActionColor = Color.white;
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (!mController.gameObject.activeInHierarchy)
                yield break;

            mActionTimer += Time.deltaTime;
            mController.mCurrentTarget = mController.mSensingManager.FindClosestMate();

            //Checks to see if we have another predator that is also performing mate action
            //If the two predators are withing the mating radius they will begin to reproduce
            if (mController.mCurrentTarget != null && Vector3.Distance(mController.mCurrentTarget.transform.position, mController.transform.position) < mController.mAttributes.mMatingDistance)
            {
                mController.mMate = mController.mCurrentTarget;
                //Called the coroutine which delays reproduction
                //After the timer has expired a new agent will be created
                foreach (var item in Reproduce())
                {
                    yield return item;
                }
                mGOB.SelectNewAction();
                yield break;
            }
            yield return null;
        }
        mGOB.SelectNewAction();
    }
    //Coroutine which begins timer for spawning a new character
    IEnumerable Reproduce()
    {
        //When within the mating distance both agents will stop moving
        mController.mNavMeshAgent.isStopped = true;
        mController.mNavMeshAgent.velocity = Vector3.zero;

        //Ensures that the agent hasn't died during reproduction and if so breaks out of the coroutine
        while (mReproduceTimer < mController.mAttributes.mTimeToReproduce)
        {
            if (!mController.gameObject.activeInHierarchy || mRecentlyReproduced)
                yield break;

            mReproduceTimer += Time.deltaTime;
            yield return null;
        }
        //Once reproduction is successful a new agent is spawned and a new agent is spawned
        mController.SpawnAgent(mController.mMate);
        mGOB.mActionSuccessful = true;
    }

    public override void StopAction()
    {
        mRecentlyReproduced = true;
    }
}
