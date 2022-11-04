using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLayer
{
    public NetworkLayer(int numOfNeurons)
    {
        mNumberOfNeurons = numOfNeurons;
        mNeurons = new Neuron[mNumberOfNeurons];   
    }
    public int mNumberOfNeurons;
    Neuron[] mNeurons;
}
