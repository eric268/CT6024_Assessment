using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;



public class PredatorController : AgentController
{
    [SerializeField]
    public PredatorAttributes mAttributes;
    public GOBScript mGOB;
    public NavMeshAgent mNavMeshAgent;
    public GameObject mCurrentTarget;
    PredatorSensing mSensingManager;
    public delegate GameObject FindTargetDelegate();
    public FindTargetDelegate FFindPreyTarget;
    public FindTargetDelegate FFindMateTarget;

    public GameObject mMate;
    private HuntAction mHuntAction;
    private SleepAction mSleepAction;
    private ReproduceAction mReproduceAction;

    private void Awake()
    {
        mAgentSpawner = GameObject.FindGameObjectWithTag("PredatorSpawner").GetComponent<AgentSpawner>();
        mEnergyTag = "Prey";
        mRigidBody= GetComponent<Rigidbody>();
        mGOB = GetComponent<GOBScript>();
        mNavMeshAgent= GetComponent<NavMeshAgent>();
        mSensingManager = GetComponentInChildren<PredatorSensing>();
        mNavMeshAgent.speed = mAttributes.mSpeed;
    }

    private void Start()
    {
        Initalize();
    }

    private void Update()
    {
        UpdateEnergyLevels();
        if (mCurrentTarget!= null) 
        {
            Debug.DrawLine(mCurrentTarget.transform.position, transform.position, Color.green, Time.deltaTime);
        }
    }

    void Initalize()
    {
        FFindPreyTarget = mSensingManager.FindClosestPrey;
        FFindMateTarget = mSensingManager.FindClosestMate;
        mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        mHuntAction = new HuntAction(this);
        mSleepAction = new SleepAction(this);
        mReproduceAction = new ReproduceAction(this);
    }

    void ChangeAppearance(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    public void ImplementAction(ActionType action)
    {
        ResetAttributes();
        switch (action)
        {
            case ActionType.Hunt:
                ChangeAppearance(Color.red);
                mHuntAction.ResetAction();
                StartCoroutine(mHuntAction.Hunt());
                break;
            case ActionType.Sleep:
                ChangeAppearance(Color.blue);
                mSleepAction.ResetAction();
                StartCoroutine(mSleepAction.Sleep());
                break;
            case ActionType.Reproduce:
                mAttributes.mLookingForMateEnergyMultiplier = 0.7f;
                ChangeAppearance(Color.white);
                mReproduceAction.ResetAction();
                StartCoroutine(mReproduceAction.SearchForMate());
                break;
        }
        
    }

    public void Move(FindTargetDelegate targetSelection)
    {
        mCurrentTarget = targetSelection();
        if (mCurrentTarget)
        {
            mNavMeshAgent.SetDestination(mCurrentTarget.transform.position);
        }
        else
        {
            if (mSensingManager.IsFacingWall())
            {
                mNavMeshAgent.SetDestination(transform.position + (5.0f * -transform.forward));
            }
            else
            {
                mNavMeshAgent.SetDestination(transform.position + (5.0f * transform.forward));
            }
        }
    }

    protected override void ObjectConsumed(float val)
    {
        mAttributes.mTotalObjectsEatten++;
        mAttributes.mEnergyLevel += val;

        //Change Goal Values
    }

    void OnPreyEatten(PreyController prey)
    {
        mHuntAction.mRecentlyEatten = true;
        ObjectConsumed(prey.mAttributes.mEnergyGivenWhenEaten);
        prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
        mGOB.mActionSuccessful = true;
        mGOB.ChooseAction();
    }

    protected override void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            KillPredator();
        }
        else
        {
            // Lose energy in relation to sprinting or not
            //Lose less energy when looking for mate
            mAttributes.mEnergyLevel -= Time.deltaTime * mAttributes.mSprintMultiplier * mAttributes.mLookingForMateEnergyMultiplier;
        }
    }

    public void KillPredator()
    {
        StopAllCoroutines();
        mAgentSpawner.ReturnAgentToPool(gameObject);
    }

    public GameObject SpawnAgent(GameObject p1, GameObject p2)
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        if (temp == null)
            return null;

        temp.transform.position = transform.position - transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PredatorController controller = temp.GetComponent<PredatorController>();
        if (controller) 
        {
            controller.Initalize();
            controller.mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
        }

        GOBScript gob = temp.GetComponent<GOBScript>();
        if (gob)
        {
            gob.Initalize();
            gob.ChooseAction();
        }
        mGOB.mActionSuccessful = true;
        mReproduceAction.mRecentlyReproduced = true;
        mGOB.ChooseAction();
        return temp;
    }

    private void ResetAttributes()
    {
        mMate = null;
        mAttributes.mLookingForMateEnergyMultiplier = 1.0f;
        mAttributes.mSprintMultiplier = 1.0f;
        mNavMeshAgent.isStopped = false;
        mAttributes.mMateFound = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            PreyController prey = collision.gameObject.GetComponent<PreyController>();
            if (prey)
            {
                if (mGOB.mCurrentAction != null && mGOB.mCurrentAction.mActionTypes == ActionType.Hunt)
                {
                    OnPreyEatten(prey);
                }
            }
        }
    }
}
