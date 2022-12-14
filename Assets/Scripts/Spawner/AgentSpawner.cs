using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public enum SpawnerType
{
    Food,
    Prey,
    Predator
}

//Handles spawning of the predator and prey agents
public class AgentSpawner : MonoBehaviour
{
    Transform groundPosition;
    [SerializeField]
    int mNumberAgentsToSpawn;
    [SerializeField]
    GameObject agentPrefab;

    [SerializeField]
    private string mAgentName;
    public Queue<GameObject> mAgentQueue;
    public int mCurrentGen = 1;
    public int mMaxNumberOfAgents;
    public int mCurrentNumberOfAgents;

    private int mMaxNumberOfExtraPoints = 50;
    private int mExtraPointCounter = 0;
    public int mExtraPointGivenRate = 45;

    [SerializeField]
    private SpawnerType mSpawnerType;
    // Start is called before the first frame update

    private void Awake()
    {
        groundPosition = GameObject.FindGameObjectWithTag("Ground").transform;
        mAgentQueue = new Queue<GameObject>();
    }

    private void Start()
    {
        for (int i = mCurrentNumberOfAgents; i < mMaxNumberOfAgents; i++)
        {
            GameObject obj = Instantiate(agentPrefab, gameObject.transform);
            obj.name = mAgentName + " #" + i.ToString();
            mCurrentNumberOfAgents++;
            RandomizeAgentPosition(obj);
            //Adds excess agents to object pool
            if (i >= mNumberAgentsToSpawn)
            {
                mAgentQueue.Enqueue(obj);
                obj.SetActive(false);
                mCurrentNumberOfAgents--;
            }
        }
        //Added to potentially give a boost to predators as prey neural network improves
        //if (mSpawnerType == SpawnerType.Predator)
        //{
        //    InvokeRepeating(nameof(IncreaseAllPredatorGeneticPoints), mExtraPointGivenRate, mExtraPointGivenRate);
        //}
    }
    //Checks to ensure max number of agents isn't reached
    //If so will return null
    public GameObject SpawnAgent(GameObject prefab)
    {
        if (mMaxNumberOfAgents > mCurrentNumberOfAgents)
        {
            mCurrentNumberOfAgents++;
            //If pool queue is empty will create new agent
            if (mAgentQueue.Count <= 0)
                mAgentQueue.Enqueue(Instantiate(agentPrefab, gameObject.transform));
            GameObject obj = mAgentQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }
    //Returns agent to object pool queue
    public void ReturnAgentToPool(GameObject obj)
    {
        if (obj.activeInHierarchy)
        {
            mCurrentNumberOfAgents--;
            obj.SetActive(false);
            mAgentQueue.Enqueue(obj);
        }
    }
    //Spawns an agent in a random position on the map
    public void RandomizeAgentPosition(GameObject obj)
    {
        float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
        float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
        obj.transform.position = new Vector3(randX, 1.0f, randZ);
        obj.transform.Rotate(0, Random.Range(0, 360), 0);
    }
    //Added to potentially give a boost to predators as prey neural network improves
    //Over-time will add genetic attribute points to predator
    public void IncreaseAllPredatorGeneticPoints()
    {
        if (mExtraPointCounter <= mMaxNumberOfExtraPoints)
        {
            Debug.Log("Extra point given");
            mExtraPointCounter++;
            for (int i =0; i < gameObject.transform.childCount; i++) 
            {
                if (TryGetComponent(out PredatorController controller))
                {
                    //Gives two points at a time
                    controller.mNumberGeneticPoints+=2;
                    controller.UpdatePoints();
                }
            }
        }
        else
        {
            CancelInvoke(nameof(IncreaseAllPredatorGeneticPoints));
        }
    }
}
