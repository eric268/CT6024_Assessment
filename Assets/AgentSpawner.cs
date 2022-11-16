using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    Transform groundPosition;

    [SerializeField]
    int numPreyToSpawn = 100;
    [SerializeField]
    GameObject preyPrefab;

    public static Queue<GameObject> preyPool;
    public GameObject[] mPreyArray;
    // Start is called before the first frame update

    private void Awake()
    {
        preyPool = new Queue<GameObject>();
        mPreyArray = new GameObject[GameManagerScript.maxNumberOfPrey];
        System.Random rand = new System.Random();
        for (int i = 0; i < GameManagerScript.maxNumberOfPrey; i++)
        {
            GameObject obj = SpawnAgent(preyPrefab);
            int rot = rand.Next(360);
            obj.transform.Rotate(0.0f, rot, 0.0f);
            mPreyArray[i] = obj;
            PreyAttributes att = obj.GetComponent<PreyController>().mAttributes;
            att.mTurnRate = rand.Next(att.mTurnRateStartMin, att.mTurnRateStartMax);
            if (i > numPreyToSpawn)
            {
                obj.SetActive(false);
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
}
