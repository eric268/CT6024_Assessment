using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLayer
{
    public NetworkLayer(int numOfNeurons)
    {
        mNumberOfNeurons = numOfNeurons;
        mNeurons = new Neuron[mNumberOfNeurons];
        for (int i =0; i < mNeurons.Length; i++)
        {
            mNeurons[i] = new Neuron();
        }
    }
    public int mNumberOfNeurons;
    public Neuron[] mNeurons;
    public NetworkLayer mNextLayer;
    public NetworkLayer mPreviousLayer;
    public double[,] mWeights;
}
