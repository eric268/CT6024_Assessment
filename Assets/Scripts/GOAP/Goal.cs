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
        Reproduce,
        Num_Goal_Types
    }

    public class Goal
    {
        public GoalTypes mGoalTypes;
        public float mValue;

        public Goal(GoalTypes type, float v)
        {
            mGoalTypes = type;
            mValue = v;
        }
        public float GetDiscomfort()
        {
            return mValue * mValue;
        }

    }
}
