using System;
using Unity.VisualScripting;
using UnityEngine;

public class PreyController : AgentController
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;

    //[SerializeField]
    //LayerMask mFoodLayerMask;
   
    [SerializeField]
    public PreyAttributes mAttributes;
    FoodSpawnerScript mFoodSpawner;
    int prevResult;
    public bool spawn = true;
    public string mEnergyTag;
    public string mSpawnerTag;
    [SerializeField]
    public int mInputLayerSize;
    public double mEnergyConsumptionRate;

    [SerializeField]
    private float mNetworkUpdateRate = 0.05f;


    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mSensingManager = GetComponentInChildren<SensingManager>();
        mFoodSpawner = FindObjectOfType<FoodSpawnerScript>();
        mAgentSpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
        mNetworkLayerSizes = new int[4] { mInputLayerSize, 40,40, 3 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
    }

    private void Start()
    {
        //InvokeRepeating(nameof(UpdateNetwork), UnityEngine.Random.Range(0.01f, 0.25f), mNetworkUpdateRate);
    }

    private void Update()
    {
        UpdateEnergyLevels();
        int result = mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs(gameObject));
        Move(result);
    }

    private void FixedUpdate()
    {
        
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
        controller.mSensingManager = controller.GetComponentInChildren<SensingManager>();
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
        //Tuple<int, double>

        //mEnergyConsumptionRate = result.Item2;

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
        mRigidBody.velocity = transform.forward * (mAttributes.mSpeed);// * (float)mEnergyConsumptionRate);
    }

    private void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            mAgentSpawner.ReturnPreyToPool(gameObject);
        }
        else
        {
            mAttributes.mEnergyLevel -= Time.deltaTime;// * (float)mEnergyConsumptionRate;
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
            else
            {
                ObjectConsumed(15.0f);
                mAgentSpawner.ReturnPreyToPool(collision.gameObject);
            }
        }
    }

    protected override void ObjectConsumed(float foodVal)
    {
        mAttributes.mCurrentNumObjectsEaten++;
        mAttributes.mTotalFoodCollected++;
        if (mAttributes.mCurrentNumObjectsEaten >= mAttributes.mObjectsEattenToReproduce)
        {
            SpawnAgent();
            mAttributes.mCurrentNumObjectsEaten = 0;
        }
        mAttributes.mEnergyLevel += foodVal;
    }
}

