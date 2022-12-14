using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro.EditorUtilities;
using System.Linq;
using UnityEngine.UIElements;

//Class which contains the framework for the preys neural network
public class NeuralNetwork
{
    public NetworkLayer[] mNetworkLayers;
    public NeuralNetwork(int[] layerSizes)
    {
        Debug.Assert(layerSizes.Length > 0);
        mNetworkLayers = new NetworkLayer[layerSizes.Length];
        mNetworkLayers[0] = new NetworkLayer(layerSizes[0]);

        for(int i = 1; i < layerSizes.Length; i++)
        {
            mNetworkLayers[i] = new NetworkLayer(layerSizes[i]);
            mNetworkLayers[i - 1].mNextLayer = mNetworkLayers[i];
            mNetworkLayers[i].mPreviousLayer = mNetworkLayers[i - 1];
            SetNeuronWeightsInLayer(mNetworkLayers[i-1]);
        }
    }
    //Takes in the networks inputs and returns the neuron with the highest activation from the output layer
    public int RunNetwork(List<double> input)
    {
        Debug.Assert(mNetworkLayers.Length > 0 && mNetworkLayers[0].mNeurons.Length == input.Count);
        for (int i =0; i < input.Count; i++)
        {
            mNetworkLayers[0].mNeurons[i].mActivation = input[i];
        }
        //Only want to forward propagate from the second last layer to the last layer 
        //Otherwise no layer will exist to propagate to
        for (int i = 0; i < mNetworkLayers.Length -1; i++)
        {
            ForwardPropigateActivation(mNetworkLayers[i]);
        }
        //returns an integer indicating the neuron with the highest activation
        //Only want to pass in the last layer (output layer) from the network
        return GetFinalOutput(mNetworkLayers[mNetworkLayers.Length - 1]);
    }
    //Initializes the container for the network layers weights
    //Sets these to a small random value
    void SetNeuronWeightsInLayer(NetworkLayer currentLayer)
    {
        if (currentLayer == null || currentLayer.mNextLayer == null)
            return;

        currentLayer.mWeights = new double[currentLayer.mNextLayer.mNumberOfNeurons, currentLayer.mNumberOfNeurons];
        System.Random rand = new System.Random();

        for (int i =0; i < currentLayer.mWeights.GetLength(0); i++)
        {
            for (int j =0; j < currentLayer.mWeights.GetLength(1);j++)
            {
                currentLayer.mWeights[i, j] = 0.00001 * rand.Next(-30000, 30000);
            }
        }
    }
    //Calculates and sets the activation of the next layers neurons
    //This is based on the sum of the weighted sum of the previous layers activation plus the layers bias
    void ForwardPropigateActivation(NetworkLayer currentLayer)
    {
        for (int i = 0; i < currentLayer.mWeights.GetLength(0); i++)
        {
            currentLayer.mNextLayer.mNeurons[i].mActivation = 0.0;
            for (int j = 0; j < currentLayer.mWeights.GetLength(1); j++)
            {
                currentLayer.mNextLayer.mNeurons[i].mActivation += currentLayer.mWeights[i, j] * currentLayer.mNeurons[j].mActivation;
            }
            currentLayer.mNextLayer.mNeurons[i].mActivation = ReLU(currentLayer.mNextLayer.mNeurons[i].mActivation + currentLayer.mNextLayer.mNeurons[i].mBias);
        }
    }
    //Creates a copy of the parents neural network with a chance to mutate the network
    //This mutation can either increase or decrease each weight and bias
    //This is based on a learning rate which is also inherited and possibly mutated from the parent
    //A higher learning rate leads to more drastic changes
    public void CopyAndMutateNetwork(NetworkLayer[] layersToCopy, float mLerningRate)
    {
        System.Random rand = new System.Random();
        for (int k = 0; k < mNetworkLayers.Length; k++)
        {
            if (mNetworkLayers[k].mNextLayer == null)
            {
                for (int j = 0; j < mNetworkLayers[k].mNumberOfNeurons; j++)
                {
                    mNetworkLayers[k].mNeurons[j].mBias = layersToCopy[k].mNeurons[j].mBias;
                    if (rand.Next(2) == 0)
                    {
                        mNetworkLayers[k].mNeurons[j].mBias += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                    }
                }
                return;
            }

            bool doOnce = true;
            for (int i = 0; i < mNetworkLayers[k].mWeights.GetLength(0); i++)
            {
                for (int j = 0; j < mNetworkLayers[k].mWeights.GetLength(1); j++)
                {

                    mNetworkLayers[k].mWeights[i, j] = layersToCopy[k].mWeights[i, j];
                    if (doOnce)
                        mNetworkLayers[k].mNeurons[j].mBias = layersToCopy[k].mNeurons[j].mBias;

                    if (rand.Next(2) == 0)
                    {
                        mNetworkLayers[k].mWeights[i, j] += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                        if (doOnce)
                            mNetworkLayers[k].mNeurons[j].mBias += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                    }
                }
                doOnce = false;
            }
        }
    }
    //Returns the neuron with the highest activation
    int GetFinalOutput(NetworkLayer outputLayer)
    {
        double highestActivation = -Mathf.Infinity;
        int i, ans = -1;
        for (i = 0; i < outputLayer.mNeurons.Length -1; i++)
        {
            outputLayer.mNeurons[i].mActivation = Sigmoid(outputLayer.mNeurons[i].mActivation);
            if (highestActivation < outputLayer.mNeurons[i].mActivation)
            {
                highestActivation = outputLayer.mNeurons[i].mActivation;
                ans = i;
            }
        }
        return ans;
    }

    double Sigmoid(double x) => (1.0 / (1.0 + Math.Exp(-x)));
    double ReLU(double x) => Math.Max(0.0f, x);
}
