using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectiousScript : MonoBehaviour
{ 
    [SerializeField]
    LayerMask mAgentLayerMask;
    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.transform.parent.GetComponent<AttributesScript>().mInfected)
            return;

        if (mAgentLayerMask == (mAgentLayerMask | (1 << other.gameObject.layer)) && !other.gameObject.GetComponent<AttributesScript>().mInfected)
        {
            other.gameObject.GetComponent<StateTransitionScript>().InfectedTransition();
        }
    }
    
}
