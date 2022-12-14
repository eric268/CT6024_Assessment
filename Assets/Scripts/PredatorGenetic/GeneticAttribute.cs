using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneticAttribute
{
    public TypeGeneticAttributes mType;
    public float mPointValue;
    public float mPointTotal;
    public float mAttribute;
    public int mNumBasePoints;
    public Action<float> OnAttributeChanged;

    public GeneticAttribute(TypeGeneticAttributes type, float val, int b)
    {
        mType = type;
        mPointValue = val;
        mNumBasePoints= b;
        mPointTotal = mNumBasePoints;
        mAttribute = 0;
    }
}
