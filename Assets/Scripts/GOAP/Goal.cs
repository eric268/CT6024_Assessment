using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    public enum GoalTypes
    {
        Eat,
        Sleep,
        Reproduce,
        Num_Goal_Types
    }

    public class Goal
    {
        public GoalTypes mGoalTypes;
        public float mValue;
        public float mChange;

        public Goal(GoalTypes type, float v, float c)
        {
            mGoalTypes = type;
            mValue = v;
            mChange = c;
        }
        public void UpdateValue(float deltaTime)
        {
            mValue += mChange * deltaTime;
        }
        public float GetDiscontentment(float val)
        {
            return val * val;
        }

    }
}
