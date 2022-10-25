using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AIGOAP
{
    public class Action
    {
        public float mEatEffect;
        public float mSleepEffect;
        public float mDrinkEffect;
        public Action(float e, float s, float d)
        {
            mEatEffect = e;
            mSleepEffect = s;
            mDrinkEffect = d;
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
                    return goal.mValue + mDrinkEffect;
                default:
                    return goal.mValue;
            }
        }
    }
}
