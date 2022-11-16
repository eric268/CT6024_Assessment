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

    [SerializeField]
    LayerMask mFoodLayerMask;
    public Rigidbody mRigidBody;
    [SerializeField]
    public PreyAttributes mAttributes;

    PreySpawner preySpawner;

    [SerializeField]
    int direction;

    private SensingManager mSensingManager;
    int prevResult;
    public bool spawn = true;


    private void Awake()
    {
        mNetworkLayerSizes = new int[3] { 16, 40, 3 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
        preySpawner = GameObject.FindWithTag("PreySpawner").GetComponent<PreySpawner>();
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
        controller.mAttributes.mEnergyLevel = mAttributes.mMaxEnergy;
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
        temp.transform.parent = gameObject.transform.parent;
        transform.position = transform.position - transform.forward * 2.5f;
        PreyController controller = temp.GetComponent<PreyController>();
        controller.mAttributes.mLearningRate = mAttributes.mLearningRate;
        controller.mAttributes.mEnergyLevel = mAttributes.mMaxEnergy;
        controller.mSensingManager = controller.GetComponentInChildren<SensingManager>();
        controller.mNeuralNetwork.CopyAndMutateNetwork(mNeuralNetwork.mNetworkLayers, controller.mAttributes.mLearningRate);

        temp.GetComponent<PreyController>().mAttributes.mTurnRate = temp.GetComponent<PreyController>().mAttributes.mTurnRate + rand.Next(-1, 1);
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
            /*
            case 0:
                mRigidBody.velocity = Vector3.forward * mAttributes.mSpeed;
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                break;
            case 1:
                //Move Right
                //transform.Rotate(0.0f,mAttributes.mTurnRate, 0.0f);
                mRigidBody.velocity = -Vector3.forward * mAttributes.mSpeed;
                transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                break;
            case 2:
                //transform.Rotate(0.0f, -mAttributes.mTurnRate, 0.0f);
                mRigidBody.velocity = Vector3.right * mAttributes.mSpeed;
                transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                break;
            case 3:
                mRigidBody.velocity = -Vector3.right * mAttributes.mSpeed;
                transform.eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
                break;
            */
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
            PreySpawner.ReturnPreyToPool(gameObject);
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
            if (cone.mSensingContainer.ContainsKey("Food"))
                cone.mSensingContainer[foodCollider.gameObject.tag].Remove(foodCollider);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            FoodScript fs = collision.gameObject.GetComponent<FoodScript>();
            if (fs)
            {
                FoodSpawnerScript.mCurrentAmountofFoodOnMap--;
                mAttributes.mCurrentFoodEaten++;
                mAttributes.mTotalFoodCollected++;
                if (mAttributes.mCurrentFoodEaten >= mAttributes.mFoodRequiredToReplicate)
                {
                    SplitPreyInstant();
                   //StartCoroutine(SplitPrey());
                    mAttributes.mCurrentFoodEaten = 0;
                }
                mAttributes.mEnergyLevel += fs.mEnergyAmount;
                FoodSpawnerScript.ReturnFood(collision.gameObject);
            }
            else
            {
                Debug.LogError("Item with food layer does not have food script attached");
            }
        }
    }
}

