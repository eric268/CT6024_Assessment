using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;



public class PredatorController : AgentController
{
    [SerializeField]
    public int mNumberGeneticPoints = 100;
    [SerializeField]
    public PredatorAttributes mAttributes;
    public GOBScript mGOB;
    public NavMeshAgent mNavMeshAgent;
    public GameObject mCurrentTarget;
    public PredatorSensing mSensingManager;
    public GameObject mMate;
    public PredatorGeneticManager mGeneticManager;

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
        mGeneticManager = new PredatorGeneticManager(mNumberGeneticPoints);
        Initalize();
    }

    private void Update()
    {
        Move(mCurrentTarget);
        UpdateEnergyLevels();
    }

    void Initalize()
    {
        mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        BindGeneticAttributesEvent();
        mGeneticManager.BroadcastAllAttributes();
    }

    public void ChangeAppearance(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    public void Move(GameObject target)
    {
        if (mCurrentTarget != null)
        {
            mNavMeshAgent.speed = mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].mAttribute * mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Sprint].mAttribute;
            
            if (target.CompareTag(mEnergyTag))
            {
                mNavMeshAgent.SetDestination(mCurrentTarget.transform.position + mCurrentTarget.transform.forward * 2.0f);
            }
            else
            {
                mNavMeshAgent.SetDestination(mCurrentTarget.transform.position);
            }
            
        }
        else
        {
            mNavMeshAgent.speed = mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].mAttribute;
            if (mSensingManager.IsFacingWall())
            {
                mNavMeshAgent.SetDestination(transform.position + (5.0f * -transform.forward));
            }
            else
            {
                mNavMeshAgent.SetDestination(transform.position + transform.forward * 5.0f);
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
        mGOB.mCurrentAction.StopAction();
        ObjectConsumed(prey.mAttributes.mEnergyGivenWhenEaten);
        prey.mAgentSpawner.ReturnAgentToPool(prey.gameObject);
        mGOB.mActionSuccessful = true;
        mGOB.SelectNewAction();
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
            mAttributes.mEnergyLevel -= Time.deltaTime *  mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Sprint].mAttribute;
        }
    }

    public void KillPredator()
    {
        StopAllCoroutines();
        mAgentSpawner.ReturnAgentToPool(gameObject);
    }

    public GameObject SpawnAgent(GameObject otherParent)
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        Debug.Assert(temp);
        temp.transform.position = transform.position - transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PredatorController controller = temp.GetComponent<PredatorController>();
        if (controller) 
        {
            controller.gameObject.name = "Newly Spawned Predator";
            controller.OnSpawn(this, otherParent);
        }
        
        return temp;
    }

    public void ResetAttributes()
    {
        mMate = null;
        mNavMeshAgent.isStopped = false;
        mAttributes.mMateFound = false;
        //mCurrentTarget = null;
    }

    void OnSpawn(PredatorController parent1Controller, GameObject parent2)
    {
        mGeneticManager = new PredatorGeneticManager(GetParentGeneticPoints(parent1Controller, parent2));
        LimitGeneticAttributes();
        Initalize();
        MutateGeneticAttributes();
        mGOB.Initalize();
        mGOB.SelectNewAction();
        mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            PreyController prey = collision.gameObject.GetComponent<PreyController>();
            if (prey)
            {
                if (mGOB.mCurrentAction != null && mGOB.mCurrentAction is HuntAction)
                {
                    OnPreyEatten(prey);
                }
            }
        }
    }

    void BindGeneticAttributesEvent()
    {
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.AngularSpeed].OnAttributeChanged += SetAngularSpeed;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.MateSensingRadius].OnAttributeChanged += mSensingManager.SetMateSensingRadius;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingAngle].OnAttributeChanged += mSensingManager.SetFarSensingAngle;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingRadius].OnAttributeChanged += mSensingManager.SetFarSensingRadius;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingAngle].OnAttributeChanged += mSensingManager.SetCloseSensingAngle;
        mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingRadius].OnAttributeChanged += mSensingManager.SetCloseSensingRadius;
    }

    void SetAngularSpeed(float speed)
    {
        mNavMeshAgent.angularSpeed = speed;
    }

    List<float> GetParentGeneticPoints(PredatorController parent1Controller, GameObject parent2)
    {
        PredatorController parent2Controller = parent2.GetComponent<PredatorController>();
        Debug.Assert(parent1Controller && parent2Controller);

        List<float> parent1GeneticPoints = parent1Controller.mGeneticManager.GetFirstHalfGenetics();
        List<float> parent2GeneticPoints = parent2Controller.mGeneticManager.GetSecondHalfGenetics();

        foreach (float f in parent2GeneticPoints)
        {
            parent1GeneticPoints.Add(f);
        }

        return parent1GeneticPoints;
    }

    void MutateGeneticAttributes()
    {
        //Want to randomly increase one attribute
        while(true)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.ImproveAttribute(randomAttribute))
            {
                break;
            }
        }
        //Want to randomly decrease one attribute as long as it's point count is greater than 1
        while(true)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.DeteriorateAttribute(randomAttribute))
            {
                break;
            }
        }


    }
    //When getting the attribute genes from both parents the total may add up to above or below what the max number of points should
    //This function will randomly select attributes to either add a point or take a point away from to ensure it is equal to mNumberGeneticPoints
    //Only one of these while loops will run depending on whether we need to add or subtract if it is equal to mNumberGeneticPoints neither will run
    void LimitGeneticAttributes()
    {
        while(mGeneticManager.mNumberOfStartingPoints < mNumberGeneticPoints)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.ImproveAttribute(randomAttribute))
            {
                mGeneticManager.mNumberOfStartingPoints++;
            }
        }

        while (mGeneticManager.mNumberOfStartingPoints > mNumberGeneticPoints)
        {
            int randomAttribute = Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            if (mGeneticManager.DeteriorateAttribute(randomAttribute))
            {
                mGeneticManager.mNumberOfStartingPoints--;
            }
        }
    }
}
