using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    Transform groundPosition;

    [SerializeField]
    int numPreyToSpawn = 1000;
    [SerializeField]
    GameObject preyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i < numPreyToSpawn; i++)
        {
            SpawnAgent(preyPrefab);
        }
    }


    void SpawnAgent(GameObject prefab)
    {
        GameObject obj = Instantiate(preyPrefab, gameObject.transform);
        float randX = Random.Range(-groundPosition.localScale.x * 5f, groundPosition.localScale.x * 5f);
        float randZ = Random.Range(-groundPosition.localScale.z * 5f, groundPosition.localScale.z * 5f);
        obj.transform.position = new Vector3(randX, 1.0f, randZ);
    }
}
