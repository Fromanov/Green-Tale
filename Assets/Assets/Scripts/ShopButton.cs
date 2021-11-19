using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public enum BusterType
    {
        Shovel,        
        Glowes,
        WatringCan,
        RainbowBomb
    }

    public BusterType busterType;
    public int shopItemValue;
    public int shopItemCost;

}
public class ShopButton : ShopItem
{
    private GameData gameData;

    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();
    }    

    public void BuyButtonClick() 
    {
        if(gameData.saveData.playerCoins >= shopItemCost)
        {
            gameData.saveData.playerCoins -= shopItemCost;
            gameData.saveData.busterValue[(int)busterType] += shopItemValue;
            gameData.Save();
        }
    }    
}
