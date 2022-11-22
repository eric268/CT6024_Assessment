using UnityEngine;

public enum AgentType
{
    Prey,
    Predator,
    NUM_OF_AGENTS
}


public class AgentController : MonoBehaviour
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;

    //[SerializeField]
    //LayerMask mFoodLayerMask;
    public Rigidbody mRigidBody;
    [SerializeField]
    public AgentAttributes mAttributes;

    AgentSpawner mAgentSpawner;
    FoodSpawnerScript mFoodSpawner;

    [SerializeField]
    int direction;

    private SensingManager mSensingManager;
    int prevResult;
    public bool spawn = true;

    public string mEnergyTag;
    public string mSpawnerTag;

    [SerializeField]
    public int mInputLayerSize;

    public AgentType mAgentType;

    public bool mDebugDuplicate = false;


    private void Awake()
    {
        //mNetworkLayerSizes = new int[3] { 28, 40, 3 };
        mNetworkLayerSizes = new int[4] { mInputLayerSize, 40,40, 3 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
        mSensingManager = GetComponentInChildren<SensingManager>();
        mRigidBody = GetComponent<Rigidbody>();
        if (mAgentType == AgentType.Prey)
        {
            InitalizeIfPrey();
        }
        else
        {
            InitalizeIfPredator();
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        UpdateEnergyLevels();
        UpdateNetwork();

        if (mDebugDuplicate)
        {
            mDebugDuplicate = false;
            SplitPreyInstant();
        }
    }

    public GameObject SplitPreyInstant()
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        if (temp == null)
            return null;

        System.Random rand = new System.Random();
        temp.transform.position = transform.position  -transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        AgentController controller = temp.GetComponent<AgentController>();
        controller.mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        controller.mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
        controller.mSensingManager = controller.GetComponentInChildren<SensingManager>();
        controller.mNeuralNetwork.CopyAndMutateNetwork(mNeuralNetwork.mNetworkLayers, controller.mAttributes.mLearningRate);


        temp.GetComponent<AgentController>().mAttributes.mTurnRate =  mAttributes.mTurnRate + Random.Range(-1, 2);
        temp.GetComponent<AgentController>().mAttributes.mLearningRate = mAttributes.mLearningRate + Random.Range(-0.03f, 0.03f);
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
        mRigidBody.velocity = transform.forward * mAttributes.mSpeed;
    }

    private void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            mAgentSpawner.ReturnPreyToPool(gameObject);
        }
        else
        {
            mAttributes.mEnergyLevel -= Time.deltaTime;
        }
    }

    public void RemoveFoodFromSensing(Collider foodCollider)
    {
        foreach (SensingVisionCone cone in mSensingManager.sensingVisionCones)
        {
            if (cone.mSensingContainer.ContainsKey(mEnergyTag))
                cone.mSensingContainer[foodCollider.gameObject.tag].Remove(foodCollider);
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
                EnergyConsumed(fs.mEnergyAmount);
                mFoodSpawner.ReturnFood(collision.gameObject);
            }
            else
            {
                EnergyConsumed(15.0f);
                mAgentSpawner.ReturnPreyToPool(collision.gameObject);
            }
        }
        //TODO
        //Need to remove prey from all collider containers
    }

    private void EnergyConsumed(float foodVal)
    {
        mAttributes.mCurrentFoodEaten++;
        mAttributes.mTotalFoodCollected++;
        if (mAttributes.mCurrentFoodEaten >= mAttributes.mFoodRequiredToReplicate)
        {
            SplitPreyInstant();
            mAttributes.mCurrentFoodEaten = 0;
        }
        mAttributes.mEnergyLevel += foodVal;
    }

    void InitalizeIfPrey()
    {
        mAgentSpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
        mFoodSpawner = FindObjectOfType<FoodSpawnerScript>();
        string name = mAgentSpawner.gameObject.name;
    }

    void InitalizeIfPredator()
    {
        mAgentSpawner = GameObject.FindGameObjectWithTag("PredatorSpawner").GetComponent<AgentSpawner>();
        string name = mAgentSpawner.gameObject.name;
    }
}

