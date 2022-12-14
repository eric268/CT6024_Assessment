using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Initalizes and updates all prey specific UI elements
public class PreyUIManager : AgentUIManager
{ 
    PreyController preyController;
    TextMeshProUGUI[] mTextContainer;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        base.OnEnable();
        preyController = GetComponentInParent<PreyController>();
        mTextContainer = mAttributeMenu.GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Assert(mNumberOfAttributesToDisplay == mTextContainer.Length);
        SetAttributeText();
    }


    private void Update()
    {
        UpdateHealthBar(preyController.mAttributes.mEnergyLevel, preyController.mAttributes.mMaxEnergy);
    }
    //Initalizes the prey UI text 
    protected override void SetAttributeText()
    {
        mTextContainer[0].text = "Name: " + gameObject.name;
        mTextContainer[1].text = "Current Generation: " + preyController.mAttributes.mCurrentGeneration;
        mTextContainer[2].text = "Food Eaten: " + preyController.mAttributes.mTotalObjectsEatten;
        mTextContainer[3].text = "Learning Rate: " + (int)(preyController.mAttributes.mLearningRate * 100) + "%";
        mTextContainer[4].text = "Turn Rate: " + preyController.mAttributes.mTurnRate;
    }

    //Updates the prey UI text that can change over time
    protected override void UpdateAttributeText()
    {
        mTextContainer[2].text = "Food Eaten: " + preyController.mAttributes.mTotalObjectsEatten;
    }
}
