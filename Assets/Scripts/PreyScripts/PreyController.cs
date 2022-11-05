using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    public PreyAttributes mAttributes;

    void Start()
    {
        mNetworkLayerSizes = new int[4] { 12, 8, 8, 5 };
        mNeuralNetwork = new NeuralNetwork(mNetworkLayerSizes);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateEnergyLevels();
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
        if (mFoodLayerMask == (mFoodLayerMask | 1 << collision.gameObject.layer))
        {
            FoodScript fs = collision.gameObject.GetComponent<FoodScript>();
            if (fs)
            {
                mAttributes.mEnergyLevel += fs.mEnergyAmount;
                collision.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Item with food layer does not have food script attached");
            }
        }
    }
}
