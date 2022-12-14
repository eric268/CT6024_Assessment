using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Types of vision cones that predator has
public enum PredatorVisionConeTypes
{
    Close_Cone,
    Far_Cone,
    NUM_VISION_CONE_TYPES
}
//Class that manages all predator sensing
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
        Debug.Assert(sensingVisionCones.Length == (int)PredatorVisionConeTypes.NUM_VISION_CONE_TYPES);
    }

    public void SetMateSensingRadius(float radius)
    {
        mMateSensingRadius = radius;
    }
    //Returns the closest prey agent
    //Checks the closest vision cone first if a prey agent is found can skip the far check
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

    //Checks in a full 360 area for a potential mate
    //Found predator also has to be performing mate action to work
    public GameObject FindClosestMate()
    {
        List<GameObject> objects = VisionCone.GetObjectsWithinRadius(mMateLayerMask,transform.position, mMateSensingRadius);
        GameObject mate = null;
        float closestDistance = Mathf.Infinity;
        PredatorController controller = GetComponentInParent<PredatorController>();

        foreach (GameObject obj in objects)
        {
            //Ensure it is not finding itself
            if (controller && obj != controller.gameObject)
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

    //Performs a ray cast to check if it will run into a wall
    //Only performs this check if there are no prey agents in its sensing colliders
    public bool IsFacingWall()
    {
        return Physics.Raycast(transform.position, transform.forward, mWallSensingRadius, mWallLayerMask);
    }

    //Below functions are all listening from genetic attribute changes

    public void SetWallSensingRadius(float radius)
    {
        mWallSensingRadius = radius;
    }

    public void SetCloseSensingAngle(float angle)
    {
        Debug.Assert(sensingVisionCones.Length == (int)PredatorVisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)PredatorVisionConeTypes.Close_Cone].SetAngle(angle);
    }

    public void SetCloseSensingRadius(float radius)
    {
        Debug.Assert(sensingVisionCones.Length == (int)PredatorVisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)PredatorVisionConeTypes.Close_Cone].SetRadius(radius);
    }


    public void SetFarSensingAngle(float angle)
    {
        Debug.Assert(sensingVisionCones.Length == (int)PredatorVisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)PredatorVisionConeTypes.Far_Cone].SetAngle(angle);
    }
    public void SetFarSensingRadius(float radius)
    {
        Debug.Assert(sensingVisionCones.Length == (int)PredatorVisionConeTypes.NUM_VISION_CONE_TYPES);
        sensingVisionCones[(int)PredatorVisionConeTypes.Far_Cone].SetRadius(radius);
    }
}
