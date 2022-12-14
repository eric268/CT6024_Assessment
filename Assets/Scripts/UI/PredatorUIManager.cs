using AIGOAP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PredatorUITypes
{
    General,
    Genetics,
    GOB,
    NUM_PREDATOR_UI_TYPES
}


public class PredatorUIManager : AgentUIManager
{
    PredatorController mPredatorController;
    PredatorUIText[] mAttributeContainer;
    TextMeshProUGUI[] mGeneralText;
    TextMeshProUGUI[] mGeneticText;
    TextMeshProUGUI[] mGOBText;
    Button[] mButtons;

    [SerializeField]
    private int mNumberGeneralTextItems;
    [SerializeField]
    private int mNumberGenticTextItems;
    [SerializeField]
    private int mNumberGOBTextItems;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        base.OnEnable();
        mPredatorController = GetComponentInParent<PredatorController>();
        mButtons = mAttributeMenu.gameObject.GetComponentsInChildren<Button>();
        Debug.Assert(mButtons.Length == (int)PredatorUITypes.NUM_PREDATOR_UI_TYPES);
        SetAttributeText();
        mButtons[(int)PredatorUITypes.General].onClick.AddListener(delegate () { OnGeneralButtonPressed(); });
        mButtons[(int)PredatorUITypes.Genetics].onClick.AddListener(delegate () { OnGeneticsButtonPressed(); });
        mButtons[(int)PredatorUITypes.GOB].onClick.AddListener(delegate () { OnGOBButtonPressed(); });
    }

    void OnGeneralButtonPressed()
    {
        HideAllAttributes();
        mAttributeContainer[(int)PredatorUITypes.General].gameObject.SetActive(true);
    }

    void OnGeneticsButtonPressed()
    {
        HideAllAttributes();
        mAttributeContainer[(int)PredatorUITypes.Genetics].gameObject.SetActive(true);
    }

    void OnGOBButtonPressed()
    {
        HideAllAttributes();
        mAttributeContainer[(int)PredatorUITypes.GOB].gameObject.SetActive(true);
    }

    void HideAllAttributes()
    {
        foreach (PredatorUIText text in mAttributeContainer)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateHealthBar(mPredatorController.mAttributes.mEnergyLevel, mPredatorController.mAttributes.mMaxEnergy);
        UpdateAttributeText();
    }

    protected override void SetAttributeText()
    {
        mAttributeContainer = mAttributeMenu.GetComponentsInChildren<PredatorUIText>();
        Debug.Assert(mAttributeContainer != null && mAttributeContainer.Length == (int)PredatorUITypes.NUM_PREDATOR_UI_TYPES);
        LoadText();
        SetGeneralText();
        SetGeneticText();
        SetGOBText();
        HideAllAttributes();
        mAttributeContainer[(int)PredatorUITypes.General].gameObject.SetActive(true);
    }

    protected override void UpdateAttributeText()
    {
        if (mAttributeContainer[(int)PredatorUITypes.General].isActiveAndEnabled)
        {
            mGeneralText[2].text = "Food Eaten: " + mPredatorController.mAttributes.mTotalObjectsEatten;
        }
        else if (mAttributeContainer[(int)PredatorUITypes.Genetics].isActiveAndEnabled)
        {
            SetGeneticText();
        }
        else
        {
            SetGOBText();
        }
    }

    private void SetGeneralText()
    {
        mGeneralText[0].text = "Name: " + gameObject.name;
        mGeneralText[1].text = "Current Generation: " + mPredatorController.mAttributes.mCurrentGeneration;
        mGeneralText[2].text = "Food Eaten: " + mPredatorController.mAttributes.mTotalObjectsEatten;
    }

    private void SetGeneticText()
    {
        mGeneticText[0].text = "Speed: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.Speed].mPointTotal;
        mGeneticText[1].text = "Angular Speed: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.AngularSpeed].mPointTotal;
        mGeneticText[2].text = "Mate Sensing: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.MateSensingRadius].mPointTotal;
        mGeneticText[3].text = "Wall Sensing: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.WallSensing].mPointTotal;
        mGeneticText[4].text = "Far Sensing FOV: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingAngle].mPointTotal;
        mGeneticText[5].text = "Far Sensing Radius: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.FarSensingRadius].mPointTotal;
        mGeneticText[6].text = "Close Sensing FOV: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingAngle].mPointTotal;
        mGeneticText[7].text = "Close Sensing Radius: " + mPredatorController.mGeneticManager.mGeneticAttributes[(int)TypeGeneticAttributes.CloseSensingRadius].mPointTotal;
    }

    private void SetGOBText()
    {
        mGOBText[0].text = "Hunt: " + (int)mPredatorController.mGOB.mGoalArray[(int)GoalTypes.Eat].mValue;
        mGOBText[1].text = "Sleep: " + (int)mPredatorController.mGOB.mGoalArray[(int)GoalTypes.Sleep].mValue;
        mGOBText[2].text = "Reproduce: " + (int)mPredatorController.mGOB.mGoalArray[(int)GoalTypes.Reproduce].mValue;
    }

    void LoadText()
    {
        mGeneralText = mAttributeContainer[(int)PredatorUITypes.General].GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Assert(mGeneralText.Length == mNumberGeneralTextItems);

        mGeneticText = mAttributeContainer[(int)PredatorUITypes.Genetics].GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Assert(mGeneticText.Length == mNumberGenticTextItems);

        mGOBText = mAttributeContainer[(int)PredatorUITypes.GOB].GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Assert(mGOBText.Length == mNumberGOBTextItems);
    }
}
