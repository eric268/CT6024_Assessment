using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AIGOAP
{
    public enum ActionType
    {
        Hunt,
        Sleep,
        Reproduce,
        NUM_ACTION_TYPES
    }

    public class Action
    {
        public ActionType mActionTypes;

        public float mEatEffect;
        public float mSleepEffect;
        public float mReproduceEffect;
        public float mActionDuration;

        public float[] mActionEffects;
        public Action(ActionType a, float e, float s, float r, float d)
        {
            mActionTypes = a;
            mEatEffect = e;
            mSleepEffect = s;
            mReproduceEffect = r;
            mActionDuration = d;

            mActionEffects = new float[(int)ActionType.NUM_ACTION_TYPES];
            mActionEffects[0] = mEatEffect;
            mActionEffects[1] = mSleepEffect;
            mActionEffects[2] = mReproduceEffect;
        }
        public float GetGoalChanged(Goal goal)
        {
            switch (goal.mGoalTypes)
            {
                case GoalTypes.Eat:
                    return Mathf.Max(0,goal.mValue + mEatEffect);
                case GoalTypes.Sleep:
                    return Mathf.Max(0, goal.mValue + mSleepEffect);
                case GoalTypes.Reproduce:
                    return Mathf.Max(0, goal.mValue + mReproduceEffect);
                default:
                    return Mathf.Max(0, goal.mValue);
            }
        }
    }
}
