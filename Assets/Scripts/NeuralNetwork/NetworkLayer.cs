using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Container class for a layer in the neural network
//Initializes all the neurons in that layer and holds references to the next and previous layers
//**Holds the weights for each connection from the current layers neurons to next layer neurons**
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
