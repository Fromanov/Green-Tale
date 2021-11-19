using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapInfoPanel : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text healthTimerText;
    public TMP_Text coinText;

    private GameData gameData;

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameData != null)
        {
            healthText.text = gameData.saveData.playerHealth.ToString();            
            coinText.text = gameData.saveData.playerCoins.ToString();
            
            if(gameData.saveData.isHealthTimerActive)
            {
                if (gameData.saveData.healthTimer > 0)
                {
                    TimeSpan tempTimer = TimeSpan.FromSeconds(Mathf.Round(gameData.saveData.healthTimer));
                    healthTimerText.text = String.Format("{0}:{1:D2}", tempTimer.Minutes, tempTimer.Seconds);
                }
                else if (gameData.saveData.healthTimer <= 0)
                {
                    healthTimerText.text = "";
                }
            }
            else
            {
                healthTimerText.text = "";
            }
            
        }
        
    }
}
