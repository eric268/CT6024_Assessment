using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PredatorSensing : SensingScript
{
    [SerializeField]
    LayerMask mPreyLayerMask;
    [SerializeField]
    LayerMask mMateLayerMask;
    [SerializeField]
    LayerMask mWallLayerMask;

    PredatorAttributes mAttributes;
    public VisionCone mVisionCone;
    // Start is called before the first frame update
    void Awake()
    {
        mAttributes = GetComponentInParent<PredatorController>().mAttributes;
        mVisionCone = GetComponentInChildren<VisionCone>();
    }

    public GameObject FindClosestPrey()
    {
        return FindClosestObjectInVision(mVisionCone.GetObjectsWithinVision(mPreyLayerMask));
    }

    public GameObject FindClosestMate()
    {
        List<GameObject> objects = mVisionCone.GetObjectsWithinRadius(mMateLayerMask, mAttributes.mMateSensingRadius);
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
        bool hit = Physics.Raycast(transform.position, transform.forward, mVisionCone.mRadius, mWallLayerMask);
        Color color;
        if (hit)
        {
            color = Color.red;
        }
        else
        {
            color = Color.green;
        }

        Debug.DrawLine(transform.position, transform.position + transform.forward * mVisionCone.mRadius, color, 1.0f);        
        return hit;
    }
}
