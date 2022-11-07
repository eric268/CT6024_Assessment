using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    [SerializeField]
    Vector3 newPos;
    [SerializeField]
    bool xAxis;
    [SerializeField]
    LayerMask agentLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (agentLayerMask == (agentLayerMask | 1 << collision.gameObject.layer))
        {
            if (xAxis)
            {
                collision.gameObject.transform.position = new Vector3(newPos.x, collision.gameObject.transform.position.y, collision.gameObject.transform.position.z);
            }
            else
            {
                collision.gameObject.transform.position = new Vector3(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y, newPos.z);
            }
        }
    }
}
