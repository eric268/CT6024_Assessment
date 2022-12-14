using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which houses the activation and bias of each network node
//Bias is initalized with a small random value 
public class Neuron
{
    public double mActivation;
    public double mBias;
    public Neuron()
    {
        System.Random rand = new System.Random();
        mBias =  0.00001 * rand.Next(-10000, 10000);
        mActivation = 0.0;
    }
}
