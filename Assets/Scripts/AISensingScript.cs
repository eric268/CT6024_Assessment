using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensingScript : MonoBehaviour
{
    public HashSet<Collider> mSensingArray;
    [SerializeField]
    public LayerMask mAgentLayerMask;
    void Start()
    {
        mSensingArray = new HashSet<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mAgentLayerMask == (mAgentLayerMask | (1 << other.gameObject.layer)))
        {
            mSensingArray.Add(other);
            Debug.Log("Agent added to mSensingArray");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mSensingArray.Remove(other);
    }
}
