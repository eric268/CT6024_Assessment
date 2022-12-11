using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PredatorController : AgentController
{
    [SerializeField]
    private PredatorAttributes mAttributes;
    private GOBScript mGOB;
    private NavMeshAgent mNavMeshAgent;
    private GameObject mCurrentTarget;
    PredatorSensing mSensingManager;

    private void Awake()
    {
        mEnergyTag = "Prey";
        mRigidBody= GetComponent<Rigidbody>();
        mGOB = GetComponent<GOBScript>();
        mNavMeshAgent= GetComponent<NavMeshAgent>();
        mSensingManager = GetComponentInChildren<PredatorSensing>();
    }

    private void Update()
    {

    }

    void SetCurrentTarget()
    {
        mCurrentTarget = mSensingManager.FindClosestObject();
    }

    protected override void ObjectConsumed(float val)
    {
        mAttributes.mTotalObjectsEatten++;
        mAttributes.mEnergyLevel += val;
        mAttributes.mFatigue++;
    }

    protected override GameObject SpawnAgent()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            PreyController prey = collision.gameObject.GetComponent<PreyController>();
            if (prey)
            {
                ObjectConsumed(prey.mAttributes.mEnergyGivenWhenEaten);
                prey.mAgentSpawner.ReturnAgentToPool(collision.gameObject);
            }
        }
    }
}
