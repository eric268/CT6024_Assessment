using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Base class for agents attributes
//Child classes consist of PredatorAttributes & PreyAttributes
public class AgentAttributes
{
    [Header("Energy")]
    [SerializeField]
    public float mMaxEnergy = 100;
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

