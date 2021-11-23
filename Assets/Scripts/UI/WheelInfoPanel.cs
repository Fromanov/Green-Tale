using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WheelInfoPanel : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text coinText;
    public TMP_Text showelText;
    public TMP_Text wateringCanText;
    public TMP_Text glovesText;
    public TMP_Text colorBombText;

    private GameData gameData;

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
    }
    
    void Update()
    {
        if(gameData)
        {
            healthText.text = gameData.saveData.playerHealth.ToString();
            coinText.text = gameData.saveData.playerCoins.ToString();
            showelText.text = gameData.saveData.busterValue[0].ToString();
            wateringCanText.text = gameData.saveData.busterValue[2].ToString();
            glovesText.text = gameData.saveData.busterValue[1].ToString();
            colorBombText.text = gameData.saveData.busterValue[3].ToString();
        }
    }
}
