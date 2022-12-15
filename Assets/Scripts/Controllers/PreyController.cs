using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

//Controller for prey agent
public class PreyController : AgentController
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;
    [SerializeField]
    public PreyAttributes mAttributes;
    FoodSpawnerScript mFoodSpawner;
    PreySensing mSensingManager;
    private void Awake()
    {
        mEnergyTag = "Food";
        mRigidBody = GetComponent<Rigidbody>();
        mSensingManager = GetComponentInChildren<PreySensing>();
        mFoodSpawner = FindObjectOfType<FoodSpawnerScript>();
        mAgentSpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
        mNetworkLayerSizes = new int[4] { 40, 55,55, 3 };
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
        Move(RunNetwork());
    }

    private int RunNetwork()
    {
        List<double> networkInputs = mSensingManager.GetNeuralNetworkInputs(gameObject);
        return  mNeuralNetwork.RunNetwork(networkInputs);
    }

    private void OnSpawn(PreyController parent)
    {
        transform.position = parent.transform.position - transform.forward * 2.5f;
        transform.parent = parent.gameObject.transform.parent;
        mAttributes.mEnergyLevel = mAttributes.mStartingEnergy;
        mAttributes.mCurrentGeneration = parent.mAttributes.mCurrentGeneration + 1;
        mSensingManager = GetComponentInChildren<PreySensing>();
        mAttributes.mTurnRate = parent.mAttributes.mTurnRate + UnityEngine.Random.Range(-2, 2);
        mAttributes.mTurnRate = Mathf.Clamp(mAttributes.mTurnRate, mAttributes.mTurnRateStartMin, mAttributes.mTurnRateStartMax);
        mAttributes.mLearningRate = parent.mAttributes.mLearningRate + UnityEngine.Random.Range(-0.03f, 0.03f);
        mNeuralNetwork.CopyAndMutateNetwork(parent.mNeuralNetwork.mNetworkLayers, mAttributes.mLearningRate);
    }

    //Called when a prey agent has eaten enough food items to reproduce
    protected GameObject SpawnAgent()
    {
        GameObject temp = mAgentSpawner.SpawnAgent(gameObject);
        //Need null check because if max number of prey agents have been spawned SpawnAgent() will return a null GameObject
        if (temp == null)
            return null;

        if (temp.TryGetComponent(out PreyController controller))
        {
            controller.OnSpawn(this);
        }
        else
        {
            return null;
        }
        return temp;
    }
    //Move takes in the neuron from the neural network output layer with the highest activation
    //This dictates whether the agent should continue moving in it's current direction, turn left or turn right
    private void Move(int result)
    {
        switch (result)
        {
            case 0:
                //Intentionally left blank since neural network has decided moving straight is correct
                break;
            case 1:
                transform.Rotate(0.0f, mAttributes.mTurnRate, 0.0f);
                break;
            case 2:
                transform.Rotate(0.0f, -mAttributes.mTurnRate, 0.0f);
                break;

        }
        //Agent always moves forward. The network simply determines which direction it should move in
        mRigidBody.velocity = transform.forward * (mAttributes.mSpeed);
    }
    //Checks if the prey agent has run out of energy. If so agent is killed and returned to pool
    //Otherwise energy is reduced 
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
    //Checks if prey agent has collided with food, if so it updates it's energy and returns the food item to the food object pool
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
    //Updates some attributes when food is consumed. 
    //If enough food items have been eaten the agent reproduces 
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

