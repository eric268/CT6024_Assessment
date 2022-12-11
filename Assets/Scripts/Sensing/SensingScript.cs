using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensingScript : MonoBehaviour
{
    protected VisionCone[] sensingVisionCones;

    protected GameObject FindClosestObjectInVision(List<GameObject> objects)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;

        float dist = 0.0f;
        foreach (GameObject obj in objects)
        {
            dist = Vector3.Distance(obj.transform.position, transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestObject = obj;
            }
        }

        return closestObject;
    }
}
