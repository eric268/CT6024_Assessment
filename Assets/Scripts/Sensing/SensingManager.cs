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

    public InputData(float d, float x, float z)
    {
        float distance = d;
        float xPos = x;
        float zPos = z;
    }
}

public class SensingManager : MonoBehaviour
{
    SensingVisionCone[] sensingVisionCones;
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
                //***mVisionConeAngle may need to be /2.0********
                InputData temp = GetInputForClosestObject(objectsWithVision[tag], sensingVisionCones[i].mRadius, sensingVisionCones[i].mVisionConeAngle);
                inputData.Add(temp.distance);
                inputData.Add(temp.xPos);
                inputData.Add(temp.zPos);
            }
        }
        return inputData;
    }

    InputData GetInputForClosestObject(List<GameObject> objects, float radius, float angle)
    {
        if (objects.Count == 0)
            return new InputData(0.0f, 0.0f, 0.0f);

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
        float maxX = (Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
        float maxZ = (Mathf.Cos(angle * Mathf.Deg2Rad) * radius);

        float x = 1.0f - Mathf.Abs(g.transform.position.x - transform.position.x)/maxX;
        float z = 1.0f - Mathf.Abs(g.transform.position.z - transform.position.z)/maxZ;
        dist = 1.0f - (dist / radius);

        return new InputData(x, z, dist);
    }
}
