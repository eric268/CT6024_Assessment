using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the timescale of the simluation
public class GameManager : MonoBehaviour
{
    public void SetGameTimeScale(int time)
    {
        Time.timeScale = time;
    }
}
