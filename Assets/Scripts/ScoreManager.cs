using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score;
    public Slider scoreSlider;    

    private Board board;
    private GameData gameData;
    private int numberStars;

    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    void Update()
    {
        scoreText.text = score.ToString();        
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;

        for(int i = 0; i < board.scoreGoals.Length; i++)
        {
            if(score > board.scoreGoals[i] && numberStars < i +1)
            {
                numberStars++;
            }
        }

        if(gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];

            if(highScore < score)
            {
                gameData.saveData.highScores[board.level] = score;                            
            }

            int currentStars = gameData.saveData.stars[board.level];

            if(numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }

            gameData.Save();
        }

        UpdateBar();
    }    

    private void UpdateBar()
    {
        if (board != null && scoreSlider != null)
        {
            int scoreGoal = board.scoreGoals[2];
            float curValue = ((float)score / (float)scoreGoal) * 100;
            scoreSlider.value = curValue;
        }
    }
}
