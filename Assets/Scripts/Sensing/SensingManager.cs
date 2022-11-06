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
    void Start()
    {
        sensingVisionCones = GetComponentsInChildren<SensingVisionCone>();
    }

    public List<double> GetNeuralNetworkInputs()
    {
        List<double> inputData = new List<double>();
        for (int i = 0; i < sensingVisionCones.Length; i++)
        {
            var objectsWithVision = sensingVisionCones[i].GetObjectsWithinVision();
            foreach (string tag in sensingTag)
            {
                InputData temp = GetInputForClosestObject(objectsWithVision[tag],sensingVisionCones[i].mVisionDirectionOffset, sensingVisionCones[i].mRadius, sensingVisionCones[i].mVisionConeAngle/2.0f);
                inputData.Add(temp.distance);
                inputData.Add(temp.xPos);
                inputData.Add(temp.zPos);
            }
        }
        return inputData;
    }

    InputData GetInputForClosestObject(List<GameObject> objects, int offset, float radius, float angle)
    {
        if (objects.Count == 0)
            return new InputData();

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
        float x = 1.0f - Mathf.Abs(g.transform.position.x - transform.position.x)/maxX;
        float z = 1.0f - Mathf.Abs(g.transform.position.z - transform.position.z)/maxZ;

        //Vector3 dir = (g.transform.position - transform.position).normalized;
        //float x = Mathf.Abs(dir.x);
        //float z = Mathf.Abs(dir.z);
        //dist = 1.0f - (dist / radius);

        return new InputData(x, z, dist);
    }
}
