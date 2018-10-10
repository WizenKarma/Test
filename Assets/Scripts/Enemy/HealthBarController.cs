﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour {


    #region PRIVATE_VARIABLES
    private Vector2 positionCorrection = new Vector2(0, 100);
    private GameManager gameManager; // reference to GM to tell when it has died
    #endregion
    #region PUBLIC_REFERENCES
    public RectTransform targetCanvas;
    public RectTransform healthBar;
    public Transform objectToFollow;
    public Enemy enemy;
    #endregion
    #region PUBLIC_METHODS


    public void SetHealthBarData(Transform targetTransform, RectTransform healthBarPanel)
    {
        gameManager = FindObjectOfType<GameManager>(); // hey gm i found you
        this.targetCanvas = healthBarPanel;
        healthBar = GetComponent<RectTransform>();
        objectToFollow = targetTransform;
        RepositionHealthBar();
        healthBar.gameObject.SetActive(true);
        enemy = objectToFollow.GetComponent<Enemy>();
        enemy.OnHealthChange += OnHealthChanged; //adds the event that will update the healthbar.
    }

    public void Hide()
    {
        healthBar.gameObject.SetActive(false);
    }

    public void Show()
    {
        healthBar.gameObject.SetActive(true);
    }

    public void die()
    {
        Destroy(this.gameObject);
    }
     void OnHealthChanged()
    {
        float healthFill = enemy.Health.Value/enemy.MaxHealth.Value;
        if (healthFill <= 0)
        {
            gameManager.numberOfEnemiesActive--; // hey gm i died
            Destroy(enemy.gameObject);
            Destroy(this.gameObject);
        }
        healthBar.GetComponent<Image>().fillAmount = healthFill;
    }

    #endregion
    #region UNITY_CALLBACKS
    void Update()
    {
        RepositionHealthBar();
    }

    #endregion
    #region PRIVATE_METHODS
    private void RepositionHealthBar()
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(objectToFollow.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * targetCanvas.sizeDelta.x) - (targetCanvas.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));
        //now you can set the position of the ui element
        healthBar.anchoredPosition = WorldObject_ScreenPosition;
        //this is all for in case we want to scale the health bars which doesn't currently work
       // Vector3 temp = new Vector3(WorldObject_ScreenPosition.x, WorldObject_ScreenPosition.y, 0f);
       // float scaleDiff = (temp - objectToFollow.transform.position).sqrMagnitude;
        //print(scaleDiff);
        //healthBar.localScale = new Vector3(scaleDiff/5000f,scaleDiff/5000f,1f);
    }
    #endregion
}
