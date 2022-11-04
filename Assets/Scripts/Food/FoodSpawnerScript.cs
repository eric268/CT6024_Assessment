using System.Collections;
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
    float spawnRate = 1.0f;
    int nextItemToSpawn;


    [SerializeField]
    Transform groundPosition;

    public static Queue<GameObject> foodPool;
    // Start is called before the first frame update
    void Start()
    {
        foodPool = new Queue<GameObject>();
        nextItemToSpawn = foodPoolSize / 2;
        InvokeRepeating(nameof(SpawnFood), 0.0f, spawnRate);
        for (int i =0; i < startingFoodAmount; i++)
        {
            SpawnFood();
        }
    }

    void SpawnFood()
    {
        if (foodPool.Count <= 0)
        {
            foodPool.Enqueue(Instantiate(foodPrefab, gameObject.transform));
        }
        GameObject food = foodPool.Dequeue();
        food.SetActive(true);
        float randX = Random.Range(-groundPosition.localScale.x * 4.0f, groundPosition.localScale.x * 4.0f);
        float randZ = Random.Range(-groundPosition.localScale.z * 4.0f, groundPosition.localScale.z * 4.0f);
        food.transform.position = new Vector3(randX, 0.5f, randZ);
    }

    public static void ReturnFood(GameObject obj)
    {
        obj.SetActive(false);
        foodPool.Enqueue(obj);
    }
}
