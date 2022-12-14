using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Manages the general simulation UI elements
public class UIManager : MonoBehaviour
{
    GameManager mGameManager;
    public TextMeshProUGUI mTimeText;
    public TextMeshProUGUI mGenerationText;
    public TextMeshProUGUI mFitnessText;
    public TextMeshProUGUI mNumPreyText;
    public TextMeshProUGUI mNumPredatorsText;
    public TextMeshProUGUI mTimeSliderText;
    public Slider mTimeSlider;

    [SerializeField]
    private AgentSpawner mPreySpawner;
    [SerializeField]
    private AgentSpawner mPredatorSpawner;
    void Awake()
    {
        mGameManager = FindObjectOfType<GameManager>();
        mTimeSlider.onValueChanged.AddListener(delegate { OnSliderTimeChange(mTimeSlider); });
        mTimeSliderText.text = "Time Scale: " + (int)mTimeSlider.value;
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
        mTimeSliderText.text = "Time Scale: " + (int)slider.value;
        mGameManager.SetGameTimeScale((int)slider.value);
    }
}
