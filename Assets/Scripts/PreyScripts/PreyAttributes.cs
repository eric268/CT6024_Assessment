using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PreyAttributes// : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField]
    public float mMaxEnergy;
    [SerializeField]
    private float energyLevel;
    [SerializeField]
    public float mEnergyLevel
    {
        get => energyLevel;
        set => this.energyLevel = Mathf.Clamp(value, 0, 100);
    }
    [SerializeField]
    public float mLearningRate;
    [SerializeField]
    public float mlearningRateMin = 0.05f;
    [SerializeField]
    public float mlearningRateMax = 0.3f;

    [Space(15)]
    [SerializeField]
    public float mSpeed;
    public int mFoodRequiredToReplicate;
    public int mCurrentFoodEaten;

    [SerializeField]
    public int mTurnRateStartMin;
    [SerializeField]
    public int mTurnRateStartMax;
    public int mTurnRate;

    public int mTotalFoodCollected = 0;
}
