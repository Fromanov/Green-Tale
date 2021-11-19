using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public bool paused = false;
    public Animator pauseAnimator;
    public Image soundButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public TMP_Text playerHealth;

    private Board board;
    private GameData gameData;
    private SoundManager sound;

    void Start()
    {
        if(PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOffSprite;
            }
            else 
            {
                soundButton.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButton.sprite = musicOnSprite;
        }

        pausePanel.SetActive(false);        
        pauseAnimator.SetBool("isPause", false);

        sound = FindObjectOfType<SoundManager>();
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();

        if(gameData)
        {
            playerHealth.text = gameData.saveData.playerHealth.ToString();
        }        
    }
    
    void Update()
    {
        if(paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
            pauseAnimator.SetBool("isPause", true);            
        }

        if (!paused && pausePanel.activeInHierarchy)
        {
            pauseAnimator.SetBool("isPause", false);            
            board.currentState = GameState.move;
            StartCoroutine(wait());
            //pausePanel.SetActive(false);
        }
    }

    public void SoundButtonClick()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                sound.AdjustVolume();
            }
            else
            {
                soundButton.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                sound.AdjustVolume();
            }
        }       
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    public void ExitLevel()
    {        
        if(gameData != null)
        {
            gameData.saveData.playerHealth--;
        }
        gameData.Save();

        SceneManager.LoadScene("Map");        
    }

    IEnumerator wait()
    {        
        yield return new WaitForSeconds(1.5f);
        pausePanel.SetActive(false);
    }
}
