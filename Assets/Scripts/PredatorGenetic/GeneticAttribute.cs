using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which contains the information regarding genetic attributes
//Genetic attributes work on a point system
//When an agent receives a point in an attribute that attribute is increased by a set amount
//Attributes are initialized with a base value
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
