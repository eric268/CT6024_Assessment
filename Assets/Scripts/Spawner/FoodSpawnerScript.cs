using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;


public class FoodSpawnerScript : MonoBehaviour
{
    [SerializeField]
    int foodPoolSize = 50;
    [SerializeField]
    int startingFoodAmount = 25;
    [SerializeField]
    GameObject foodPrefab;
    [SerializeField]
    float spawnRate;
    [SerializeField]
    float spawnRateDecreaseFrequency = 60.0f;
    int nextItemToSpawn;
    [SerializeField]
    float maxSpawnRate;
    [SerializeField]
    AgentSpawner mPreySpawner;

    public static int mMaxFoodOnMap = 500;
    public static int mCurrentAmountofFoodOnMap = 0;

    [SerializeField]
    Transform groundPosition;

    public static Queue<GameObject> foodPool;

    private void Awake()
    {
        mPreySpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
    }
    // Start is called before the first frame update
    void Start ()
    {
        foodPool = new Queue<GameObject>();
        nextItemToSpawn = foodPoolSize / 2;
        InvokeRepeating(nameof(SpawnFood), 0.0f, spawnRate);
        for (int i =0; i < startingFoodAmount; i++)
        {
            SpawnFood();
        }
        InvokeRepeating(nameof(DecreaseSpawnRate), spawnRateDecreaseFrequency, spawnRateDecreaseFrequency);
    }

    void SpawnFood()
    {
        if (mCurrentAmountofFoodOnMap >= mMaxFoodOnMap)
            return;

        mCurrentAmountofFoodOnMap++;

        if (foodPool.Count <= 0)
        {
            foodPool.Enqueue(Instantiate(foodPrefab, gameObject.transform));
        }
        GameObject food = foodPool.Dequeue();
        food.SetActive(true);
        float randX = Random.Range(-groundPosition.localScale.x * 4.5f, groundPosition.localScale.x * 4.5f);
        float randZ = Random.Range(-groundPosition.localScale.z * 4.5f, groundPosition.localScale.z * 4.5f);
        food.transform.position = new Vector3(randX, 0.5f, randZ);
    }

    void DecreaseSpawnRate()
    {
        if (spawnRate > maxSpawnRate)
            return;
        spawnRate += 0.05f;
        Debug.Log("Spawn Rate Decreased to: " + spawnRate);
        CancelInvoke(nameof(SpawnFood));
        InvokeRepeating(nameof(SpawnFood), 0.0f, spawnRate);
    }

    public void ReturnFood(GameObject obj)
    {
        obj.SetActive(false);
        foodPool.Enqueue(obj);
    }
}
