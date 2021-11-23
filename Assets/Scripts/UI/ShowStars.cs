using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStars : MonoBehaviour
{
    public Sprite activeStar;
    public Sprite lockedStar;
    public Image[] stars;

    private ScoreManager scoreManager;
    private Board board;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();

        CleanStarPanel();        
    }

    public void Update()
    {
        ActivateStars();
    }

    void CleanStarPanel()
    {
        stars[0].sprite = lockedStar;
        stars[0].sprite = lockedStar;
        stars[0].sprite = lockedStar;
    }

    void ActivateStars()
    {
        if(scoreManager.score >= board.scoreGoals[0] && scoreManager.score < board.scoreGoals[1])
        {
            stars[0].sprite = activeStar;
        }
        else if(scoreManager.score >= board.scoreGoals[1] && scoreManager.score < board.scoreGoals[2])
        {
            stars[0].sprite = activeStar;
            stars[1].sprite = activeStar;
        }
        else if (scoreManager.score >= board.scoreGoals[2])
        {
            stars[0].sprite = activeStar;
            stars[1].sprite = activeStar;
            stars[2].sprite = activeStar;
        }
    }
}
