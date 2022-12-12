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
    private GOBScript mGOB;
    private NavMeshAgent mNavMeshAgent;
    private GameObject mCurrentTarget;
    PredatorSensing mSensingManager;
    public delegate GameObject FindTargetDelegate();
    public FindTargetDelegate FFindPreyTarget;
    public FindTargetDelegate FFindMateTarget;

    float mActionTimer = 0.0f;
    float mReproduceTimer = 0.0f;
    [SerializeField]
    float mMatingDistance = 1.0f;
    bool mRecentlyEatten = false;
    bool mRecentlyReproduced = false;
    GameObject mMate;

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
                mRecentlyEatten = false;
                StartCoroutine(Hunt());
                break;
            case ActionType.Sleep:
                ChangeAppearance(Color.blue);
                StartCoroutine(Sleep());
                break;
            case ActionType.Reproduce:
                mAttributes.mLookingForMateEnergyMultiplier = 0.7f;
                ChangeAppearance(Color.white);
                mRecentlyReproduced = false;
                StartCoroutine(SearchForMate());
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
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (mRecentlyEatten || !gameObject.activeInHierarchy)
                yield break;
            mActionTimer += Time.deltaTime;
            Move(FFindPreyTarget);
            if (mCurrentTarget)
            {
                mAttributes.mSprintMultiplier = 1.25f;
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
        if (!gameObject.activeInHierarchy)
            yield break;
        mNavMeshAgent.isStopped = true;
        Debug.Log("Sleep");
        mCurrentTarget = null;
        yield return new WaitForSeconds(mGOB.mCurrentAction.mActionDuration);
        mGOB.mActionSuccessful= true;
        mGOB.ChooseAction();
    }

    IEnumerator SearchForMate()
    {
        while (mActionTimer < mGOB.mCurrentAction.mActionDuration)
        {
            if (!gameObject.activeInHierarchy || mRecentlyReproduced)
                yield break;
            mActionTimer += Time.deltaTime;
            if (mCurrentTarget != null && Vector3.Distance(mCurrentTarget.transform.position, transform.position) < mMatingDistance)
            {
                mMate = mCurrentTarget;
                mAttributes.mMateFound = true;
                foreach(var item in Reproduce(mCurrentTarget, gameObject))
                {
                    yield return null;
                }
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

    IEnumerable Reproduce(GameObject p1, GameObject p2)
    {
        mNavMeshAgent.isStopped = true;
        mNavMeshAgent.velocity = Vector3.zero;
        while (mReproduceTimer < mAttributes.mTimeToReproduce)
        {
            if (!gameObject.activeInHierarchy || mRecentlyReproduced)
                yield break;

            mReproduceTimer += Time.deltaTime;
            yield return null;
        }
        SpawnAgent(p1,p2);
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

    protected GameObject SpawnAgent(GameObject p1, GameObject p2)
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
        mRecentlyReproduced = true;
        mGOB.mActionSuccessful = true;
        mGOB.ChooseAction();
        return temp;
    }

    private void ResetAttributes()
    {
        mMate = null;
        mAttributes.mLookingForMateEnergyMultiplier = 1.0f;
        mActionTimer = 0.0f;
        mAttributes.mSprintMultiplier = 1.0f;
        mNavMeshAgent.isStopped = false;
        mReproduceTimer = 0.0f;
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
                else
                {
                    //prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
                }
            }
        }
    }
}
