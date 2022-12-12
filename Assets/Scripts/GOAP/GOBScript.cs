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
        PredatorController mPredatorController;
        public Action mCurrentAction;
        public bool mActionSuccessful = false;

        private void Awake()
        {
            mPredatorController = GetComponent<PredatorController>();
        }
        // Start is called before the first frame update
        void Start()
        {
            Initalize();
        }
        private void LateUpdate()
        {
            UpdateDiscontentmentValues();
        }

        public void Initalize()
        {
            mActionArray = new Action[(int)ActionType.NUM_ACTION_TYPES];
            mActionArray[0] = new Action(ActionType.Hunt, -5.0f, 3.0f, 2.0f, 5.0f);
            mActionArray[1] = new Action(ActionType.Sleep, 3.0f, -3.0f, -1.0f, 2.0f);
            mActionArray[2] = new Action(ActionType.Reproduce, 3.0f, 3.0f, -10.0f, 4.0f);

            mGoalArray = new Goal[(int)GoalTypes.Num_Goal_Types];
            mGoalArray[0] = new Goal(GoalTypes.Eat, 0.0f, 1.0f);
            mGoalArray[1] = new Goal(GoalTypes.Sleep, 0.0f, 0.5f);
            mGoalArray[2] = new Goal(GoalTypes.Reproduce, 0.0f, 0.5f);

            ChooseAction();
        }

        void UpdateDiscontentmentValues()
        {
            for (int i =0; i < mGoalArray.Length; i++) 
            {
                mGoalArray[i].UpdateValue(Time.deltaTime);
            }
        }

        public void OnActionComplete()
        {
            for (int i = 0; i < (int)ActionType.NUM_ACTION_TYPES; i++)
            {
                mGoalArray[i].mValue += mCurrentAction.mActionEffects[i];
            }
        }

        public Action ChooseAction()
        {
            //Call stop coroutine because action maybe complete earlier than expected like when hunting for instance
            if (mActionSuccessful)
            {
                OnActionComplete();
                mActionSuccessful = false;
            }
            mCurrentAction = ChooseAction(mActionArray, mGoalArray);
            mPredatorController.ImplementAction(mCurrentAction.mActionTypes);
            return mCurrentAction;
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

            for (int i =0; i < (int)GoalTypes.Num_Goal_Types; i++)
            {
                float value = goals[i].mValue + action.GetGoalChanged(goals[i]);
                value += action.mActionDuration * goals[i].mChange;
                discontentment += goals[i].GetDiscontentment(value);
            }

            return discontentment;
        }
    }
}
