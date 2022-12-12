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
    public TextMeshProUGUI mNumPreyText;
    public TextMeshProUGUI mNumPredatorsText;

    private AgentSpawner mPreySpawner;
    private AgentSpawner mPredatorSpawner;
    void Awake()
    {
        mPreySpawner = GameObject.FindGameObjectWithTag("PreySpawner").GetComponent<AgentSpawner>();
        mPredatorSpawner = GameObject.FindGameObjectWithTag("PredatorSpawner").GetComponent<AgentSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        mTimeText.text = "Time: " + MathF.Round(Time.time).ToString();
        mNumPreyText.text = "Prey Remaining: " + mPreySpawner.mCurrentNumberOfAgents;
        mNumPredatorsText.text = "Predators Remaining: " + mPredatorSpawner.mCurrentNumberOfAgents;
    }
}
