using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class PreyController : AgentController
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;
   
    [SerializeField]
    public PreyAttributes mAttributes;
    FoodSpawnerScript mFoodSpawner;

    [SerializeField]
    public int mInputLayerSize;

    [SerializeField]
    private float mNetworkUpdateRate = 0.05f;

    PreySensing mSensingManager;

    private int mResult;


    private void Awake()
    {
        mEnergyTag = "Food";
        mRigidBody = GetComponent<Rigidbody>();
        mSensingManager = GetComponentInChildren<PreySensing>();
        mFoodSpawner = FindObjectOfType<FoodSpawnerScript>();
        mAgentSpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
        mNetworkLayerSizes = new int[4] { mInputLayerSize, 40, 40, 3 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
    }

    private void Start()
    {
        mAttributes.mLearningRate = UnityEngine.Random.Range(mAttributes.mlearningRateMin, mAttributes.mlearningRateMax);
        mAttributes.mTurnRate     = UnityEngine.Random.Range(mAttributes.mTurnRateStartMin, mAttributes.mTurnRateStartMax);
    }

    private void Update()
    {
        UpdateEnergyLevels();
        mResult = mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs(gameObject));
    }

    private void FixedUpdate()
    {
        Move(mResult);
    }

    protected override GameObject SpawnAgent()
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        if (temp == null)
            return null;

        System.Random rand = new System.Random();
        temp.transform.position = transform.position - transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PreyController controller = temp.GetComponent<PreyController>();
        controller.mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        controller.mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
        controller.mSensingManager = controller.GetComponentInChildren<PreySensing>();
        controller.mNeuralNetwork.CopyAndMutateNetwork(mNeuralNetwork.mNetworkLayers, controller.mAttributes.mLearningRate);

        temp.GetComponent<PreyController>().mAttributes.mTurnRate = mAttributes.mTurnRate + UnityEngine.Random.Range(-1, 2);
        temp.GetComponent<PreyController>().mAttributes.mLearningRate = mAttributes.mLearningRate + UnityEngine.Random.Range(-0.03f, 0.03f);
        Debug.Assert(controller != null && controller.mNeuralNetwork != null);

        return temp;
    }

    void UpdateNetwork()
    {
        int result = mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs(gameObject));
        Move(result);
    }

    private void Move(int result)
    {
        switch (result)
        {
            case 0:
                break;
            case 1:
                transform.Rotate(0.0f, mAttributes.mTurnRate, 0.0f);
                break;
            case 2:
                transform.Rotate(0.0f, -mAttributes.mTurnRate, 0.0f);
                break;

        }
        mRigidBody.velocity = transform.forward * (mAttributes.mSpeed);
    }

    protected override void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            mAgentSpawner.ReturnAgentToPool(gameObject);
        }
        else
        {
            mAttributes.mEnergyLevel -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(mEnergyTag))
        {
            FoodScript fs = collision.gameObject.GetComponent<FoodScript>();
            if (fs)
            {
                FoodSpawnerScript.mCurrentAmountofFoodOnMap--;
                ObjectConsumed(fs.mEnergyAmount);
                mFoodSpawner.ReturnFood(collision.gameObject);
            }
        }
    }

    protected override void ObjectConsumed(float foodVal)
    {
        mAttributes.mCurrentNumObjectsEaten++;
        mAttributes.mTotalObjectsEatten++;
        if (mAttributes.mCurrentNumObjectsEaten >= mAttributes.mObjectsEattenToReproduce)
        {
            SpawnAgent();
            mAttributes.mCurrentNumObjectsEaten = 0;
        }
        mAttributes.mEnergyLevel += foodVal;
    }
}

