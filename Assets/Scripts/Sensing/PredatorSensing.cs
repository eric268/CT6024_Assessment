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
    // Start is called before the first frame update
    void Start()
    {
        sensingVisionCones = GetComponentsInChildren<VisionCone>();
    }

    public GameObject FindClosestPrey()
    {
        return FindClosestObjectInVision(sensingVisionCones[0].GetObjectsWithinVision(mPreyLayerMask));
    }

    public GameObject FindClosestMate()
    {
        List<GameObject> objects = sensingVisionCones[0].GetObjectsWithinVision(mMateLayerMask);
        GameObject mate = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject obj in objects) 
        {
            if (obj != gameObject)
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
        return sensingVisionCones[0].GetObjectsWithinVision(mWallLayerMask).Count > 0 ? true : false;
    }
}
