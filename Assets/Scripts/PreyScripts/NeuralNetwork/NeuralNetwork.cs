using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public NeuralNetwork(int[] layerSizes)
    {
        mNetworkLayers = new NetworkLayer[layerSizes.Length];

        for(int i = 0; i < layerSizes.Length; i++)
        {
            mNetworkLayers[i] = new NetworkLayer(layerSizes[i]);
        }
    }
    NetworkLayer[] mNetworkLayers;
}
