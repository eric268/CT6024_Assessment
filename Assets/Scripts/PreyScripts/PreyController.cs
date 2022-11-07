using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class PreyController : MonoBehaviour
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    public NeuralNetwork mNeuralNetwork;

    [SerializeField]
    LayerMask mFoodLayerMask;
    Rigidbody mRigidBody;
    [SerializeField]
    public PreyAttributes mAttributes;

    private SensingManager mSensingManager;
    int prevResult;
    public bool spawn = true;

    [SerializeField]
    public float mTurnRate;

    private void Awake()
    {
        mNetworkLayerSizes = new int[3] { 13, 8, 5 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
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

    void SplitPrey()
    {
        if (GameManagerScript.currentNumberOfPrey >= GameManagerScript.maxNumberOfPrey)
            return;

        GameManagerScript.currentNumberOfPrey++;
        System.Random rand = new System.Random();
        Vector3 dir = (Vector3.zero - transform.position).normalized;
        GameObject temp = Instantiate(gameObject, transform.position - transform.forward * 0.5f,new Quaternion(0.0f,0.0f,0.0f,1.0f), gameObject.transform.parent);
        int rot = rand.Next(360);
        temp.transform.Rotate(0.0f, rot, 0.0f);
        PreyController controller = temp.GetComponent<PreyController>();
        controller.mAttributes.mEnergyLevel = mAttributes.mMaxEnergy;
        //controller.mAttributes.mLearningRate = mAttributes.mLearningRate;
        controller.mSensingManager = mSensingManager = GetComponentInChildren<SensingManager>();
        controller.mNeuralNetwork = this.mNeuralNetwork;

        Debug.Assert(controller != null && controller.mNeuralNetwork != null);

        foreach (NetworkLayer layer in controller.mNeuralNetwork.mNetworkLayers)
        {
            controller.mNeuralNetwork.UpdateWeightsAndBias(layer, controller.mAttributes.mLearningRate);
        }

    }

    void UpdateNetwork()
    {
        int result = mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs(gameObject));
        Move(result);
        //mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs(gameObject));
        //Move(mNeuralNetwork.mNetworkLayers[mNeuralNetwork.mNetworkLayers.Length - 1]);


    }

    //private void Move(NetworkLayer outputLayer)
    //{
    //    float val = (float)((outputLayer.mNeurons[0].mActivation > outputLayer.mNeurons[1].mActivation) ? -outputLayer.mNeurons[0].mActivation : outputLayer.mNeurons[1].mActivation);
    //    transform.Rotate(0.0f, val, 0.0f);
    //    mRigidBody.velocity = transform.forward * mAttributes.mSpeed;

    //}

    private void Move(int result)
    {
        switch (result)
        {
            case 0:
                //Move Forward
                mRigidBody.velocity = Vector3.forward * mAttributes.mSpeed;
                break;
            case 1:
                //Move Right
                mRigidBody.velocity = transform.right * mAttributes.mSpeed;
                //float amount = (float)mNeuralNetwork.mNetworkLayers[mNeuralNetwork.mNetworkLayers.Length - 1].mNeurons[1].mActivation;
                //transform.Rotate(0.0f, mTurnRate /** amount*/, 0.0f);
                break;
            case 2:
                mRigidBody.velocity = -transform.right * mAttributes.mSpeed;
                //float amount2 = (float)mNeuralNetwork.mNetworkLayers[mNeuralNetwork.mNetworkLayers.Length - 1].mNeurons[2].mActivation;
                //transform.Rotate(0.0f, -mTurnRate /** amount2*/, 0.0f);
                break;
                //case 3:
                //    //Move Left
                //    mRigidBody.velocity = -Vector3.right * mAttributes.mSpeed;
                //    transform.rotation = new Quaternion(0.0f, 270.0f, 0.0f, 1.0f);
                //    break;
                //default:
                //    Debug.LogError("Unexpected result PreyController move function");
                //    break;
            case 3:
                mRigidBody.velocity = (transform.right * 0.5f + transform.forward * 0.5f).normalized * mAttributes.mSpeed;
                break;
            case 4:
                mRigidBody.velocity = -(transform.right * 0.5f + transform.forward * 0.5f).normalized * mAttributes.mSpeed;
                break; ;
        }
        //mRigidBody.velocity = transform.forward * mAttributes.mSpeed;

    }

    private void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
            GameManagerScript.currentNumberOfPrey--;
            Destroy(gameObject);
        }
        else
        {
            mAttributes.mEnergyLevel -= Time.deltaTime;
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
                if (mAttributes.mCurrentFoodEaten >= mAttributes.mFoodRequiredToReplicate)
                {
                    SplitPrey();
                    mAttributes.mCurrentFoodEaten = 0;
                }
                mAttributes.mEnergyLevel += fs.mEnergyAmount;
                foreach (SensingVisionCone cone in mSensingManager.sensingVisionCones)
                {
                    if (cone.mSensingContainer.ContainsKey(collision.gameObject.tag))
                        cone.mSensingContainer[collision.gameObject.tag].Remove(collision.collider);
                }
                collision.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Item with food layer does not have food script attached");
            }
        }
    }
}
