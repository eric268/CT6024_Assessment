using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    public class GOBScript : MonoBehaviour
    {
        [SerializeField]
        Action[] mActionArray;
        [SerializeField]
        Goal[] mGoalArray;
        PredatorAttributes predatorAttributes;

        private void Awake()
        {
            predatorAttributes = GetComponent<PredatorController>().mAttributes;
        }
        // Start is called before the first frame update
        void Start()
        {
            mActionArray = new Action[(int)ActionType.NUM_ACTION_TYPES];
            
            mActionArray[0] = new Action(ActionType.Eat, -5.0f, 3.0f, 1.0f, 2.0f);
            mActionArray[1] = new Action(ActionType.Sleep, 3.0f, -3.0f, 3.0f, -1.0f);
            mActionArray[2] = new Action(ActionType.Drink, 2.0f, 1.0f, -2.0f, 1.0f);
            mActionArray[3] = new Action(ActionType.Reproduce, 3.0f, 3.0f, 1.0f, -10.0f);

            mGoalArray = new Goal[(int)GoalTypes.Num_Goal_Types];
            mGoalArray[0] = new Goal(GoalTypes.Eat, 0.0f);
            mGoalArray[1] = new Goal(GoalTypes.Sleep, 0.0f);
            mGoalArray[2] = new Goal(GoalTypes.Drink, 0.0f);
            mGoalArray[3] = new Goal(GoalTypes.Reproduce, 0.0f);
        }

        void UpdateDiscontentmentValues()
        {
            mGoalArray[0].mValue += predatorAttributes.mHungerRate;
            mGoalArray[1].mValue += predatorAttributes.mFatigueRate;
            mGoalArray[2].mValue += predatorAttributes.mThirstRate;
            mGoalArray[3].mValue += predatorAttributes.mReproduceRate;
        }

        private void FixedUpdate()
        {
            UpdateDiscontentmentValues();
        }

        public Action GetAction()
        {
            Action action = ChooseAction(mActionArray, mGoalArray);
            for (int i =0; i < (int)ActionType.NUM_ACTION_TYPES; i++) 
            {
                mGoalArray[i].mValue += action.mActionEffects[i];
            }
            return action;
        }

        public Action ChooseAction(Action[] actionArr, Goal[] goalArr)
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
