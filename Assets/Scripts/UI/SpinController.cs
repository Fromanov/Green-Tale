using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SpinController : MonoBehaviour
{
    public Button freeSpinButton;
    public Button adSpinButton;

    private int randomValue;
    private float timeInterval;
    private bool coroutineAllowed;
    private int finalAngle;
    private GameData gameData;
    private RealTimeCounter wheelTimer;

    [SerializeField]
    private TMP_Text winText;

    private void Start()
    {
        coroutineAllowed = true;
        gameData = FindObjectOfType<GameData>();
        wheelTimer = FindObjectOfType<RealTimeCounter>();
    }

    private void Update()
    {
        if (gameData.saveData.isFreeSpinTimerActive != true)
        {
            freeSpinButton.interactable = true;
        }
        else
        {
            freeSpinButton.interactable = false;
        }

        if (gameData.saveData.isAdsSpinTimerActive != true)
        {
            adSpinButton.interactable = true;
        }
        else
        {
            adSpinButton.interactable = false;
        }
    }

    public void FreeSpinClick()
    {
        freeSpinButton.interactable = false;
        string type = "Free";

        if (coroutineAllowed)
        {
            if (gameData.saveData.isFreeSpinTimerActive != true)
            {
                winText.text = "";
                gameData.saveData.isFreeSpinTimerActive = true;
                StartCoroutine(Spin(type));
            }
        }
    }

    public void AdsSpinClick()
    {
        adSpinButton.interactable = false;
        string type = "Ads";

        if (coroutineAllowed)
        {
            if (gameData.saveData.isAdsSpinTimerActive != true)
            {
                winText.text = "";
                gameData.saveData.isAdsSpinTimerActive = true;
                StartCoroutine(Spin(type));
            }
        }
    }

    private IEnumerator Spin(string type)
    {
        coroutineAllowed = false;
        randomValue = Random.Range(20, 30);
        timeInterval = 0.1f;


        for (int i = 0; i < randomValue; i++)
        {
            this.transform.Rotate(0, 0, 22.5f);

            if (i > Mathf.RoundToInt(randomValue * 0.5f))
            {
                timeInterval = 0.2f;
            }

            if (i > Mathf.RoundToInt(randomValue * 0.85f))
            {
                timeInterval = 0.4f;
            }

            yield return new WaitForSeconds(timeInterval);
        }

        if (Mathf.RoundToInt(this.transform.eulerAngles.z) % 45 != 0)
        {
            this.transform.Rotate(0, 0, 22.5f);
        }

        finalAngle = Mathf.RoundToInt(this.transform.eulerAngles.z);

        //  25, 45, 66, 111, 133, 156, 201,223, 246, 291, 313, 336

        Debug.Log(finalAngle);

        int prizeValue = Random.Range(1, 2);
        int coinPrizeValue = Random.Range(10, 20);

        switch (finalAngle)
        {
            case 25:
                winText.text = "You Win " + prizeValue + " Gloves";
                GivePrize(prizeValue, "Gloves");
                break;
            case 45:
                winText.text = "You Win " + prizeValue + " Watering can";
                GivePrize(prizeValue, "Watering can");
                break;
            case 65:
                winText.text = "You Win " + prizeValue + " Rainbow bomb";
                GivePrize(prizeValue, "Rainbow bomb");
                break;
            case 115:
                winText.text = "You Win " + prizeValue + " Heart";
                GivePrize(prizeValue, "Heart");
                break;
            case 135:
                winText.text = "You Win " + coinPrizeValue + " Coins";
                GivePrize(coinPrizeValue, "Coins");
                break;
            case 155:
                winText.text = "You Win " + prizeValue + " Showel";
                GivePrize(prizeValue, "Showel");
                break;
            case 205:
                winText.text = "You Win " + prizeValue + " Gloves";
                GivePrize(prizeValue, "Gloves");
                break;
            case 225:
                winText.text = "You Win " + prizeValue + " Watering can";
                GivePrize(prizeValue, "Watering can");
                break;
            case 245:
                winText.text = "You Win " + prizeValue + " Rainbow bomb";
                GivePrize(prizeValue, "Rainbow bomb");
                break;
            case 295:
                winText.text = "You Win " + prizeValue + " Heart";
                GivePrize(prizeValue, "Heart");
                break;
            case 315:
                winText.text = "You Win " + coinPrizeValue + " Coins";
                GivePrize(coinPrizeValue, "Coins");
                break;
            case 335:
                winText.text = "You Win " + prizeValue + " Showel";
                GivePrize(prizeValue, "Showel");
                break;
            default:
                winText.text = "You lose. Don't worry, try again for free";
                ResetTimer(type);
                break;
        }

        coroutineAllowed = true;

        gameData.Save();

    }

    public void ResetTimer(string buttonType)
    {
        if (buttonType == "Free")
        {
            wheelTimer.ResetFreeTimer();
        }
        else if (buttonType == "Ads")
        {
            wheelTimer.ResetAdTimer();
        }
    }

    private void GivePrize(int value, string type)
    {
        switch (type)
        {
            case "Showel":
                gameData.saveData.busterValue[0] += value;
                break;
            case "Heart":
                gameData.saveData.playerHealth += value;
                break;
            case "Gloves":
                gameData.saveData.busterValue[1] += value;
                break;
            case "Watering can":
                gameData.saveData.busterValue[2] += value;
                break;
            case "Rainbow bomb":
                gameData.saveData.busterValue[3] += value;
                break;
            case "Coins":
                gameData.saveData.playerCoins += value;
                break;
        }
    }

    public void ExitClick()
    {
        SceneManager.LoadScene("Map");
    }
}
