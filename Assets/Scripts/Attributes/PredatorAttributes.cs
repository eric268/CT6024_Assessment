using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which manages all predator attributes except for genetic attributes which as handled in PredatorGeneticManager
[Serializable]
public class PredatorAttributes : AgentAttributes
{
    public bool mIsSprinting;
    public float mLookingForMateEnergyMultiplier;
    public bool mMateFound;
    public float mMatingDistance;
    public float mTimeToReproduce;
    public int mBonusGeneticPoints;
    public bool mIsLookingForMate;
}
