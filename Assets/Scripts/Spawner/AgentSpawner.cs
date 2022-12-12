using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;



public class AgentSpawner : MonoBehaviour
{
    Transform groundPosition;

    [SerializeField]
    int mNumberAgentsToSpawn = 100;
    [SerializeField]
    GameObject agentPrefab;

    public Queue<GameObject> mAgentQueue;
    public int mCurrentGen = 1;
    public int mMaxNumberOfAgents;
    public int mCurrentNumberOfAgents;
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
            mCurrentNumberOfAgents++;
            RandomizeAgentPosition(obj);

            if (i >= mNumberAgentsToSpawn)
            {
                mAgentQueue.Enqueue(obj);
                obj.SetActive(false);
                mCurrentNumberOfAgents--;
            }
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
}
