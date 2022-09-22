using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttributesScript : MonoBehaviour
{
    [Header("Infected Attributes")]
    public GameObject mCurrentTarget = null;
    public bool mInfected = false;
    public bool mEnemyClose = false;
    public CurrentState mCurrentState = CurrentState.NOT_INFECTED;

    [Header("Movement Attributes")]
    [SerializeField]
    public float mMovementSpeed = 4.5f;

    [Header("Visual Attributes")]
    [SerializeField]
    public Material mNonInfectedMaterial;
    [SerializeField]
    public Material mInfectedMaterial;

    public float mBehaviourUpdateFrequency = 0.15f;
}
