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

    private float mMateSensingRadius;
    private float mWallSensingRadius;

    // Start is called before the first frame update
    void Awake()
    {
        sensingVisionCones = GetComponentsInChildren<VisionCone>();
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
    }

    public void SetMateSensingRadius(float radius)
    {
        mMateSensingRadius = radius;
    }

    public GameObject FindClosestPrey()
    {
        foreach (VisionCone cone in sensingVisionCones)
        {
            List<GameObject> prey = cone.GetObjectsWithinVision(mPreyLayerMask);
            if (prey.Count > 0)
            {
                return FindClosestObjectInVision(prey);
            }
        }
        return null;
    }

    public GameObject FindClosestMate()
    {
        List<GameObject> objects = VisionCone.GetObjectsWithinRadius(mMateLayerMask,transform.position, mMateSensingRadius);
        GameObject mate = null;
        float closestDistance = Mathf.Infinity;
        PredatorController controller = GetComponentInParent<PredatorController>();

        foreach (GameObject obj in objects)
        {
            if (controller && obj != controller.gameObject && !controller.mMate)
            {
                GOBScript gob = obj.GetComponent<GOBScript>();
                if (gob && gob.mCurrentAction is ReproduceAction)
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
        return Physics.Raycast(transform.position, transform.forward, mWallSensingRadius, mWallLayerMask);
    }

    public void SetWallSensingRadius(float radius)
    {
        mWallSensingRadius = radius;
    }

    public void SetCloseSensingAngle(float angle)
    {
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)VisionConeTypes.Close_Cone].SetAngle(angle);
    }

    public void SetCloseSensingRadius(float radius)
    {
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)VisionConeTypes.Close_Cone].SetRadius(radius);
    }


    public void SetFarSensingAngle(float angle)
    {
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)VisionConeTypes.Far_Cone].SetAngle(angle);
    }
    public void SetFarSensingRadius(float radius)
    {
        Debug.Assert(sensingVisionCones.Length == (int)VisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)VisionConeTypes.Far_Cone].SetRadius(radius);
    }
}
