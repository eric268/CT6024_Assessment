using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorSensing : SensingScript
{
    [SerializeField]
    LayerMask mPreyLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        sensingVisionCones = GetComponentsInChildren<VisionCone>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject FindClosestObject()
    {
        float closest = -Mathf.Infinity;
        GameObject closestObject = null;
        foreach(VisionCone cone in sensingVisionCones)
        {
            //float dist = Vector3.Distance(gameObject)
        }
        return FindClosestObjectInVision(sensingVisionCones[0].GetObjectsWithinVision(mPreyLayerMask));
    }
}
