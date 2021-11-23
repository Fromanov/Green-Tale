using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerTextManager : MonoBehaviour
{

    public TMP_Text freeSpinTimerText;
    public TMP_Text adSpinTimerText;

    private GameData gameData;

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
    }

    void Update()
    {
        freeSpinTimerText.text = TimeSpan.FromSeconds(Mathf.Round(gameData.saveData.freeSpinTimer)).ToString();
        adSpinTimerText.text = TimeSpan.FromSeconds(Mathf.Round(gameData.saveData.adsSpinTimer)).ToString();

        if (gameData.saveData.isFreeSpinTimerActive == false)
        {
            freeSpinTimerText.text = "";
        }

        if (gameData.saveData.isAdsSpinTimerActive == false)
        {
            adSpinTimerText.text = "";
        }
    }
}
