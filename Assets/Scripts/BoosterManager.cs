using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoosterManager : MonoBehaviour
{
    public bool isBoosterPressed;
    public BoosterType boosterType;
    public TMP_Text boosterButtonColorBomb;
    public TMP_Text boosterButtonGloves;
    public TMP_Text boosterButtonWateringCan;
    public TMP_Text boosterButtonShowel;

    public Animator busterAnimatorColorBomb;
    public Animator busterAnimatorGloves;
    public Animator busterAnimatorWateringCan;
    public Animator busterAnimatorShowel;

    private Board board;
    private GameData gameData;


    private void Start()
    {
        isBoosterPressed = false;
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();

        UpdateBusterValue();
    }

    public void BoosterClick()
    {
        isBoosterPressed = true;
        board.currentState = GameState.booster;

        UpdateBusterValue();
    }

    public void Update()
    {
        if (isBoosterPressed == false)
        {
            busterAnimatorColorBomb.SetBool("isActive", false);
            busterAnimatorGloves.SetBool("isActive", false);
            busterAnimatorWateringCan.SetBool("isActive", false);
            busterAnimatorShowel.SetBool("isActive", false);
        }
    }

    public void DeleteSingleButtonPress()
    {
        busterAnimatorShowel.SetBool("isActive", true);

        if (gameData.saveData.busterValue[0] > 0)
        {
            if (board.currentState == GameState.move)
            {
                boosterType = BoosterType.DeleteSingle;
                gameData.saveData.busterValue[0]--;
                gameData.Save();
                BoosterClick();
            }
        }
    }

    public void DeleteRowButtonPress()
    {
        busterAnimatorWateringCan.SetBool("isActive", true);

        if (gameData.saveData.busterValue[2] > 0)
        {
            if (board.currentState == GameState.move)
            {
                boosterType = BoosterType.DeleteRow;
                gameData.saveData.busterValue[2]--;
                gameData.Save();
                BoosterClick();
            }
        }
    }

    public void DeleteColumnButtonPress()
    {
        busterAnimatorGloves.SetBool("isActive", true);

        if (gameData.saveData.busterValue[1] > 0)
        {
            if (board.currentState == GameState.move)
            {
                boosterType = BoosterType.DeleteColumn;
                gameData.saveData.busterValue[1]--;
                gameData.Save();
                BoosterClick();
            }
        }
    }

    public void TransformColorBombButtonPress()
    {
        busterAnimatorColorBomb.SetBool("isActive", true);

        if (gameData.saveData.busterValue[3] > 0)
        {
            if (board.currentState == GameState.move)
            {
                boosterType = BoosterType.TransformColorBomb;
                gameData.saveData.busterValue[3]--;
                gameData.Save();
                BoosterClick();
            }
        }
    }

    public void UpdateBusterValue()
    {
        boosterButtonColorBomb.text = gameData.saveData.busterValue[3].ToString();
        boosterButtonGloves.text = gameData.saveData.busterValue[1].ToString(); ;
        boosterButtonWateringCan.text = gameData.saveData.busterValue[2].ToString(); ;
        boosterButtonShowel.text = gameData.saveData.busterValue[0].ToString(); ;
    }
}
