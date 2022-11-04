using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{
    public HashSet<Collider> mSensingContainer;
    [SerializeField]
    public Dictionary<Collider, float> mSpottedCounter;

    SphereCollider mCollider;

    [SerializeField]
    public int mVisionConeSize = 90;

    [SerializeField]
    public int mVisionConeSpacing;

    [SerializeField]
    float mSpottedRate = 2.0f;

    GameObject mCurrentTarget;

    void Start()
    {
        mSpottedCounter = new Dictionary<Collider, float>();
        mSensingContainer = new HashSet<Collider>();
        mCollider = GetComponent<SphereCollider>();
        if (!mCollider.isTrigger)
            mCollider.isTrigger = true;
        mVisionConeSize    = Mathf.Clamp(mVisionConeSize, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeSize + 1);
    }
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.red, Time.deltaTime);
        DrawDebugVisionCone();

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
            float ang =  (i + transform.eulerAngles.y + mVisionConeSpacing/2.0f) * Mathf.Deg2Rad;
            float ang2 = (i - transform.eulerAngles.y + mVisionConeSpacing/2.0f) * Mathf.Deg2Rad;
            Vector3 pos1 = transform.position + new Vector3(Mathf.Sin(-ang2), 0.0f, Mathf.Cos(-ang2)).normalized * mCollider.radius;
            Vector3 pos2 = transform.position + new Vector3(Mathf.Sin(ang), 0.0f, Mathf.Cos(ang)).normalized * mCollider.radius;
            Debug.DrawLine(transform.position, pos1, Color.white, Time.deltaTime);
            Debug.DrawLine(transform.position, pos2, Color.white, Time.deltaTime);
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
            if (mVisionConeSpacing/2.0f <= angle + offset && angle <= mVisionConeSize/2.0f + mVisionConeSpacing/2.0f - offset)
            {
                var LOSCheck = c.gameObject.GetComponentInChildren<LOSCheckScript>().LOSCheckPositions;
                foreach(GameObject t in LOSCheck)
                {
                    if (!Physics.Linecast(transform.position,t.transform.position))
                    {
                        Debug.Log(mSpottedCounter[c]);
                        mSpottedCounter[c] += mSpottedRate * Time.deltaTime;
                        if (mSpottedCounter[c] >= mSpottedRate)
                        {
                            mSpottedCounter[c] = mSpottedRate;
                            return c.gameObject;
                        }
                        else
                            break;
                    }
                    else
                    {
                        mSpottedCounter[c] -= mSpottedRate * Time.deltaTime;
                        mSpottedCounter[c] = Mathf.Clamp(mSpottedCounter[c], 0.0f, mSpottedRate);
                        break;  
                    }
                }
            }
            else
            {
                mSpottedCounter[c] -= mSpottedRate * Time.deltaTime;
                mSpottedCounter[c] = Mathf.Clamp(mSpottedCounter[c], 0.0f, mSpottedRate);
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Agent"))
        {
            mSensingContainer.Add(other);
            if (!mSpottedCounter.ContainsKey(other))
                 mSpottedCounter.Add(other, 0.0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mSensingContainer.Remove(other);
        mSpottedCounter.Remove(other);
    }

    private void OnValidate()
    {
        mVisionConeSize = Mathf.Clamp(mVisionConeSize, 0, 360);
        mVisionConeSpacing = Mathf.Clamp(mVisionConeSpacing, 0, 360 - mVisionConeSize + 1);
    }
}
