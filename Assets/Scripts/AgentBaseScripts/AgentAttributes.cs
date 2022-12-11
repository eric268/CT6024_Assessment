using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentAttributes
{
    [Header("Energy")]
    private float mMaxEnergy = 100;
    [SerializeField]
    public float mStartingEnergy;
    [SerializeField]
    private float energyLevel;
    [SerializeField]
    public float mEnergyLevel
    {
        get => energyLevel;
        set => this.energyLevel = Mathf.Clamp(value, 0, mMaxEnergy);
    }

    [Space(15)]
    [SerializeField]
    public float mSpeed;
    public int mObjectsEattenToReproduce;
    public int mCurrentNumObjectsEaten;
    public int mTotalFoodCollected = 0;
    public int mCurrentGeneration = 1;
}


[Serializable]
public class PreyAttributes : AgentAttributes
{

    [SerializeField]
    public float mLearningRate;
    [SerializeField]
    public float mlearningRateMin = 0.05f;
    [SerializeField]
    public float mlearningRateMax = 0.3f;

    [SerializeField]
    public int mTurnRateStartMin;
    [SerializeField]
    public int mTurnRateStartMax;
    public int mTurnRate;
}
