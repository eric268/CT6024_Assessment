using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        return FindClosestObjectInVision(sensingVisionCones[0].GetObjectsWithinVision(mPreyLayerMask));
    }
}
