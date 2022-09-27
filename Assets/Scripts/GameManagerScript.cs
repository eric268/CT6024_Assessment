using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public HashSet<GameObject> mNonInfectedAgents;

    // Start is called before the first frame update
    void Awake()
    {
        mNonInfectedAgents = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Agent"));
        Debug.Log(mNonInfectedAgents.Count);
    }

    private void Start()
    {
        BeginInfection();
    }

    private void BeginInfection()
    {
        int rand = Random.Range(0, mNonInfectedAgents.Count);
        mNonInfectedAgents.ElementAt(rand).GetComponent<StateTransitionScript>().BeginInfection();
    }

    // Update is called once per frame
    public void NewAgentInfected(GameObject infectedAgent)
    {
        mNonInfectedAgents.Remove(infectedAgent);
    }
}
