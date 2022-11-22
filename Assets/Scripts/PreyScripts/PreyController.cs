using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEditor.PlayerSettings;

public class PreyController : MonoBehaviour
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;

    //[SerializeField]
    //LayerMask mFoodLayerMask;
    public Rigidbody mRigidBody;
    [SerializeField]
    public PreyAttributes mAttributes;

    GameObject mSpawner;
    AgentSpawner preySpawner;
    AgentSpawner predatorSpawner;
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


    private void Awake()
    {
        //mNetworkLayerSizes = new int[3] { 28, 40, 3 };
        mNetworkLayerSizes = new int[4] { mInputLayerSize, 40,40, 3 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
        mSpawner = GameObject.FindGameObjectWithTag("Spawner");
        mFoodSpawner = mSpawner.transform.GetChild(0).GetComponent<FoodSpawnerScript>();
        preySpawner = mSpawner.transform.GetChild(1).GetComponent<AgentSpawner>();
        predatorSpawner = mSpawner.transform.GetChild(2).GetComponent<AgentSpawner>();

    }

    void Start()
    {
        mSensingManager = GetComponentInChildren<SensingManager>();
        mRigidBody = GetComponent<Rigidbody>();
        //mAttributes.mLearningRate = Random.value * mLearningRate;
        //mTurnRate *= Random.value;
    }
    private void Update()
    {
        UpdateEnergyLevels();
        UpdateNetwork();
    }

    private void OnEnable()
    {
        //InvokeRepeating(nameof(UpdateNetwork), Random.value, 0.05f);
    }

    public IEnumerator SplitPrey()
    {
        Vector3 pos = transform.position - transform.forward * 5.0f;
        yield return new WaitForSeconds(1.5f);
        GameObject temp = preySpawner.SpawnAgent(gameObject);
        if (temp == null)
            yield break;

        System.Random rand = new System.Random();
        temp.transform.position = pos;
        temp.transform.parent = gameObject.transform.parent;
        PreyController controller = temp.GetComponent<PreyController>();
        controller.mAttributes.mLearningRate = mAttributes.mLearningRate;
        controller.mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        controller.mSensingManager = controller.GetComponentInChildren<SensingManager>();
        controller.mNeuralNetwork.CopyAndMutateNetwork(mNeuralNetwork.mNetworkLayers, controller.mAttributes.mLearningRate);
        temp.GetComponent<PreyController>().mAttributes.mTurnRate = temp.GetComponent<PreyController>().mAttributes.mTurnRate + rand.Next(-1, 1);
        Debug.Assert(controller != null && controller.mNeuralNetwork != null);
        yield return null;
    }

    public GameObject SplitPreyInstant()
    {
        GameObject temp = preySpawner.SpawnAgent(gameObject);
        if (temp == null)
            return null;

        System.Random rand = new System.Random();
        temp.transform.position = transform.position  -transform.forward * 2.5f;
        temp.transform.parent = gameObject.transform.parent;
        PreyController controller = temp.GetComponent<PreyController>();
        controller.mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        controller.mAttributes.mCurrentGeneration = mAttributes.mCurrentGeneration + 1;
        controller.mSensingManager = controller.GetComponentInChildren<SensingManager>();
        controller.mNeuralNetwork.CopyAndMutateNetwork(mNeuralNetwork.mNetworkLayers, controller.mAttributes.mLearningRate);


        temp.GetComponent<PreyController>().mAttributes.mTurnRate =  mAttributes.mTurnRate + Random.Range(-1, 2);
        temp.GetComponent<PreyController>().mAttributes.mLearningRate = mAttributes.mLearningRate + Random.Range(-0.03f, 0.03f);
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
            preySpawner.ReturnPreyToPool(gameObject);
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
                preySpawner.ReturnPreyToPool(collision.gameObject);
            }
        }
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
}

