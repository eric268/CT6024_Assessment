using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSCheckScript : MonoBehaviour
{
    public Transform[] LOSCheckPositions;
    // Start is called before the first frame update
    void Start()
    {
        LOSCheckPositions = new Transform[transform.childCount];
        for (int i =0; i < transform.childCount; i++)
        {
            LOSCheckPositions[i] = transform.GetChild(i).transform;
        } 
    }
}
