using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{
    public HashSet<Collider> mSensingContainer;
    [SerializeField]
    public Dictionary<Collider, float> mSpottedContainer;

    SphereCollider mCollider;

    [SerializeField]
    public int mVisionConeSize = 90;

    [SerializeField]
    public int mVisionConeSpacing;

    [SerializeField]
    public int mVisionDirectionOffset = 0;

    [SerializeField]
    public Color mVisionColor = Color.white;

    [SerializeField]
    public float mVisionDistance = 10.0f;


    [SerializeField]
    float mSpottedTimer = 2.0f;

    GameObject mCurrentTarget;

    private void Awake()
    {
        mCollider = GetComponent<SphereCollider>();
    }

    void Start()
    {
        mSpottedContainer = new Dictionary<Collider, float>();
        mSensingContainer = new HashSet<Collider>();
        if (!mCollider.isTrigger)
            mCollider.isTrigger = true;
        mCollider.radius = mVisionDistance;
        mVisionConeSize    = Mathf.Clamp(mVisionConeSize, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeSize + 1);
    }
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.red, Time.deltaTime);
        //DrawDebugVisionCone();

        mCurrentTarget = CheckIfWithinVision();
        if (mCurrentTarget)
        {
            Debug.Log("Target Spotted");
        }
    }

    private void DrawDebugVisionCone()
    {
        for (int i = 0; i < mVisionConeSize / 2.0f; i++)
        {
            float ang =  (i + transform.eulerAngles.y + mVisionConeSpacing/2.0f + mVisionDirectionOffset) * Mathf.Deg2Rad;
            float ang2 = (i - transform.eulerAngles.y + mVisionConeSpacing/2.0f - mVisionDirectionOffset) * Mathf.Deg2Rad;
            Vector3 pos1 = transform.position + new Vector3(Mathf.Sin(-ang2), 0.0f, Mathf.Cos(-ang2)).normalized * mCollider.radius;
            Vector3 pos2 = transform.position + new Vector3(Mathf.Sin(ang), 0.0f, Mathf.Cos(ang)).normalized * mCollider.radius;
            Debug.DrawLine(transform.position, pos1, mVisionColor, Time.deltaTime);
            Debug.DrawLine(transform.position, pos2, mVisionColor, Time.deltaTime);
        }
    }

    GameObject CheckIfWithinVision()
    {
        if (mSensingContainer.Count == 0)
            return null;

        foreach (Collider c in mSensingContainer)
        {
            float angle =Vector3.Angle(transform.forward, (c.gameObject.transform.position - transform.position));
            float offset = 2.0f;
            if (mVisionConeSpacing/2.0f + mVisionDirectionOffset <= angle + offset && angle <= mVisionConeSize/2.0f + mVisionConeSpacing/2.0f - offset + mVisionDirectionOffset)
            {
                var LOSCheck = c.gameObject.GetComponentInChildren<LOSCheckScript>().LOSCheckPositions;
                foreach(GameObject t in LOSCheck)
                {
                    if (!Physics.Linecast(transform.position,t.transform.position))
                    {
                        Debug.Log(mSpottedContainer[c]);
                        mSpottedContainer[c] += mSpottedTimer * Time.deltaTime;
                        if (mSpottedContainer[c] >= mSpottedTimer)
                        {
                            mSpottedContainer[c] = mSpottedTimer;
                            return c.gameObject;
                        }
                        else
                            break;
                    }
                    else
                    {
                        mSpottedContainer[c] -= mSpottedTimer * Time.deltaTime;
                        mSpottedContainer[c] = Mathf.Clamp(mSpottedContainer[c], 0.0f, mSpottedTimer);
                        break;  
                    }
                }
            }
            else
            {
                mSpottedContainer[c] -= mSpottedTimer * Time.deltaTime;
                mSpottedContainer[c] = Mathf.Clamp(mSpottedContainer[c], 0.0f, mSpottedTimer);
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Agent"))
        {
            mSensingContainer.Add(other);
            if (!mSpottedContainer.ContainsKey(other))
                 mSpottedContainer.Add(other, 0.0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mSensingContainer.Remove(other);
        mSpottedContainer.Remove(other);
    }

    private void OnValidate()
    {
        mVisionDirectionOffset = Mathf.Clamp(mVisionDirectionOffset, 0, 360);
        mVisionConeSize = Mathf.Clamp(mVisionConeSize, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeSize + 1);
        if (mCollider)
            mCollider.radius = mVisionDistance;
    }
}
