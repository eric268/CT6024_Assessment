using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AIGOAP
{
    public class Action
    {
        public float mEatEffect;
        public float mSleepEffect;
        public float mReproduce;
        public float mFightEffect;
        public Action(float e, float s, float r)
        {
            mEatEffect = e;
            mSleepEffect = s;
            mReproduce = r;
        }
        public float GetGoalChanged(Goal goal)
        {
            switch (goal.mGoalTypes)
            {
                case GoalTypes.Eat:
                    return goal.mValue + mEatEffect;
                case GoalTypes.Sleep:
                    return goal.mValue + mSleepEffect;
                case GoalTypes.Drink:
                    return goal.mValue + mReproduce;
                default:
                    return goal.mValue;
            }
        }
    }
}
