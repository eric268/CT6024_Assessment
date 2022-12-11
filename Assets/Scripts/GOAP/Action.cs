using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AIGOAP
{
    public enum ActionType
    {
        Eat,
        Sleep,
        Drink,
        Reproduce,
        NUM_ACTION_TYPES
    }

    public class Action
    {
        public ActionType mActionTypes;

        public float mEatEffect;
        public float mSleepEffect;
        public float mReproduceEffect;
        public float mFightEffect;
        public float mThirstEffect;

        public float[] mActionEffects;
        public Action(ActionType a, float e, float s, float t, float r)
        {
            mActionTypes = a;
            mEatEffect = e;
            mSleepEffect = s;
            mThirstEffect = t;
            mReproduceEffect = r;

            mActionEffects = new float[(int)ActionType.NUM_ACTION_TYPES];
            mActionEffects[0] = mEatEffect;
            mActionEffects[1] = mSleepEffect;
            mActionEffects[2] = mThirstEffect;
            mActionEffects[3] = mReproduceEffect;
        }
        public float GetGoalChanged(Goal goal)
        {
            switch (goal.mGoalTypes)
            {
                case GoalTypes.Eat:
                    return Mathf.Max(0,goal.mValue + mEatEffect);
                case GoalTypes.Sleep:
                    return Mathf.Max(0, goal.mValue + mSleepEffect);
                case GoalTypes.Drink:
                    return Mathf.Max(0, goal.mValue + mThirstEffect);
                case GoalTypes.Reproduce:
                    return Mathf.Max(0, goal.mValue + mReproduceEffect);
                default:
                    return Mathf.Max(0, goal.mValue);
            }
        }
    }
}
