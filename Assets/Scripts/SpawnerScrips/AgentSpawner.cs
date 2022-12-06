using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;



public class AgentSpawner : SpawnerScript
{
    [SerializeField]
    Transform groundPosition;

    [SerializeField]
    int numPreyToSpawn = 100;
    [SerializeField]
    GameObject preyPrefab;

    public Queue<GameObject> mAgentQueue;
    public GameObject[] mAgentArray;

    [Header("Generation Debug")]
    public bool mRespawnPrey = false;
    public int mRespawnAmount = 100;

    UIManager mainUI;

    public int mCurrentGen = 1;

    public int mMaxNumberOfAgents;
    public int mCurrentNumberOfAgents;

    public string name;
    // Start is called before the first frame update

    private void Awake()
    {
        name = gameObject.name;
        mainUI = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<UIManager>();
        mAgentQueue = new Queue<GameObject>();
        mAgentArray = new GameObject[mMaxNumberOfAgents];
        System.Random rand = new System.Random();

        for (int i = 0; i < mMaxNumberOfAgents; i++)
        {
            mAgentArray[i] = Instantiate(preyPrefab, gameObject.transform);
            mCurrentNumberOfAgents++;
            RandomizeAgentPosition(mAgentArray[i]);
            AgentAttributes att = mAgentArray[i].GetComponent<AgentController>().mAttributes;
            Debug.Assert(att != null);
            att.mLearningRate = Random.Range(att.mlearningRateMin, att.mlearningRateMax);
            att.mTurnRate = rand.Next(att.mTurnRateStartMin, att.mTurnRateStartMax);

            if (i >= numPreyToSpawn)
            {
                mAgentQueue.Enqueue(mAgentArray[i]);
                mAgentArray[i].SetActive(false);
                mCurrentNumberOfAgents--;
            }
        }
    }

    public void RandomizeAgentPosition(GameObject obj)
    {
        float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
        float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
        obj.transform.position = new Vector3(randX, 1.0f, randZ);
        obj.transform.Rotate(0, Random.Range(0, 360), 0);
    }

    private void Update()
    {
        if (mRespawnPrey)
        {
            mRespawnPrey = false;
        }
    }

    public override GameObject SpawnObject()
    {
        if (mMaxNumberOfAgents > mCurrentNumberOfAgents)
        {
            mCurrentNumberOfAgents++;
            if (mAgentQueue.Count <= 0)
                mAgentQueue.Enqueue(Instantiate(preyPrefab, gameObject.transform));
            GameObject obj = mAgentQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;

        if (mMaxNumberOfAgents > mCurrentNumberOfAgents)
        {
            mCurrentNumberOfAgents++;
            if (mAgentQueue.Count <= 0)
                mAgentQueue.Enqueue(Instantiate(preyPrefab, gameObject.transform));
            GameObject obj = mAgentQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public override void DespawnObject(GameObject obj)
    {
        foreach (GameObject agent in mAgentArray)
        {
            if (agent.activeInHierarchy == true)
            {
                AgentController pc = agent.GetComponent<AgentController>();
                Debug.Assert(pc != null);
                pc.RemoveObjectFromSensingPool(obj.GetComponent<Collider>());
            }
        }
        obj.SetActive(false);
        mAgentQueue.Enqueue(obj);
    }
}
