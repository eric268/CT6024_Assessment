using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensingScript : MonoBehaviour
{
    public HashSet<Collider> mSensingArray;
    public List<Collider> mSensingList;
    [SerializeField]
    public LayerMask mAgentLayerMask;
    void Start()
    {
        mSensingArray = new HashSet<Collider>();
        mSensingList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mAgentLayerMask == (mAgentLayerMask | (1 << other.gameObject.layer)))
        {
            mSensingArray.Add(other);
            mSensingList.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mSensingArray.Remove(other);
        mSensingList.Remove(other);
    }
}
