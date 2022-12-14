using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void SetGameTimeScale(int time)
    {
        Time.timeScale = time;
    }
}
