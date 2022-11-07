using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro.EditorUtilities;
using System.Linq;
using UnityEngine.UIElements;

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

    public int RunNetwork(List<double> input)
    {
        Debug.Assert(mNetworkLayers.Length > 0 && mNetworkLayers[0].mNeurons.Length == input.Count);
        for (int i =0; i < input.Count; i++)
        {
            mNetworkLayers[0].mNeurons[i].mActivation = input[i];
        }

        for (int i = 0; i < mNetworkLayers.Length -1; i++)
        {
            ForwardPropigateActivation(mNetworkLayers[i]);
        }

        return GetFinalOutput(mNetworkLayers[mNetworkLayers.Length - 1]);
    }

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
                currentLayer.mWeights[i, j] = 0.00001 * rand.Next(-15000, 15000);
            }
        }
    }

    void ForwardPropigateActivation(NetworkLayer currentLayer)
    {
        for (int i = 0; i < currentLayer.mWeights.GetLength(0); i++)
        {
            currentLayer.mNextLayer.mNeurons[i].mActivation = 0.0;
            for (int j = 0; j < currentLayer.mWeights.GetLength(1); j++)
            {
                currentLayer.mNextLayer.mNeurons[i].mActivation += currentLayer.mWeights[i, j] * currentLayer.mNeurons[j].mActivation;
            }
            currentLayer.mNextLayer.mNeurons[i].mActivation = Sigmoid(currentLayer.mNextLayer.mNeurons[i].mActivation + currentLayer.mNextLayer.mNeurons[i].mBias);
        }
    }

    public void UpdateWeightsAndBias(NetworkLayer currentLayer, float mLerningRate)
    {
        System.Random rand = new System.Random();
        
        if (currentLayer.mNextLayer == null)
        {
            foreach(Neuron n in currentLayer.mNeurons)
            {
                n.mBias += 0.00001 * rand.Next(-10000, 10000) * mLerningRate;
            }
            return;
        }
        
        bool doOnce = true;
        for (int i = 0; i < currentLayer.mWeights.GetLength(0); i++)
        {
            
            for (int j = 0; j < currentLayer.mWeights.GetLength(1); j++)
            {
                currentLayer.mWeights[i, j] += 0.00001 * rand.Next(-10000, 10000) * mLerningRate;
                if (doOnce)
                    currentLayer.mNeurons[j].mBias += 0.00001 * rand.Next(-10000, 10000) * mLerningRate;
            }
            doOnce = false;
        }
    }

    int GetFinalOutput(NetworkLayer outputLayer)
    {
        double highestActivation = -Mathf.Infinity;
        int i, ans = -1;
        for (i = 0; i < outputLayer.mNeurons.Length; i++)
        {
            if (highestActivation < outputLayer.mNeurons[i].mActivation)
            {
                highestActivation = outputLayer.mNeurons[i].mActivation;
                ans = i;
            }
        }
        return ans;
    }

    double Sigmoid(double x)
    {
        return (1.0 / (1.0 + Math.Exp(-x)));
    }

}
