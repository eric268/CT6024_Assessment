using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PreyController : MonoBehaviour
{
    [Header("Neural Network")]
    [SerializeField]
    public int[] mNetworkLayerSizes;
    private NeuralNetwork mNeuralNetwork;

    [SerializeField]
    LayerMask mFoodLayerMask;
    Rigidbody mRigidBody;
    [SerializeField]
    public PreyAttributes mAttributes;

    private SensingManager mSensingManager;

    void Start()
    {
        mSensingManager = GetComponentInChildren<SensingManager>();
        mNetworkLayerSizes = new int[4] { 12, 8, 8, 5 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
        mRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //UpdateEnergyLevels();
        int result = mNeuralNetwork.RunNetwork(mSensingManager.GetNeuralNetworkInputs());
        Move(result);
    }

    private void Move(int result)
    {
        switch (result)
        {
            case 0:
                //No Movement
                break;
            case 1:
                //Move Forward
                mRigidBody.velocity = Vector3.forward;
                break;
            case 2:
                //Move Right
                mRigidBody.velocity = Vector3.right;
                break;
            case 3:
                //Move Backward
                mRigidBody.velocity = -Vector3.forward;
                break;
            case 4:
                //Move Left
                mRigidBody.velocity = -Vector3.right;
                break;
            default:
                Debug.LogError("Unexpected result PreyController move function");
                break;
        }

    }

    public List<double> GetVisionInput()
    {
        List<double> input = new List<double>();

        return input;
    }

    private void UpdateEnergyLevels()
    {
        if (mAttributes.mEnergyLevel <= 0)
        {
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
