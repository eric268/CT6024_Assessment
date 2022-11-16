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
                currentLayer.mWeights[i, j] = 0.00001 * rand.Next(-30000, 30000);
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
            currentLayer.mNextLayer.mNeurons[i].mActivation = ReLU(currentLayer.mNextLayer.mNeurons[i].mActivation + currentLayer.mNextLayer.mNeurons[i].mBias);// Sigmoid(currentLayer.mNextLayer.mNeurons[i].mActivation + currentLayer.mNextLayer.mNeurons[i].mBias);
        }
    }

    public void UpdateWeightsAndBias(NetworkLayer currentLayer, float mLerningRate)
    {
        System.Random rand = new System.Random();
        
        if (currentLayer.mNextLayer == null)
        {
            foreach(Neuron n in currentLayer.mNeurons)
            {
                if (rand.Next(2) == 0)
                {
                    n.mBias += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                }
            }
            return;
        }
        
        bool doOnce = true;
        for (int i = 0; i < currentLayer.mWeights.GetLength(0); i++)
        {   
            for (int j = 0; j < currentLayer.mWeights.GetLength(1); j++)
            {
                if (rand.Next(2) == 0)
                {
                    currentLayer.mWeights[i, j] += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                    if (doOnce)
                        currentLayer.mNeurons[j].mBias += 0.00001 * rand.Next(-30000, 30000) * mLerningRate;
                }
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

    public void CopyAndMutateNetwork(NetworkLayer[] layersToCopy, float mLerningRate)
    {
        System.Random rand = new System.Random();
        for (int k = 0; k < mNetworkLayers.Length; k++)
        {
            if (mNetworkLayers[k].mNextLayer == null)
            {
                for(int j =0; j < mNetworkLayers[k].mNumberOfNeurons; j++)
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

    double Sigmoid(double x) => (1.0 / (1.0 + Math.Exp(-x)));
    double ReLU(double x) => Math.Max(0.0f, x);
}
