using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameType
{
    Moves
}

[System.Serializable]
public class EndGameRequirments
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject tryAgainPanel;
    public GameObject youWinPanel;
    public TMP_Text counter;
    public int currentCounterValue;
    public EndGameRequirments requirments;

    private Board board;
    FadePanelController fadePanel;

    void Start()
    {
        fadePanel = FindObjectOfType<FadePanelController>();
        board = FindObjectOfType<Board>();

        SetGameTypeFromLevelObj();
        SetUpGame();
    }

    void SetGameTypeFromLevelObj()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirments = board.world.levels[board.level].requirments;
                }
            }
        }
    }

    void SetUpGame()
    {
        currentCounterValue = requirments.counterValue;

        if (requirments.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
        }
        else
        {
            movesLabel.SetActive(false);
        }

        counter.text = "" + currentCounterValue;
    }

    public void DecreseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;

            if (currentCounterValue <= 0)
            {
                LoseGame();
            }

            counter.text = "" + currentCounterValue;
        }
    }

    public void WinGame()
    {
        if(board.currentState != GameState.wait)
        {
            
        }

        youWinPanel.SetActive(true);
        board.currentState = GameState.win;

        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        fadePanel.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;        

        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        fadePanel.GameOver();
    }    
}
