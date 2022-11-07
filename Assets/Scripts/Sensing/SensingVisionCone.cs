using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{
    private List<string> mSensingTags;

    public Dictionary<string, HashSet<Collider>> mSensingContainer;

    SphereCollider mCollider;
    SensingManager mManager;
    [SerializeField]
    public int mVisionConeAngle = 90;

    [SerializeField]
    public int mVisionConeSpacing;

    [SerializeField]
    public int mVisionDirectionOffset = 0;

    [SerializeField]
    public Color mVisionColor = Color.white;

    [SerializeField]
    public float mRadius;

    private void Awake()
    {
        mCollider = GetComponent<SphereCollider>();
        mSensingTags = GetComponentInParent<SensingManager>().sensingTag;
        mSensingContainer = new Dictionary<string, HashSet<Collider>>();
        foreach (string tag in mSensingTags)
            mSensingContainer.Add(tag, new HashSet<Collider>());
    }

    void Start()
    { 
        if (!mCollider.isTrigger)
            mCollider.isTrigger = true;
        mVisionConeAngle    = Mathf.Clamp(mVisionConeAngle, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeAngle + 1);
        mCollider.radius = mRadius;
    }
    private void Update()
    {
        if (GetComponentInParent<SensingManager>().mDebugDrawVisionCones)
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.green, Time.deltaTime);
            DrawDebugVisionCone();
        }

    }

    private void DrawDebugVisionCone()
    {
        for (int i = 0; i < mVisionConeAngle / 2.0f; i++)
        {
            float ang =  (i + transform.eulerAngles.y - mVisionConeSpacing/2.0f - mVisionDirectionOffset) * Mathf.Deg2Rad;
            float ang2 = (i - transform.eulerAngles.y - mVisionConeSpacing/2.0f + mVisionDirectionOffset) * Mathf.Deg2Rad;
            Vector3 pos1 = transform.position + new Vector3(Mathf.Sin(-ang2), 0.0f, Mathf.Cos(-ang2)) * mCollider.radius;
            Vector3 pos2 = transform.position + new Vector3(Mathf.Sin(ang), 0.0f, Mathf.Cos(ang)) * mCollider.radius;
            Debug.DrawLine(transform.position, pos1, mVisionColor, Time.deltaTime);
            Debug.DrawLine(transform.position, pos2, mVisionColor, Time.deltaTime);
        }
    }

    public Dictionary<string, List<GameObject>> GetObjectsWithinVision()
    {
        Dictionary<string, List<GameObject>> objectsInVisionList = new Dictionary<string, List<GameObject>>();
        foreach (string tag in mSensingTags)
        {
            objectsInVisionList[tag] = new List<GameObject>();
            foreach (Collider c in mSensingContainer[tag])
            {
                Vector3 temp = c.gameObject.transform.position - transform.position;
                float angle = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(temp.x,temp.z));
                float offset = 2.0f;

                bool withinVision;
                if (Mathf.Abs(mVisionDirectionOffset) + mVisionConeAngle / 2.0f > 180)
                {
                    float diff = Mathf.Abs(mVisionDirectionOffset) + mVisionConeAngle / 2.0f - 180;
                    Debug.Log("Within: " + (180 - diff));
                    withinVision = angle >= 180 - diff || angle <= - 180 + diff;
                }
                else
                {
                    withinVision = mVisionDirectionOffset - mVisionConeAngle / 2.0f <= angle && angle <= mVisionDirectionOffset + mVisionConeAngle / 2.0f;
                }
                
                if (withinVision)
                {
                    objectsInVisionList[tag].Add(c.gameObject);
                    //LOSCheckScript LOS = c.gameObject.GetComponentInChildren<LOSCheckScript>();
                    //if (LOS)
                    //{
                    //    foreach (GameObject t in LOS.LOSCheckPositions)
                    //    {
                    //        if (!Physics.Linecast(transform.position, t.transform.position))
                    //        {
                    //            objectsInVisionList[tag].Add(t);
                    //            break;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //Debug.DrawLine(transform.position, c.transform.position, Color.green, Time.deltaTime);
                    //    if (!Physics.Linecast(transform.position, c.transform.position))
                    //    {
                                //objectsInVisionList[tag].Add(c.gameObject);
                        //}
                    //}
                }
            }
        }
        return objectsInVisionList;
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach(string tag in mSensingTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                mSensingContainer[tag].Add(other);
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (string tag in mSensingTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                mSensingContainer[tag].Remove(other);
                Debug.Log("Removed. Size:" + mSensingContainer[tag].Count);
            }
        }
    }

    private void OnValidate()
    {
        mVisionDirectionOffset = Mathf.Clamp(mVisionDirectionOffset, -180, 180);
        mVisionConeAngle = Mathf.Clamp(mVisionConeAngle, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeAngle + 1);
    }
}
