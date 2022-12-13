using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.XR;

public class InputData
{
    public float distance;
    public float xPos;
    public float zPos;
    public InputData()
    {
        distance = 0.0f;
        xPos = 0.0f;
        zPos = 0.0f;
    }
    public InputData(float d, float x, float z)
    {
        float distance = d;
        float xPos = x;
        float zPos = z;
    }
}

public class PreySensing : SensingScript
{
    List<double> inputData;
    [SerializeField]
    LayerMask mFoodLayerMask;
    [SerializeField]
    LayerMask mPredatorLayerMask;
    [SerializeField]
    LayerMask mWallLayerMask;
    LayerMask[] mSensingLayerMasks;


    private void Awake()
    {
        mSensingLayerMasks = new LayerMask[3];
        mSensingLayerMasks[0] = mFoodLayerMask;
        mSensingLayerMasks[1] = mPredatorLayerMask;
        mSensingLayerMasks[2] = mWallLayerMask;

        inputData = new List<double>();
        sensingVisionCones = GetComponentsInChildren<VisionCone>();
    }

    public List<double> GetNeuralNetworkInputs(GameObject agent)
    {
        inputData.Clear();
        SetDirectionNetworkInput(agent, inputData);
        //Iterating through every vision cone
        for (int i = 0; i < sensingVisionCones.Length; i++)
        {
            //Getting all objects that are within field of view and match layer mask
            foreach (LayerMask mask in mSensingLayerMasks)
            {
                List<GameObject> objectsWithVision = sensingVisionCones[i].GetObjectsWithinVision(mask);
                GetInputForClosestObject(inputData, objectsWithVision, sensingVisionCones[i].mRadius);
            }
        }
        return inputData;
    }

    void SetDirectionNetworkInput(GameObject agent, List<double> inputData)
    {
        //4 inputs
        PreyController controller = agent.GetComponent<PreyController>();
        Vector2 tempV = new Vector2(Mathf.Abs(controller.mRigidBody.velocity.x), Mathf.Abs(controller.mRigidBody.velocity.z));
        tempV.Normalize();

        if (controller.mRigidBody.velocity.x < 0)
        {
            inputData.Add(0.0f);
            inputData.Add(tempV.x);
        }
        else
        {
            inputData.Add(tempV.x);
            inputData.Add(0.0f);
        }

        if (controller.mRigidBody.velocity.z < 0)
        {
            inputData.Add(0.0f);
            inputData.Add(tempV.y);
        }
        else
        {
            inputData.Add(tempV.y);
            inputData.Add(0.0f);
        }
    }

    void GetInputForClosestObject(List<double> inputData, List<GameObject> objects, float radius)
    {
        //4 inputs
        if (objects.Count == 0)
        {
            inputData.Add(0);
            inputData.Add(0);
            inputData.Add(0);
            inputData.Add(0);
            return;
        }
        //Don't need a null check because guaranteed to have at least one object or above return will execute
        GameObject closestObject = FindClosestObjectInVision(objects);
        float distance = Vector3.Distance(transform.position, closestObject.transform.position);
        distance = (distance / radius);
        Vector3 dir = (closestObject.transform.position - transform.position).normalized;
        float x = (dir.x);
        float z = (dir.z);


        inputData.Add(Mathf.Clamp(objects.Count / 10.0f, 0.0f,1.0f));
        inputData.Add(distance);
        inputData.Add(Mathf.Abs(x));
        inputData.Add(Mathf.Abs(z));
    }
}
