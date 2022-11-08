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
        //First pass in current direction as inputs
        //Input[0] 1 if moving forward
        //Input[1] 1 if moving right
        //Input[2] 1 if moving back
        //Input[3] 1 if moving left
        PreyController controller = agent.GetComponent<PreyController>();   
        inputData.Add(controller.mRigidBody.velocity.x/controller.mAttributes.mSpeed);
        inputData.Add(controller.mRigidBody.velocity.z/controller.mAttributes.mSpeed);

        //for (int i = 0; i < 4; i++)
        //{
        //    inputData.Add(0);
        //}
        //if (agent.GetComponent<Rigidbody>().velocity.x > 0.0)
        //{
        //    inputData[0] = 1.0;
        //}
        //else if (agent.GetComponent<Rigidbody>().velocity.x < 0.0)
        //{
        //    inputData[1] = 1.0;
        //}
        //else if (agent.GetComponent<Rigidbody>().velocity.z > 0.0)
        //{
        //    inputData[2] = 1.0;
        //}
        //else
        //{
        //    inputData[3] = 1.0;
        //}
    }

    void GetInputForClosestObject(List<double> inputData, List<GameObject> objects, int offset, float radius, float angle)
    {
        if (objects.Count == 0)
        {
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

        float x = /*1.0f - */ Mathf.Abs(g.transform.position.x - transform.position.x)/maxX;
        float z = /*1.0f - */ Mathf.Abs(g.transform.position.z - transform.position.z)/maxZ;
        dist    = 1.0f -   (dist / radius);

        //Vector3 dir = (g.transform.position - transform.position).normalized;
        //float x = Mathf.Abs(dir.x);
        //float z = Mathf.Abs(dir.z);

        inputData.Add(dist);
        inputData.Add(x);
        inputData.Add(z);
    }
}
