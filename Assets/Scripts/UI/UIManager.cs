using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mTimeText;
    public TextMeshProUGUI mGenerationText;
    public TextMeshProUGUI mFitnessText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mTimeText.text = "Time: " + MathF.Round(Time.time).ToString();
    }
}
