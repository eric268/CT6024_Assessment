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
        System.Random rand = new System.Random();
        for(int i =0; i < numPreyToSpawn; i++)
        {
            SpawnAgent(preyPrefab);
            int rot = rand.Next(360);
            preyPrefab.transform.Rotate(0.0f, rot, 0.0f);
        }
    }


    void SpawnAgent(GameObject prefab)
    {
        GameObject obj = Instantiate(preyPrefab, gameObject.transform);
        float randX = Random.Range(-groundPosition.localScale.x * 3f, groundPosition.localScale.x * 3f);
        float randZ = Random.Range(-groundPosition.localScale.z * 3f, groundPosition.localScale.z * 3f);
        obj.transform.position = new Vector3(randX, 1.0f, randZ);
    }
}
