using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager mGameManager;
    public TextMeshProUGUI mTimeText;
    public TextMeshProUGUI mGenerationText;
    public TextMeshProUGUI mFitnessText;
    public TextMeshProUGUI mNumPreyText;
    public TextMeshProUGUI mNumPredatorsText;
    public Slider mTimeSlider;

    [SerializeField]
    private AgentSpawner mPreySpawner;
    [SerializeField]
    private AgentSpawner mPredatorSpawner;
    void Awake()
    {
        mGameManager = FindObjectOfType<GameManager>();
        mTimeSlider.onValueChanged.AddListener(delegate { OnSliderTimeChange(mTimeSlider); });
    }

    // Update is called once per frame
    void Update()
    {
        mTimeText.text = "Time: " + MathF.Round(Time.time).ToString();
        mNumPreyText.text = "Prey Remaining: " + mPreySpawner.mCurrentNumberOfAgents;
        mNumPredatorsText.text = "Predators Remaining: " + mPredatorSpawner.mCurrentNumberOfAgents;
    }

    void OnSliderTimeChange(Slider slider)
    {
        Debug.Log("V");
        mGameManager.SetGameTimeScale((int)slider.value);
    }
}
