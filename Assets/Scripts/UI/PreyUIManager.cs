using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PreyUIManager : MonoBehaviour
{
    public float screenOffset = 2.0f;
    private Camera cam;
    private RawImage image;
    PreyController preyController;
    public GameObject healthBarPrefab;
    GameObject healthBar;
    Canvas canvas;
    float startingXDeltaSize;

    private void OnEnable()
    {
        canvas = FindObjectOfType<UIManager>().GetComponent<Canvas>();
        cam = Camera.main;
        preyController = GetComponentInParent<PreyController>();
        healthBar = Instantiate(healthBarPrefab, canvas.transform);
        image = healthBar.transform.GetChild(1).GetComponent<RawImage>();
        startingXDeltaSize = image.rectTransform.sizeDelta.x;
    }
    private void OnDisable()
    {
        Destroy(healthBar);
    }

    private void FixedUpdate()
    {
        healthBar.transform.position = cam.WorldToScreenPoint(transform.gameObject.transform.position + Vector3.up * screenOffset);
        float percentageEnergyRemaining = preyController.mAttributes.mEnergyLevel / preyController.mAttributes.mStartingEnergy;
        image.rectTransform.sizeDelta = new Vector2(startingXDeltaSize * percentageEnergyRemaining, image.rectTransform.sizeDelta.y);
        image.rectTransform.anchoredPosition = new Vector2((startingXDeltaSize * percentageEnergyRemaining - startingXDeltaSize) / 2.0f, 0.0f);

    }
    //private void OnMouseEnter()
    //{
    //    healthBar.SetActive(true);
    //}
    //private void OnMouseExit()
    //{
    //    healthBar.SetActive(false);
    //}
}
