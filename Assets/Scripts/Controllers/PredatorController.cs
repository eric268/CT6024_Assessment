using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        mGeneticManager = new PredatorGeneticManager(100);
        BindGeneticAttributesEvent();
        mGeneticManager.BroadcastAllAttributes();
    }

    public void ChangeAppearance(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    public void Move(GameObject target)
    {
        if (mCurrentTarget)
        {
            mNavMeshAgent.speed = mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].mAttribute * mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Sprint].mAttribute;
            mNavMeshAgent.SetDestination(mCurrentTarget.transform.position);
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
                mNavMeshAgent.SetDestination(transform.forward);
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
            gob.SelectNewAction();
        }
        return temp;
    }

    public void ResetAttributes()
    {
        mMate = null;
        mNavMeshAgent.isStopped = false;
        mAttributes.mMateFound = false;
        mCurrentTarget = null;
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
}
