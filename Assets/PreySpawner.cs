using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PreySpawner : MonoBehaviour
{
    [SerializeField]
    Transform groundPosition;

    [SerializeField]
    int numPreyToSpawn = 100;
    [SerializeField]
    GameObject preyPrefab;

    public static Queue<GameObject> preyPool;
    public static GameObject[] mPreyArray;

    [Header("Generation Debug")]
    public bool mRespawnPrey = false;
    public int mRespawnAmount = 100;

    UIManager mainUI;

    public int mCurrentGen = 1;
    // Start is called before the first frame update

    private void Awake()
    {
        mainUI = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<UIManager>();
        preyPool = new Queue<GameObject>();
        mPreyArray = new GameObject[GameManagerScript.maxNumberOfPrey];
        System.Random rand = new System.Random();
        for (int i = 0; i < GameManagerScript.maxNumberOfPrey; i++)
        {
            mPreyArray[i] = SpawnAgent(preyPrefab);
            //int rot = rand.Next(360);
            //obj.transform.Rotate(0.0f, rot, 0.0f);
            PreyAttributes att = mPreyArray[i].GetComponent<PreyController>().mAttributes;
            Debug.Assert(att != null);
            att.mLearningRate = Random.Range(att.mlearningRateMin, att.mlearningRateMax);
            //att.mTurnRate = rand.Next(att.mTurnRateStartMin, att.mTurnRateStartMax);
            if (i >= numPreyToSpawn)
            {
                mPreyArray[i].SetActive(false);
                GameManagerScript.currentNumberOfPrey--;
            }
        }
    }

    public GameObject SpawnAgent(GameObject prefab)
    {
        if (GameManagerScript.maxNumberOfPrey > GameManagerScript.currentNumberOfPrey)
        {
            GameManagerScript.currentNumberOfPrey++;
            if (preyPool.Count <= 0)
                preyPool.Enqueue(Instantiate(preyPrefab, gameObject.transform));

            GameObject obj = preyPool.Dequeue();
            obj.SetActive(true);
            float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
            float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
            obj.transform.position = new Vector3(randX, 1.0f, randZ);
            return obj;
        }
        return null;
    }

    public static void ReturnPreyToPool(GameObject obj)
    {
        GameManagerScript.currentNumberOfPrey--;
        obj.SetActive(false);
        preyPool.Enqueue(obj);
    }

    private void Update()
    {
        if (GameManagerScript.currentNumberOfPrey == 0)
        {
            FindBestPrey();
        }
    }

    void FindBestPrey()
    {
        GameObject[] sortedArray = mPreyArray.OrderByDescending(c => c.GetComponent<PreyController>().mAttributes.mTotalFoodCollected).ToArray();
        int topInFitness = 25;
        for(int i =0; i < mRespawnAmount; i++)
        {
            int obj = Random.Range(0, topInFitness);
            GameObject t2 = sortedArray[obj].GetComponent<PreyController>().SplitPreyInstant();
            float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
            float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
            t2.transform.position = new Vector3(randX, 1.0f, randZ);
        }

        mainUI.mGenerationText.text = "Current Generation: " + ++mCurrentGen;
        mainUI.mFitnessText.text = "Highest Fitness: " + sortedArray[0].GetComponent<PreyController>().mAttributes.mTotalFoodCollected;

        foreach (GameObject obj in sortedArray)
        {
            obj.GetComponent<PreyController>().mAttributes.mTotalFoodCollected = 0;
        }
    }

    private void OnApplicationQuit()
    {

    }

}

public class TrainedWeightAndBiasData
{

}

