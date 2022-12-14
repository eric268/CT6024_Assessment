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

    //Checks if a prey agent has collied with the wall.
    //If so it teleports the agent to the opposite wall on that axis
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Prey"))
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
