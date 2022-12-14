using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script allows for agents to be selected so that their attributes can be seen by the user
public class AgentSelectionScript : MonoBehaviour
{
    [SerializeField]
    GameObject mSelectedAgent;
    [SerializeField]
    LayerMask mAgentLayerMask;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            CheckIfAgentClicked();
        }
    }
    //Checks if an agent has been clicked
    //If so it will display their UI element
    //Also will deactivate the UI for the previously selected agent
    void CheckIfAgentClicked()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 300, mAgentLayerMask))
        {
            if (mSelectedAgent != null) 
            {
                if (mSelectedAgent == hit.collider.gameObject)
                {
                    return;
                }
                else if (mSelectedAgent.transform.parent.TryGetComponent(out AgentUIManager manager))
                {
                    manager.enabled = false;
                }
            }
            if (hit.collider.gameObject.transform.parent.TryGetComponent(out AgentUIManager newManager))
            {
                newManager.enabled = true;
                mSelectedAgent = hit.collider.gameObject;
            }
        }
    }
}
