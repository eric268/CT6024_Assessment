using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    public class GOBScript : MonoBehaviour
    {
        Action[] mActionArray;
        Goal[] mGoalArray;
        // Start is called before the first frame update
        void Start()
        {
            mActionArray = new Action[3];
            mGoalArray = new Goal[3];
        }

        // Update is called once per frame
        void Update()
        {

        }
        Action ChooseAction(Action[] actionArr, Goal[] goalArr)
        {
            Action bestAction = null;
            float bestValue = float.PositiveInfinity;

            foreach (Action action in actionArr)
            {
                float thisValue = Discontentment(action, goalArr);
                if (thisValue < bestValue)
                {
                    bestValue = thisValue;
                    bestAction = action;
                }
            }
            return bestAction;
        }

        float Discontentment(Action action, Goal[] goals)
        {
            float discontentment = 0.0f;

            foreach (Goal goal in goals)
            {
                float value = goal.mValue + action.GetGoalChanged(goal);
                discontentment += value;
            }

            return discontentment;
        }
    }
}
