using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VisionConeTypes
{
    Close_Cone,
    Far_Cone,
    NUM_VISION_CONE_TYPES
}


public class PredatorSensing : SensingScript
{
    [SerializeField]
    LayerMask mPreyLayerMask;
    [SerializeField]
    LayerMask mMateLayerMask;
    [SerializeField]
    LayerMask mWallLayerMask;

    PredatorAttributes mAttributes;

    // Start is called before the first frame update
    void Awake()
    {
        mAttributes = GetComponentInParent<PredatorController>().mAttributes;
        sensingVisionCones = GetComponentsInChildren<VisionCone>();
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
    }

    public GameObject FindClosestPrey()
    {
        foreach (VisionCone cone in sensingVisionCones)
        {
            List<GameObject> prey = sensingVisionCones[(int)VisionConeTypes.Close_Cone].GetObjectsWithinVision(mPreyLayerMask);
            if (prey.Count > 0)
            {
                return FindClosestObjectInVision(prey);
            }
        }
        return null;
    }

    public GameObject FindClosestMate()
    {
        List<GameObject> objects = sensingVisionCones[(int)VisionConeTypes.Far_Cone].GetObjectsWithinRadius(mMateLayerMask, mAttributes.mMateSensingRadius);
        GameObject mate = null;
        float closestDistance = Mathf.Infinity;
        PredatorController controller = GetComponentInParent<PredatorController>();
        foreach (GameObject obj in objects)
        {
            if (controller && obj != controller.gameObject && !controller.mAttributes.mMateFound)
            {
                GOBScript gob = obj.GetComponent<GOBScript>();
                if (gob && gob.mCurrentAction.mActionTypes == ActionType.Reproduce)
                {
                    float currentDistance = Vector3.Distance(gob.gameObject.transform.position, transform.position);
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        mate = obj;
                    }
                }
            }

        }
        return mate;
    }

    public bool IsFacingWall()
    {
        bool hit = Physics.Raycast(transform.position, transform.forward, sensingVisionCones[(int)VisionConeTypes.Far_Cone].mRadius, mWallLayerMask);
        Color color;
        if (hit)
        {
            color = Color.red;
        }
        else
        {
            color = Color.green;
        }

        Debug.DrawLine(transform.position, transform.position + transform.forward * sensingVisionCones[(int)VisionConeTypes.Far_Cone].mRadius, color, 1.0f);
        return hit;
    }
}
