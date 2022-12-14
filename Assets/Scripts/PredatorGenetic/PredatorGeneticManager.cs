using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

//Manages all genetic attributes for predator class
[Serializable]
public class PredatorGeneticManager
{
    public int mNumberOfStartingPoints;
    public int mNumberOfBasePointsInAttribute;
    public List<GeneticAttribute> mGeneticAttributes;
    
    //Constructor that is called when scene is initialized 
    //A max number of points are semi-randomly spread across attributes
    //I say semi-randomly as some base points are evenly spread 
    public PredatorGeneticManager(int numPoints)
    {
        mNumberOfBasePointsInAttribute = 5;
        mNumberOfStartingPoints = numPoints;
        Initalize();
        AddPointsRandomlyToAttributes();
    }
    //This is the constructor that is called when a predator agent is spawned from a parent
    //It takes in the point allocation split across both parents
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
    //Adds the attributes to the manager
    //The second parameter in the constructor is the value that one point of that attribute provides the agent
    //For example one point in the MateSensing attribue will increase the radius that it can sense mates by 10 units
    void Initalize()
    {
        mGeneticAttributes = new List<GeneticAttribute>();
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.MateSensingRadius, 10.0f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.Speed, 0.55f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.AngularSpeed, 25.0f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.WallSensing, 2.0f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingAngle, 3.5f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.FarSensingRadius, 4.0f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingAngle, 6.5f, mNumberOfBasePointsInAttribute));
        mGeneticAttributes.Add(new GeneticAttribute(TypeGeneticAttributes.CloseSensingRadius, 0.5f, mNumberOfBasePointsInAttribute));
    }

    //Sets the attributes value and calls the action for any listening functions
    public void BroadcastAllAttributes()
    {
        for(int i =0; i < mGeneticAttributes.Count; i++) 
        {
            SetAttributeValues(i);
        }
    }
    //Randomly allocates 
    void AddPointsRandomlyToAttributes()
    {
        //All attributes state with a certain amount of base points.
        //Therefore need to multiply the number of base points allocated * number of total attributes so the point total will be correct
        for (int i = 0; i < mNumberOfStartingPoints - mGeneticAttributes.Count * mNumberOfBasePointsInAttribute; i++)
        {
            int rand = UnityEngine.Random.Range(0, (int)TypeGeneticAttributes.NUM_GENETIC_ATTRITBUES);
            mGeneticAttributes[rand].mPointTotal++;
        }
    }
    //Calculates the attributes total value and calls the action for any listening functions
    void SetAttributeValues(int i)
    {
        mGeneticAttributes[i].mAttribute = mGeneticAttributes[i].mPointValue * mGeneticAttributes[i].mPointTotal;
        if (mGeneticAttributes[i].OnAttributeChanged != null)
        {
            mGeneticAttributes[i].OnAttributeChanged(mGeneticAttributes[i].mAttribute);
        }
    }
    //Adds a point into attribute and calls the OnValueChanged() action
    public bool ImproveAttribute(int att)
    {
        Debug.Assert(att >=0 && att < mGeneticAttributes.Count);

        //Can add check for a max value and return false if that occurs and not add a point
        mGeneticAttributes[att].mPointTotal++;
        SetAttributeValues(att);

        return true;
    }
    //attempts to remove a point from an attribute
    //Will return true if successful and false if that attribute has 1 or less points in that attribute
    //This is used to ensure that no attribute can fall to 0 or below
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
    //Returns the first half of the point allocation from a parent
    public List<float> GetFirstHalfGenetics()
    { 
        return new List<float>(mGeneticAttributes.GetRange(0, mGeneticAttributes.Count / 2).Select(x => x.mPointTotal));
    }
    //Returns the second half of the point allocation from a parent
    public List<float> GetSecondHalfGenetics()
    {
        return new List<float>(mGeneticAttributes.GetRange(mGeneticAttributes.Count / 2, mGeneticAttributes.Count / 2).Select(x => x.mPointTotal));
    }

}
