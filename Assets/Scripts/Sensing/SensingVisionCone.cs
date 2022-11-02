using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{
    public HashSet<Collider> mSensingContainer;

    SphereCollider mCollider;

    [SerializeField]
    public LayerMask mAgentLayerMask;

    [SerializeField]
    public int mVisionConeSize = 90;

    void Start()
    {
        mSensingContainer = new HashSet<Collider>();
        mCollider = GetComponent<SphereCollider>();
        if (!mCollider.isTrigger)
            mCollider.isTrigger = true;
    }
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5.0f, Color.red, Time.deltaTime);
        DrawDebugVisionCone();
        
        CheckIfWithinVision();
    }

    private void DrawDebugVisionCone()
    {
        for (int i = 0; i < mVisionConeSize/2.0f; i++)
        {
            float ang = (i + transform.eulerAngles.y) * Mathf.Deg2Rad;
            float ang2 = (i - transform.eulerAngles.y) * Mathf.Deg2Rad;
            Vector3 pos1 = transform.position + new Vector3(Mathf.Sin(-ang2), 0.0f, Mathf.Cos(-ang2)).normalized * mCollider.radius;
            Vector3 pos2 = transform.position + new Vector3(Mathf.Sin(ang), 0.0f, Mathf.Cos(ang)).normalized * mCollider.radius;
            Debug.DrawLine(transform.position, pos1, Color.white, Time.deltaTime);
            Debug.DrawLine(transform.position, pos2, Color.white, Time.deltaTime);
        }
    }

    private void CheckIfWithinVision()
    {
        if (mSensingContainer.Count == 0)
            return;

        foreach (Collider coll in mSensingContainer)
        {
            float angle =Vector3.Angle(transform.forward, (coll.gameObject.transform.position - transform.position));
            if (angle <= mVisionConeSize/2.0f)
            {
                var LOSCheck = coll.gameObject.GetComponentInChildren<LOSCheckScript>().LOSCheckPositions;
                foreach(Transform t in LOSCheck)
                {
                    if (!Physics.Linecast(transform.position,t.position))
                    {
                            Debug.Log("Spotted");
                            break;
                        
                    }
                }
                

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Agent"))
        {
            mSensingContainer.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mSensingContainer.Remove(other);
    }
}
