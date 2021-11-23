using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Active")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lokedSprite;

    private int starsActive;
    private Image buttonImage;
    private Button myButton;

    [Header("Level UI")]
    public Sprite activeStar;    
    public Image[] stars;

    [Header("Else")]
    public TMP_Text levelText;
    public int level;
    public GameObject confirmPanel;


    
    private GameData gameData;

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();

        LoadData();
        ShowStars();
        SetLevel();
        DecideSprite();
    }

    void LoadData()
    {
        if(gameData != null)
        {
            if(gameData.saveData.isActive[level] && level < gameData.saveData.isActive.Length)
            {
                isActive = true;                
            }
            else
            {
                isActive = false;
            }

            if(gameData.saveData.stars[level] <= 3  && gameData.saveData.stars[level] >= 0)
            {
                starsActive = gameData.saveData.stars[level];
            }
            else
            {
                if (gameData.saveData.stars[level] > 3)
                {
                    gameData.saveData.stars[level] = 3;
                }
                else if (gameData.saveData.stars[level] < 0)
                {
                    gameData.saveData.stars[level] = 0;
                }

                starsActive = gameData.saveData.stars[level];
            }            
        }
    }

    void ShowStars()
    {
        if(isActive)
        {
            for (int i = 0; i <= starsActive - 1; i++)
            {
                stars[i].sprite = activeStar;
            }
        }                    
    }

    void DecideSprite()
    {
        if(isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lokedSprite;
            myButton.enabled = false;
            levelText.enabled = false;
        }
    }

    void SetLevel()
    {
        levelText.text = "" + (level + 1);
    }

    public void ConfirmPanel()
    {
        if(gameData.saveData.playerHealth > 0)
        {
            confirmPanel.GetComponent<ConfirmPanel>().level = level;
            confirmPanel.SetActive(true);
        }        
    }
}
