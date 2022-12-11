using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

//[RequireComponent(typeof(SphereCollider))]
public class SensingVisionCone : MonoBehaviour
{

    public List<GameObject> mSensedObjects;
    [Range(0,360)]
    public float mVisionConeAngle = 90;
    public float mVisionDirectionOffset = 0;

    [SerializeField]
    public float mRadius;

    void Start()
    {
        mSensedObjects = new List<GameObject>();
    }

    public Vector3 DirFromAngle(float angleInDegrees,bool angleIsGlobal)
    {
        if (!angleIsGlobal) 
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        angleInDegrees -= mVisionDirectionOffset;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public List<GameObject> GetObjectsWithinVision(LayerMask objectToFind)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, mRadius, objectToFind);
        mSensedObjects.Clear();

        for (int i =0; i < targetsInViewRadius.Length; i++) 
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float angle = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(dirToTarget.x, dirToTarget.z));
            bool withinVision;
            if (Mathf.Abs(mVisionDirectionOffset) + mVisionConeAngle / 2.0f > 180)
            {
                float diff = Mathf.Abs(mVisionDirectionOffset) + mVisionConeAngle / 2.0f - 180;
                withinVision = angle >= 180 - diff || angle <= -180 + diff;
            }
            else
            {
                withinVision = mVisionDirectionOffset - mVisionConeAngle / 2.0f <= angle && angle <= mVisionDirectionOffset + mVisionConeAngle / 2.0f;
            }
            if (withinVision)
            {
                mSensedObjects.Add(targetsInViewRadius[i].gameObject);
            }
        }
        return mSensedObjects;
    }

    private void OnValidate()
    {
        mVisionDirectionOffset = Mathf.Clamp(mVisionDirectionOffset, -180, 180);
        mVisionConeAngle = Mathf.Clamp(mVisionConeAngle, 0, 360);
    }
}
