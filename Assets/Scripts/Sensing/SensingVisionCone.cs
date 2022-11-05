using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{
    private List<string> mSensingTags;

    public Dictionary<string, HashSet<Collider>> mSensingContainer;

    SphereCollider mCollider;

    [SerializeField]
    public int mVisionConeAngle = 90;

    [SerializeField]
    public int mVisionConeSpacing;

    [SerializeField]
    public int mVisionDirectionOffset = 0;

    [SerializeField]
    public Color mVisionColor = Color.white;

    [SerializeField]
    public float mVisionDistance = 10.0f;

    public float mRadius;

    private void Awake()
    {
        mCollider = GetComponent<SphereCollider>();
    }

    void Start()
    {
        mSensingTags = GetComponentInParent<SensingManager>().sensingTag;
        mSensingContainer = new Dictionary<string, HashSet<Collider>>();
        if (!mCollider.isTrigger)
            mCollider.isTrigger = true;
        mCollider.radius = mVisionDistance;
        mVisionConeAngle    = Mathf.Clamp(mVisionConeAngle, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeAngle + 1);
        mRadius = mCollider.radius;
    }
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.red, Time.deltaTime);
    }

    private void DrawDebugVisionCone()
    {
        for (int i = 0; i < mVisionConeAngle / 2.0f; i++)
        {
            float ang =  (i + transform.eulerAngles.y + mVisionConeSpacing/2.0f + mVisionDirectionOffset) * Mathf.Deg2Rad;
            float ang2 = (i - transform.eulerAngles.y + mVisionConeSpacing/2.0f - mVisionDirectionOffset) * Mathf.Deg2Rad;
            Vector3 pos1 = transform.position + new Vector3(Mathf.Sin(-ang2), 0.0f, Mathf.Cos(-ang2)).normalized * mCollider.radius;
            Vector3 pos2 = transform.position + new Vector3(Mathf.Sin(ang), 0.0f, Mathf.Cos(ang)).normalized * mCollider.radius;
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
                float angle = Vector3.Angle(transform.forward, (c.gameObject.transform.position - transform.position));
                float offset = 2.0f;
                if (mVisionConeSpacing / 2.0f + mVisionDirectionOffset <= angle + offset && angle <= mVisionConeAngle / 2.0f + mVisionConeSpacing / 2.0f - offset + mVisionDirectionOffset)
                {
                    LOSCheckScript LOS = c.gameObject.GetComponentInChildren<LOSCheckScript>();
                    if (LOS)
                    {
                        foreach (GameObject t in LOS.LOSCheckPositions)
                        {
                            if (!Physics.Linecast(transform.position, t.transform.position))
                            {
                                objectsInVisionList[tag].Add(t);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!Physics.Linecast(transform.position, c.transform.position))
                        {
                            objectsInVisionList[tag].Add(c.gameObject);
                        }
                    }
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
                if (!mSensingContainer.ContainsKey(tag))
                    mSensingContainer.Add(tag, new HashSet<Collider>());
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
            }
        }
    }

    private void OnValidate()
    {
        mVisionDirectionOffset = Mathf.Clamp(mVisionDirectionOffset, 0, 360);
        mVisionConeAngle = Mathf.Clamp(mVisionConeAngle, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeAngle + 1);
        if (mCollider)
            mCollider.radius = mVisionDistance;
    }
}
