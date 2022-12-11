using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float mTimeSpeed;
    private void OnValidate()
    {
        Time.timeScale = mTimeSpeed;
    }
}
