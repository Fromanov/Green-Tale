using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    public GameObject baneerPanel;

    private SpinController wheel;

    void Start()
    {        
        Advertisement.Initialize("4390037");
        Advertisement.AddListener(this);

        ShowBanner();

        wheel = FindObjectOfType<SpinController>();
    }    

    public void PlayRewardedAd()
    {
        if(Advertisement.IsReady("Rewarded_Android"))
        {
            Advertisement.Show("Rewarded_Android"); 
        }
        else 
        {
            Debug.Log("Rewarded ad is not ready");
        }
    }

    public void ShowBanner()
    {
        if (Advertisement.IsReady("Banner_Android"))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show("Banner_Android");
        }
        else 
        {
            StartCoroutine(RepeatShowBanner());
        }        
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide(false);
    }

    IEnumerator RepeatShowBanner()
    {
        yield return new WaitForSeconds(1);
        ShowBanner();        
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Ad Error");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Start ad video");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == "Rewarded_Android" && showResult == ShowResult.Finished)
        {
            Debug.Log("Take diz"); //делаем тут автоспин

            wheel.AdsSpinClick();
        }
    }
}
