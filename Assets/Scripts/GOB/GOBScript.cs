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

        public void Initalize()
        {
            mActionArray = new Action[(int)ActionType.NUM_ACTION_TYPES];
            mActionArray[0] = new HuntAction(mPredatorController, this, -10.0f, 2.0f, 4.5f, 4.0f);
            mActionArray[1] = new SleepAction(mPredatorController, this,  5.0f, -5.0f, 1.0f, 2.0f);
            mActionArray[2] = new ReproduceAction(mPredatorController,this, 3.5f, 2.5f, -15.0f, 7.0f);

            mGoalArray = new Goal[(int)GoalTypes.Num_Goal_Types];
            mGoalArray[0] = new Goal(GoalTypes.Eat, UnityEngine.Random.Range(0.0f,5.0f), 1.0f);
            mGoalArray[1] = new Goal(GoalTypes.Sleep, UnityEngine.Random.Range(0.0f, 5.0f), 0.5f);
            mGoalArray[2] = new Goal(GoalTypes.Reproduce, UnityEngine.Random.Range(0.0f, 5.0f), 0.25f);
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
                mGoalArray[i].mValue = Mathf.Max(0, mGoalArray[i].mValue);
            }
        }

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
