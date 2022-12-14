using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which manages all prey attributes
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
