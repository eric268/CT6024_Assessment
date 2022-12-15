using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

//Spawns all the prey food items on the map
public class FoodSpawnerScript : MonoBehaviour
{
    [SerializeField]
    private int foodPoolSize = 50;
    [SerializeField]
    private int startingFoodAmount = 25;
    [SerializeField]
    private GameObject foodPrefab;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    private float spawnRateDecreaseFrequency = 60.0f;
    private int nextItemToSpawn;
    [SerializeField]
    private float maxSpawnRate;

    public static int mMaxFoodOnMap = 500;
    public static int mCurrentAmountofFoodOnMap = 0;

    [SerializeField]
    Transform groundPosition;

    public static Queue<GameObject> foodPool;
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
    //Checks if max number of food items have been spawned
    //Spawns specified amount of food on map
    void SpawnFood()
    {
        if (mCurrentAmountofFoodOnMap >= mMaxFoodOnMap)
            return;

        mCurrentAmountofFoodOnMap++;
        //If no more food items are in object pool queue will create one
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
    //Overtime amount of food spawned will decrease up to a limit
    //This is to give added pressure to prey neural network
    void DecreaseSpawnRate()
    {
        if (spawnRate > maxSpawnRate)
            return;
        spawnRate += 0.05f;
        Debug.Log("Food spawn rate decreased");
        CancelInvoke(nameof(SpawnFood));
        InvokeRepeating(nameof(SpawnFood), 0.0f, spawnRate);
    }
    //De-spawns food item and adds it to object pool queue
    public void ReturnFood(GameObject obj)
    {
        obj.SetActive(false);
        foodPool.Enqueue(obj);
    }
}
