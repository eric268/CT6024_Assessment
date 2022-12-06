using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDespawnObject
{
    GameObject SpawnObject();
    void DespawnObject(GameObject obj);
}


public abstract class SpawnerScript : MonoBehaviour, IDespawnObject
{
    public abstract GameObject SpawnObject();
    public abstract void DespawnObject(GameObject obj);
}