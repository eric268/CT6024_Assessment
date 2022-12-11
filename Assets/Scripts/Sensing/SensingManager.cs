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

public class SensingManager : MonoBehaviour
{
    List<double> inputData;
    public SensingVisionCone[] sensingVisionCones;
    public LayerMask[] mSensingLayerMasks;
    [SerializeField]
    public bool mDebugDrawVisionCones;
    void Start()
    {
        inputData = new List<double>();
        sensingVisionCones = GetComponentsInChildren<SensingVisionCone>();
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
        //5 inputs
        if (objects.Count == 0)
        {
            inputData.Add(0);
            inputData.Add(0);
            inputData.Add(0);
            inputData.Add(0);
            return;
        }

        float closestDistance = Mathf.Infinity;
        GameObject g = null;

        float dist = 0.0f;
        foreach (GameObject obj in objects)
        {
            dist = Vector3.Distance(obj.transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                g = obj;
            }
        }

        dist = (dist / radius);
        Vector3 dir = (g.transform.position - transform.position).normalized;
        float x = (dir.x);
        float z = (dir.z);


        inputData.Add(Mathf.Clamp(objects.Count / 10.0f, 0.0f,1.0f));
        inputData.Add(dist);
        inputData.Add(Mathf.Abs(x));
        inputData.Add(Mathf.Abs(z));
    }
}
