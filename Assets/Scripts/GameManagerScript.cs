using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public HashSet<GameObject> mNonInfectedAgents;
    // Start is called before the first frame update
    void Awake()
    {
        mNonInfectedAgents = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Agent"));
    }

    // Update is called once per frame
    public void NewAgentInfected(GameObject infectedAgent)
    {
        mNonInfectedAgents.Remove(infectedAgent);
    }
}
