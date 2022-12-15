using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIGOAP
{
    //Class which manages the GOB of the predator agent
    public class GOBScript : MonoBehaviour
    {
        [SerializeField]
        Action[] mActionArray;
        [SerializeField]
        public Goal[] mGoalArray;
        PredatorController mPredatorController;
        public Action mCurrentAction;
        public bool mActionSuccessful = false;

        private void Awake()
        {
            mPredatorController = GetComponent<PredatorController>();
            Initalize();
        }

        private void Start()
        {
            SelectNewAction();
        }

        private void Update()
        {
            UpdateDiscontentmentValues();
        }
        //Initializes the goals and actions of the GOB
        public void Initalize()
        {
            mActionArray = new Action[(int)ActionType.NUM_ACTION_TYPES];
            mActionArray[0] = new HuntAction(mPredatorController, this, -5.0f, 3.5f, 3.0f, 4.0f);
            mActionArray[1] = new SleepAction(mPredatorController, this,  2.5f, -4.0f, 1.5f, 2.5f);
            mActionArray[2] = new ReproduceAction(mPredatorController,this, 3.5f, 2.5f, -6.0f, 8.0f);

            mGoalArray = new Goal[(int)GoalTypes.Num_Goal_Types];
            mGoalArray[0] = new Goal(GoalTypes.Eat, UnityEngine.Random.Range(0.0f,4.0f), 0.5f);
            mGoalArray[1] = new Goal(GoalTypes.Sleep, UnityEngine.Random.Range(0.0f, 3.0f), 0.25f);
            mGoalArray[2] = new Goal(GoalTypes.Reproduce, UnityEngine.Random.Range(0.0f, 2.0f), 0.15f);
        }
        //Updates all of the goal discontentment values over time
        void UpdateDiscontentmentValues()
        {
            for (int i =0; i < mGoalArray.Length; i++) 
            {
                mGoalArray[i].UpdateValue(Time.deltaTime);
            }
        }
        //If an action is complete then the resulting effects on goal discontentment are updated
        //This function will not be called on failed actions and therefore not effect from the action will be felt
        //The discontentment is still updated over time by its discontentment increase rate
        public void OnActionComplete()
        {
            for (int i = 0; i < (int)ActionType.NUM_ACTION_TYPES; i++)
            {
                mGoalArray[i].mValue += mCurrentAction.mActionEffects[i];
                mGoalArray[i].mValue = Mathf.Max(0, mGoalArray[i].mValue);
            }
        }

        //Updates results of the previous action
        //Resets values for next action
        public void SelectNewAction()
        {
            if (mActionSuccessful)
            {
                OnActionComplete();
            }
            if (mCurrentAction != null)
            {
                mCurrentAction.ResetAction();
            }
            
            mActionSuccessful = false;
            mPredatorController.ResetAttributes();
            mCurrentAction = ChooseAction(mActionArray, mGoalArray);
            StartCoroutine(mCurrentAction.BeginAction());
            mPredatorController.ChangeAppearance(mCurrentAction.mActionColor);
            
        }
        //Returns a new action that will lead to lowest overall discontentment
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
        //Calculates the effect of an action on each goals discontentment level
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
