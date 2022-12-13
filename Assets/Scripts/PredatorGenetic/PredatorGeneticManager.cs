using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PredatorGeneticManager
{
    [SerializeField]
    int mNumberOfStartingPoints;

    public List<GeneticAttribute> mGeneticAttributes;
    
    public PredatorGeneticManager(int numPoints)
    {
        mNumberOfStartingPoints = numPoints;
       mGeneticAttributes = new List<GeneticAttribute>();
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.Sprint, 0.07f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.MateSensingRadius, 7.0f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.Speed, 0.5f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.AngularSpeed, 20.0f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingAngle, 3.5f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingRadius, 4.0f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingAngle, 6.5f, true));
       mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingRadius, 0.5f, true));

        AddPointsRandomlyToAttributes();
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
        for (int i = 0; i < mNumberOfStartingPoints; i++)
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

    public List<GeneticAttribute> GetFirstHalfGenetics()
    {
        return new List<GeneticAttribute>(mGeneticAttributes.GetRange(0, mGeneticAttributes.Count / 2));
    }

    public List<GeneticAttribute> GetSecondHalfGenetics()
    {
        return new List<GeneticAttribute>(mGeneticAttributes.GetRange(mGeneticAttributes.Count / 2, mGeneticAttributes.Count / 2));
    }

}
