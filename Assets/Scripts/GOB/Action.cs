using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AIGOAP
{
    //Describes the different types of GOB actions
    public enum ActionType
    {
        Hunt,
        Sleep,
        Reproduce,
        NUM_ACTION_TYPES
    }
    //Base class for GOB actions
    //Child class actions are Hunt, Sleep & Reproduce
    public abstract class Action
    {
        protected GOBScript mGOB;
        protected float mActionTimer;
        public float mActionDuration;
        public float[] mActionEffects;
        public Color mActionColor;
        public Action(GOBScript g, float e, float s, float r, float d)
        {
            mGOB = g;
            mActionEffects = new float[(int)ActionType.NUM_ACTION_TYPES];
            mActionEffects[(int)ActionType.Hunt] = e;
            mActionEffects[(int)ActionType.Sleep] = s;
            mActionEffects[(int)ActionType.Reproduce] = r;
            mActionDuration = d;
        }
        //Determined what the effect an action will have on goal discontentment
        public float GetGoalChanged(Goal goal)
        {
            switch (goal.mGoalTypes)
            {
                case GoalTypes.Eat:
                    return Mathf.Max(0,goal.mValue + mActionEffects[(int)ActionType.Hunt]);
                case GoalTypes.Sleep:
                    return Mathf.Max(0, goal.mValue + mActionEffects[(int)ActionType.Sleep]);
                case GoalTypes.Reproduce:
                    return Mathf.Max(0, goal.mValue + mActionEffects[(int)ActionType.Reproduce]);
                default:
                    return Mathf.Max(0, goal.mValue);
            }
        }
        public abstract IEnumerator BeginAction();
        public abstract void ResetAction();
        public abstract void StopAction();
    }
}
