using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Header("Levels")]
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;

    [Header("Player")]
    public int playerHealth;
    public int playerCoins;

    [Header("Timers")]
    public TimeSpan timeOld;
    public float freeSpinTimer;
    public bool isFreeSpinTimerActive = false;

    public float adsSpinTimer;
    public bool isAdsSpinTimerActive = false;

    public float healthTimer;
    public bool isHealthTimerActive = false;
    public float secondsInFiveMinets = 300;

    [Header("Busters")]

    public int[] busterValue;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();
    }

    void Start()
    {

    }

    private void Update()
    {
        if (saveData.isHealthTimerActive)
        {
            if (saveData.healthTimer > 0)
            {
                saveData.healthTimer -= Time.deltaTime;
            }
            else if (saveData.healthTimer <= 0)
            {
                saveData.playerHealth++;
                gameData.Save();
                ResetHealthTimer();
            }
        }

        if (saveData.playerHealth < 5)
        {
            StartPlayerHealthTimer();
        }
    }

    public void StartPlayerHealthTimer()
    {
        saveData.isHealthTimerActive = true;
    }

    public void ResetHealthTimer()
    {
        saveData.isHealthTimerActive = false;
        saveData.healthTimer = saveData.secondsInFiveMinets;
    }

    public void Save()
    {
        saveData.timeOld = DateTime.Now.TimeOfDay;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
    }
}
