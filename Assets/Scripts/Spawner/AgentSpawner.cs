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
        for (int i = 0; i < mMaxNumberOfAgents; i++)
        {
            GameObject obj = Instantiate(agentPrefab, gameObject.transform);
            obj.name = mAgentName + " #" + i.ToString();
            mCurrentNumberOfAgents++;
            RandomizeAgentPosition(obj);

            if (i >= mNumberAgentsToSpawn)
            {
                mAgentQueue.Enqueue(obj);
                obj.SetActive(false);
                mCurrentNumberOfAgents--;
            }
        }
        if (mSpawnerType == SpawnerType.Predator)
        {
            InvokeRepeating(nameof(IncreaseAllPredatorGeneticPoints), mExtraPointGivenRate, mExtraPointGivenRate);
        }
    }

    public GameObject SpawnAgent(GameObject prefab)
    {
        if (mMaxNumberOfAgents > mCurrentNumberOfAgents)
        {
            mCurrentNumberOfAgents++;
            if (mAgentQueue.Count <= 0)
                mAgentQueue.Enqueue(Instantiate(agentPrefab, gameObject.transform));
            GameObject obj = mAgentQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void ReturnAgentToPool(GameObject obj)
    {
        if (obj.activeInHierarchy)
        {
            mCurrentNumberOfAgents--;
            obj.SetActive(false);
            mAgentQueue.Enqueue(obj);
        }
    }

    public void RandomizeAgentPosition(GameObject obj)
    {
        float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
        float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
        obj.transform.position = new Vector3(randX, 1.0f, randZ);
        obj.transform.Rotate(0, Random.Range(0, 360), 0);
    }

    public void IncreaseAllPredatorGeneticPoints()
    {
        if (mExtraPointCounter <= mExtraPointGivenRate)
        {
            Debug.Log("Extra point given");
            mExtraPointCounter += 2;
            for (int i =0; i < gameObject.transform.childCount; i++) 
            {
                if (TryGetComponent(out PredatorController controller))
                {
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
