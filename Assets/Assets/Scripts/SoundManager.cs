using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource[] movePieceNoise;
    public AudioSource backgroundMusic;

    private void Start()
    {
        if(PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 1)
            {
                backgroundMusic.volume = 1;                
                backgroundMusic.Play();

                for(int i = 0; i < movePieceNoise.Length;  i++)
                {
                    movePieceNoise[i].volume = 1;
                }

                for (int i = 0; i < destroyNoise.Length; i++)
                {
                    destroyNoise[i].volume = 1;
                }
            }
            else
            {
                backgroundMusic.volume = 0;
                backgroundMusic.Play();

                for (int i = 0; i < movePieceNoise.Length; i++)
                {
                    movePieceNoise[i].volume = 0;
                }

                for (int i = 0; i < destroyNoise.Length; i++)
                {
                    destroyNoise[i].volume = 0;
                }
            }
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 1);
            backgroundMusic.Play();
        }
    }

    public void AdjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMusic.volume = 0;

                for (int i = 0; i < movePieceNoise.Length; i++)
                {
                    movePieceNoise[i].volume = 0;
                }

                for (int i = 0; i < destroyNoise.Length; i++)
                {
                    destroyNoise[i].volume = 0;
                }
            }
            else
            {
                backgroundMusic.volume = 1;

                for (int i = 0; i < movePieceNoise.Length; i++)
                {
                    movePieceNoise[i].volume = 1;
                }

                for (int i = 0; i < destroyNoise.Length; i++)
                {
                    destroyNoise[i].volume = 1;
                }
            }
        }
    }

    public void PlayRandomDestroyNoise()
    {
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToPlay].Play();
    }

    public void PlayRandomMovePieceNoise()
    {
        int clipToPlay = Random.Range(0, movePieceNoise.Length);
        movePieceNoise[clipToPlay].Play();        
    }
}
