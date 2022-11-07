using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public double mActivation;
    public double mBias;
    public Neuron()
    {
        System.Random rand = new System.Random();
        mBias = 0.0f;// 0.00001 * rand.Next(-10000, 10000);
        mActivation = 0.0;
    }
}
