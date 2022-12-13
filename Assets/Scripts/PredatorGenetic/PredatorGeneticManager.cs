using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public class PredatorGeneticManager
{
    public int mNumberOfStartingPoints;

    public List<GeneticAttribute> mGeneticAttributes;
    
    public PredatorGeneticManager(int numPoints)
    {
        mNumberOfStartingPoints = numPoints;
        Initalize();
        AddPointsRandomlyToAttributes();
    }

    public PredatorGeneticManager(List<float> mPointTotal)
    {
        Initalize();
        mNumberOfStartingPoints = mGeneticAttributes.Count;
        for(int i = 0; i < mGeneticAttributes.Count; i++)
        {
            mGeneticAttributes[i].mPointTotal = mPointTotal[i];
            mNumberOfStartingPoints += (int)mPointTotal[i];
        }
    }

    void Initalize()
    {
        mGeneticAttributes = new List<GeneticAttribute>();
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.Sprint, 0.07f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.MateSensingRadius, 7.0f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.Speed, 0.5f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.AngularSpeed, 20.0f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingAngle, 3.5f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingRadius, 4.0f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingAngle, 6.5f, true));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingRadius, 0.5f, true));
    }

    public void BroadcastAllAttributes()
    {
        for(int i =0; i < mGeneticAttributes.Count; i++) 
        {
            SetAttributeValues(i);
        }
    }

    void AddPointsRandomlyToAttributes()
    {
        //All attributes state with a base point of 1 therefore need to subtract the total number of attributes so the total will be correct
        for (int i = 0; i < mNumberOfStartingPoints - mGeneticAttributes.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            mGeneticAttributes[rand].mPointTotal++;
        }
    }

    void SetAttributeValues(int i)
    {
        mGeneticAttributes[i].mAttribute = mGeneticAttributes[i].mPointValue * mGeneticAttributes[i].mPointTotal;
        if (mGeneticAttributes[i].OnAttributeChanged != null)
        {
            mGeneticAttributes[i].OnAttributeChanged(mGeneticAttributes[i].mAttribute);
        }
    }

    public bool ImproveAttribute(int att)
    {
        Debug.Assert(att >=0 && att < mGeneticAttributes.Count);

        //Can add check for a max value and return false if that occurs and not add a point
        mGeneticAttributes[att].mPointTotal++;
        SetAttributeValues(att);

        return true;
    }

    public bool DeteriorateAttribute(int att)
    {
        Debug.Assert(att >= 0 && att < mGeneticAttributes.Count);

        if (mGeneticAttributes[att].mPointTotal > 1)
        {
            mGeneticAttributes[att].mPointTotal--;
            SetAttributeValues(att);
            return true;
        }
        return false;
    }

    public List<float> GetFirstHalfGenetics()
    { 
        return new List<float>(mGeneticAttributes.GetRange(0, mGeneticAttributes.Count / 2).Select(x => x.mPointTotal));
    }

    public List<float> GetSecondHalfGenetics()
    {
        return new List<float>(mGeneticAttributes.GetRange(mGeneticAttributes.Count / 2, mGeneticAttributes.Count / 2).Select(x => x.mPointTotal));
    }

}
