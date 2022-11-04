using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    [SerializeField]
    public int mEnergyAmount;
    [SerializeField]
    public float mLifespan = 20.0f;

    IEnumerator BeingCountdown()
    {
        yield return new WaitForSeconds(mLifespan);
        FoodSpawnerScript.ReturnFood(gameObject);
        yield return null;
    }

    private void OnEnable()
    {
        //StartCoroutine(BeingCountdown());
    }
}
