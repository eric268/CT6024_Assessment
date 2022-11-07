using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public HashSet<GameObject> mNonInfectedAgents;
    [SerializeField]
    public float mTimeSpeed;
    [SerializeField]
    public static int maxNumberOfPrey = 200;
    public static int currentNumberOfPrey;


    // Start is called before the first frame update
    void Awake()
    {
        //mNonInfectedAgents = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Agent"));
    }
    private void Update()
    {
        Debug.Log(currentNumberOfPrey);
        if (currentNumberOfPrey == 0)
            Debug.Log(Time.time);
    }
    private void Start()
    {
        //BeginInfection();
    }

    private void BeginInfection()
    {
        if(mNonInfectedAgents.Count > 0)
        {
            int rand = Random.Range(0, mNonInfectedAgents.Count);
            mNonInfectedAgents.ElementAt(rand).GetComponent<AttributesScript>().mCurrentState = CurrentState.NEWLY_INFECTED;
        }
    }
    private void OnValidate()
    {
        Time.timeScale = mTimeSpeed;
    }

    // Update is called once per frame
    public void NewAgentInfected(GameObject infectedAgent)
    {
        mNonInfectedAgents.Remove(infectedAgent);
    }
}
