using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class PredatorController : AgentController
{
    [SerializeField]
    public PredatorAttributes mAttributes;
    private GOBScript mGOB;
    private NavMeshAgent mNavMeshAgent;
    private GameObject mCurrentTarget;
    PredatorSensing mSensingManager;
    public delegate GameObject FindTargetDelegate();
    public FindTargetDelegate FFindPreyTarget;
    public FindTargetDelegate FFindMateTarget;

    float mActionTimer = 0.0f;
    [SerializeField]
    float mMatingDistance = 1.0f;
    bool mRecentlyEatten = false;
    bool mRecentlyReproduced = false;

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
    }

    public void ImplementAction(ActionType action)
    {
        mActionTimer = 0.0f;
        mAttributes.mSprintMultiplier = 1.0f;
        switch (action)
        {
            case ActionType.Hunt:
                mRecentlyEatten = false;
                StartCoroutine(Hunt());
                break;
            case ActionType.Sleep:
                StartCoroutine(Sleep());
                break;
            case ActionType.Reproduce:
                mRecentlyReproduced = false;
                StartCoroutine(Reproduce());
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

    IEnumerator Hunt()
    {
        mNavMeshAgent.enabled = true;
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (mRecentlyEatten)
                yield break;
            mActionTimer += Time.deltaTime;
            Move(FFindPreyTarget);
            if (mCurrentTarget)
            {
                mAttributes.mSprintMultiplier = 1.5f;
                mNavMeshAgent.speed = mAttributes.mSpeed * mAttributes.mSprintMultiplier;
            }
            else
            {
                mAttributes.mSprintMultiplier = 1.0f;
            }
            Debug.Log("Hunt");
            yield return null;
        }
        //If this section is reached it means no food was found
        mGOB.mActionSuccessful = false;
        mGOB.ChooseAction();
    }

    IEnumerator Sleep()
    {
        mNavMeshAgent.enabled = false;
        Debug.Log("Sleep");
        mCurrentTarget = null;
        yield return new WaitForSeconds(mGOB.mCurrentAction.mActionDuration);
        mGOB.mActionSuccessful= true;
        mGOB.ChooseAction();
    }

    IEnumerator Reproduce()
    {
        mNavMeshAgent.enabled = true;
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (mRecentlyReproduced)
                yield break;
            mActionTimer += Time.deltaTime;
            if (mCurrentTarget != null && Vector3.Distance(mCurrentTarget.transform.position, transform.position) < mMatingDistance)
            {
                mNavMeshAgent.enabled = false;
                SpawnAgent();
            }
            else
            {
                Move(FFindMateTarget);
            }
            Debug.Log("Reproduce");
            yield return null;
        }
        //If this section is reached it means that no mate was found
        mGOB.mActionSuccessful = false;
        mGOB.ChooseAction();
    }

    protected override void ObjectConsumed(float val)
    {
        mAttributes.mTotalObjectsEatten++;
        mAttributes.mEnergyLevel += val;

        //Change Goal Values
    }

    void OnPreyEatten(PreyController prey)
    {
        mRecentlyEatten = true;
        ObjectConsumed(prey.mAttributes.mEnergyGivenWhenEaten);
        prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
        mGOB.mActionSuccessful = true;
        mGOB.ChooseAction();
    }

    protected override void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            mAgentSpawner.ReturnAgentToPool(gameObject);
        }
        else
        {
            //Lose energy in relation to sprinting or not
            mAttributes.mEnergyLevel -= Time.deltaTime * mAttributes.mSprintMultiplier;
        }
    }

    protected override GameObject SpawnAgent()
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        if (temp == null)
            return null;

        System.Random rand = new System.Random();
        temp.transform.position = transform.position - transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PredatorController controller = temp.GetComponent<PredatorController>();
        if (controller) 
        {
            controller.Initalize();
        }
        GOBScript gob = temp.GetComponent<GOBScript>();
        if (gob)
        {
            gob.Initalize();
        }
        mRecentlyReproduced = true;
        mGOB.mActionSuccessful = true;
        mGOB.ChooseAction();
        return temp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            PreyController prey = collision.gameObject.GetComponent<PreyController>();
            if (prey)
            {
                if (mGOB.mCurrentAction.mActionTypes == ActionType.Hunt)
                {
                    OnPreyEatten(prey);
                }
                else
                {
                    prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
                }
            }
        }
    }
}
