using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    //Describes the different types of GOB goals
    public enum GoalTypes
    {
        Eat,
        Sleep,
        Reproduce,
        Num_Goal_Types
    }
    //Class which contains the discontentment value of each goal
    //Three types of goals: to eat, sleep and reproduce
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
        //Increases the discontentment of a goal overtime based on it's increase rate
        public void UpdateValue(float deltaTime)
        {
            mValue += mChange * deltaTime;
        }
        //Returns the discontentment squared to ensure that as discontentment increases the agent responds more urgently to higher numbers
        public float GetDiscontentment(float val)
        {
            return val * val;
        }

    }
}
