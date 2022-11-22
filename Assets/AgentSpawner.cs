using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;



public class AgentSpawner : MonoBehaviour
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

    public GameObject SpawnAgent(GameObject prefab)
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
    }

    public void ReturnPreyToPool(GameObject obj)
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

    private void Update()
    {
        if (mRespawnPrey)
        {
            mRespawnPrey = false;
        }
        //if (GameManagerScript.currentNumberOfPrey == 0)
        //{
        //    FindBestPrey();
        //}
    }

    //void FindBestPrey()
    //{
    //    GameObject[] sortedArray = mAgentArray.OrderByDescending(c => c.GetComponent<AgentController>().mAttributes.mTotalFoodCollected).ToArray();
    //    int topInFitness = 25;
    //    for(int i =0; i < mRespawnAmount; i++)
    //    {
    //        int obj = Random.Range(0, topInFitness);
    //        GameObject t2 = sortedArray[obj].GetComponent<AgentController>().SplitPreyInstant();
    //        float randX = Random.Range(-groundPosition.localScale.x * 3.5f, groundPosition.localScale.x * 3.5f);
    //        float randZ = Random.Range(-groundPosition.localScale.z * 3.5f, groundPosition.localScale.z * 3.5f);
    //        t2.transform.position = new Vector3(randX, 1.0f, randZ);
    //    }

    //    mainUI.mGenerationText.text = "Current Generation: " + ++mCurrentGen;
    //    mainUI.mFitnessText.text = "Highest Fitness: " + sortedArray[0].GetComponent<AgentController>().mAttributes.mTotalFoodCollected;

    //    foreach (GameObject obj in sortedArray)
    //    {
    //        obj.GetComponent<AgentController>().mAttributes.mTotalFoodCollected = 0;
    //    }
    //}
}
