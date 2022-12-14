using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public abstract class AgentUIManager : MonoBehaviour
{
    [SerializeField]
    protected float mPositionOffset;
    protected Canvas mCanvas;
    protected Camera mCamera;

    [SerializeField]
    protected GameObject mHealthBarPrefab;
    protected GameObject mHealthBar;
    protected Slider mHealthBarSlider;
    [SerializeField]
    protected GameObject mAttributeMenuPrefab;
    protected GameObject mAttributeMenu;
    [SerializeField]
    protected int mNumberOfAttributesToDisplay;

    protected void Awake()
    {
        mCanvas = FindObjectOfType<UIManager>().GetComponent<Canvas>();
        mCamera = Camera.main;
    }
    protected void UpdateHealthBar(float currentEnergy, float maxEnergy)
    {
        mHealthBar.transform.position = mCamera.WorldToScreenPoint(transform.gameObject.transform.position + Vector3.up * mPositionOffset);
        float percentageEnergyRemaining = currentEnergy / maxEnergy;
        mHealthBarSlider.value = percentageEnergyRemaining;
    }

    protected void OnEnable()
    {
        mHealthBar = Instantiate(mHealthBarPrefab, mCanvas.transform);
        mAttributeMenu = Instantiate(mAttributeMenuPrefab, mCanvas.transform);
        mHealthBarSlider = mHealthBar.GetComponent<Slider>();
    }
    protected void OnDisable()
    {
        if (mHealthBar != null && mAttributeMenu != null)
        {
            Destroy(mHealthBar);
            Destroy(mAttributeMenu);
        }
    }

    private void Update()
    {
        UpdateAttributeText();
    }

    protected abstract void SetAttributeText();
    protected abstract void UpdateAttributeText();
}
