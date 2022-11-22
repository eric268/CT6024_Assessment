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
    public SensingVisionCone[] sensingVisionCones;
    public List<string> sensingTag;
    [SerializeField]
    public bool mDebugDrawVisionCones;
    void Start()
    {
        sensingVisionCones = GetComponentsInChildren<SensingVisionCone>();
    }

    public List<double> GetNeuralNetworkInputs(GameObject agent)
    {
        List<double> inputData = new List<double>();
        SetDirectionNetworkInput(agent, inputData);
        //Pass in x, z and distance values for closest object in each vision cone
        for (int i = 0; i < sensingVisionCones.Length; i++)
        {
            var objectsWithVision = sensingVisionCones[i].GetObjectsWithinVision();
            foreach (string tag in sensingTag)
            {
                GetInputForClosestObject(inputData, objectsWithVision[tag],sensingVisionCones[i].mVisionDirectionOffset, sensingVisionCones[i].mRadius, sensingVisionCones[i].mVisionConeAngle);
            }
        }
        return inputData;
    }

    void SetDirectionNetworkInput(GameObject agent, List<double> inputData)
    {
        //4 inputs
        AgentController controller = agent.GetComponent<AgentController>();
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

        //if (controller.mRigidBody.velocity.x < 0 || controller.mRigidBody.velocity.z < 0)
        //{
        //    inputData.Add(0.0f);
        //    inputData.Add(0.0f);
        //    inputData.Add(tempV.x);
        //    inputData.Add(tempV.y);
        //}
        //else
        //{
        //    inputData.Add(tempV.x);
        //    inputData.Add(tempV.y);
        //    inputData.Add(0.0f);
        //    inputData.Add(0.0f);
        //}
    }

    void GetInputForClosestObject(List<double> inputData, List<GameObject> objects, int offset, float radius, float angle)
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

        //This is for passing in x z values based on how close the object is relative to max distance
        float maxX = Mathf.Abs(Mathf.Sin((offset + Mathf.Cos(offset * Mathf.Deg2Rad) * angle * 0.5f) * Mathf.Deg2Rad) * radius);
        float maxZ = Mathf.Abs(Mathf.Cos((offset + Mathf.Sin(offset * Mathf.Deg2Rad) * 0.5f * angle) * Mathf.Deg2Rad) * radius);
        //float maxX = Mathf.Max(Mathf.Sin((offset + angle * 0.5f) * Mathf.Deg2Rad), Mathf.Sin((offset - angle * 0.5f) * Mathf.Deg2Rad)) * radius;
        //float maxZ = Mathf.Max(Mathf.Cos((offset + angle * 0.5f) * Mathf.Deg2Rad), Mathf.Cos((offset - angle * 0.5f) * Mathf.Deg2Rad)) * radius;

        //Debug.Log("MaxX" + maxX);
        //Debug.Log("MaxZ" + maxZ);
        //Debug.Log("xPos" + (g.transform.position.x - transform.position.x));
        //Debug.Log("zPos" + (g.transform.position.z - transform.position.z));

        //float x = /*1.0f - */ Mathf.Abs(g.transform.position.x - transform.position.x)/maxX;
        //float z = /*1.0f - */ Mathf.Abs(g.transform.position.z - transform.position.z)/maxZ;
        dist    = /*1.0f -   */(dist / radius);

        Vector3 dir = (g.transform.position - transform.position).normalized;
        float x = (dir.x);
        float z = (dir.z);


        inputData.Add(Mathf.Clamp(objects.Count / 10.0f, 0.0f,1.0f));
        inputData.Add(dist);
        inputData.Add(Mathf.Abs(x));
        inputData.Add(Mathf.Abs(z));
        /*
        if (x < 0)
        {
            inputData.Add(x);
            inputData.Add(0.0f);
        }
        else
        {
            inputData.Add(0.0f);
            inputData.Add(x);
        }
        if (z < 0)
        {
            inputData.Add(z);
            inputData.Add(0.0f);
        }
        else
        {
            inputData.Add(0.0f);
            inputData.Add(z);
        }
        */
    }
}
