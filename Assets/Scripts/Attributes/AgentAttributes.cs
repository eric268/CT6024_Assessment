using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentAttributes
{
    [Header("Energy")]
    [SerializeField]
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

    public int mTotalObjectsEatten = 0;
    public int mCurrentGeneration = 1;
}


[Serializable]
public class PreyAttributes : AgentAttributes
{
    public int mObjectsEattenToReproduce;
    public int mCurrentNumObjectsEaten;

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

    public float mEnergyGivenWhenEaten = 15.0f;
}
[Serializable]
public class PredatorAttributes : AgentAttributes
{
    public bool mIsSprinting;
    public float mSprintMultiplier;
    public float mMateSensingRadius;
    public float mLookingForMateEnergyMultiplier;
    public float mTimeToReproduce;
    public bool mMateFound;
    public float mAngularSpeed;
    public float mMatingDistance;
}

