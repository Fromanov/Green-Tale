using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMap : MonoBehaviour
{
    public string sceneToLoad;
    private GameData gameData;
    private Board board;    

    public void OKButton()
    {
        if(gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;            

            gameData.Save();
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    public void RetryButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        gameData.saveData.playerHealth--;
        gameData.Save();
        SceneManager.LoadScene(scene.name);
    }

    public void AdsRetryButton()
    {
        Scene scene = SceneManager.GetActiveScene();        
        SceneManager.LoadScene(scene.name);
    }

    public void LoseOKButton()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();        
    }
}
