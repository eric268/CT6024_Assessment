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
    public bool mIsAdditive;
    public Action<float> OnAttributeChanged;

    public GeneticAttribute(TypeGeneticAttributes type, float val, bool b)
    {
        mType = type;
        mPointValue = val;
        mIsAdditive= b;
        mPointTotal = 1;
        mAttribute = 0;
    }
}
