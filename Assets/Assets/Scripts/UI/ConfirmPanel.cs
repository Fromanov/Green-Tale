using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level info")]
    public string levelToLoad;
    public Image[] stars;
    public int level;

    private int highScore;
    private int starsActive;

    [Header("UI stuff")]
    public Sprite activeStar;
    public Sprite lockedStar;
    public TMP_Text levelText;
    public TMP_Text highScoreText;

    private GameData gameData;


    private void Awake()
    {
        gameData = FindObjectOfType<GameData>();        

        SetText();
        LoadData();
        ShowStars();
    }

    void LoadData()
    {
        if (gameData != null)
        {
            if (gameData.saveData.stars[level] <= 3 && gameData.saveData.stars[level] >= 0)
            {
                starsActive = gameData.saveData.stars[level];
            }

            highScore = gameData.saveData.highScores[level];
        }
    }

    void ShowStars()
    {
        starsActive = gameData.saveData.stars[level];

        if (gameData.saveData.stars[level] == starsActive)
        {
            for (int i = 0; i <= starsActive - 1; i++)
            {
                stars[i].sprite = activeStar;
            }
        }
    }

    void SetText()
    {
        levelText.text = ("Level " + (level + 1).ToString());        

        if(gameData.saveData.highScores[level] >= 0 )
        {
            highScoreText.text = (gameData.saveData.highScores[level].ToString());
        }
    }

    void ClearStar()
    {
        for (int i = 0; i <= stars.Length - 1; i++)
        {
            if(stars[i].sprite == activeStar)
            {
                stars[i].sprite = lockedStar;                
            }            
        }
    }

    public void Cancel()
    {
        
        this.gameObject.SetActive(false);
        ClearStar();
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level", level);
        SceneManager.LoadScene(levelToLoad);
    }

    void OnEnable()
    {
        SetText();
        ShowStars();        
    }
    
}
