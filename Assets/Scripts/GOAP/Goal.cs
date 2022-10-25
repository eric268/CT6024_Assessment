using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    public enum GoalTypes
    {
        Eat,
        Sleep,
        Drink,
        Num_Goal_Types
    }

    public class Goal
    {
        public GoalTypes mGoalTypes;
        public float mValue;

        public Goal(float v)
        {
            mValue = v;
        }
        public float GetDiscomfort()
        {
            return mValue * mValue;
        }

    }
}
