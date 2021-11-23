using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeCounter : MonoBehaviour
{
    public static RealTimeCounter realTimeCounter;

    private float twelveHoursTimer = 43200;
    private float secondsInFiveMinutes = 300;
    private GameData gameData;

    public void Awake()
    {
        if (realTimeCounter == null)
        {
            DontDestroyOnLoad(this.gameObject);
            realTimeCounter = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        gameData = FindObjectOfType<GameData>();

        Load();
    }

    public void Load()
    {
        TimeSpan timeNow = DateTime.Now.TimeOfDay;
        TimeSpan timeLeft = timeNow - gameData.saveData.timeOld;

        Debug.Log("Time old " + gameData.saveData.timeOld);
        Debug.Log("Time past " + timeLeft);


        if (gameData.saveData.isFreeSpinTimerActive)
        {
            gameData.saveData.isFreeSpinTimerActive = true;

            if ((float)(timeLeft.TotalSeconds) >= gameData.saveData.freeSpinTimer)
            {
                ResetFreeTimer();
            }
            else
            {
                gameData.saveData.freeSpinTimer -= (float)(timeLeft.TotalSeconds);
            }

        }
        else
        {
            ResetFreeTimer();
        }

        if (gameData.saveData.isAdsSpinTimerActive)
        {
            gameData.saveData.isAdsSpinTimerActive = true;

            if ((float)(timeLeft.TotalSeconds) >= gameData.saveData.adsSpinTimer)
            {
                ResetAdTimer();
            }
            else
            {
                gameData.saveData.adsSpinTimer -= (float)(timeLeft.TotalSeconds);
            }
        }
        else
        {
            ResetAdTimer();
        }

        if (gameData.saveData.isHealthTimerActive)
        {
            if (gameData.saveData.playerHealth < 5)
            {
                if ((float)(timeLeft.TotalSeconds) >= gameData.saveData.healthTimer)
                {
                    gameData.saveData.playerHealth += 1 + (int)((float)(timeLeft.TotalSeconds) - gameData.saveData.healthTimer) / (int)secondsInFiveMinutes;
                    if (gameData.saveData.playerHealth >= 5)
                    {
                        gameData.saveData.playerHealth = 5;
                        gameData.saveData.isHealthTimerActive = false;
                    }

                }
            }
            else
            {
                gameData.saveData.healthTimer -= (float)(timeLeft.TotalSeconds);
            }
        }
    }

    private void Update()
    {
        if (gameData.saveData.isFreeSpinTimerActive)
        {
            if (gameData.saveData.freeSpinTimer > 0)
            {
                gameData.saveData.freeSpinTimer -= Time.deltaTime;
            }
            else if (gameData.saveData.freeSpinTimer <= 0)
            {
                ResetFreeTimer();
            }
        }

        if (gameData.saveData.isAdsSpinTimerActive)
        {
            if (gameData.saveData.adsSpinTimer > 0)
            {
                gameData.saveData.adsSpinTimer -= Time.deltaTime;
            }
            else if (gameData.saveData.adsSpinTimer <= 0)
            {
                ResetAdTimer();
            }
        }
    }

    public void SaveDate()
    {
        gameData.saveData.timeOld = DateTime.Now.TimeOfDay;
    }

    public void ActivateFreeSpinTimer()
    {
        gameData.saveData.isFreeSpinTimerActive = true;
    }
    public void ActivateAdSpinTimer()
    {
        gameData.saveData.isAdsSpinTimerActive = true;
    }

    public void ResetFreeTimer()
    {
        gameData.saveData.isFreeSpinTimerActive = false;
        gameData.saveData.freeSpinTimer = twelveHoursTimer;
    }

    public void ResetAdTimer()
    {
        gameData.saveData.isAdsSpinTimerActive = false;
        gameData.saveData.adsSpinTimer = twelveHoursTimer;
    }

    private void OnApplicationQuit()
    {
        SaveDate();
    }
}
