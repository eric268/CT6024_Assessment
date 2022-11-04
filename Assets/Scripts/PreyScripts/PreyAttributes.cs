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
        set => this.energyLevel = Mathf.Clamp(value, 0, mMaxEnergy);
    }

    [Space(15)]
    [SerializeField]
    public float mSpeed;
    public float mReproductionRate;
    public float mTimeUntilReproduction;

}
