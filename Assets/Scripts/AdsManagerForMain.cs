using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManagerForMain : MonoBehaviour, IUnityAdsListener
{
    private BackToMap backToMap;

    void Start()
    {
        Advertisement.Initialize("4390037");
        Advertisement.AddListener(this);

        backToMap = FindObjectOfType<BackToMap>();
    }

    public void PlayRewardedAd()
    {
        if (Advertisement.IsReady("Rewarded_Android"))
        {
            Advertisement.Show("Rewarded_Android");
        }
        else
        {
            Debug.Log("Rewarded ad is not ready");
        }
    }

    public void PlayInterstitialAd()
    {
        if (Advertisement.IsReady("Interstitial_Android"))
        {
            Advertisement.Show("Interstitial_Android");
        }
        else
        {
            Debug.Log("Rewarded ad is not ready");
        }
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
            Debug.Log("Take diz");
            backToMap.AdsRetryButton();
        }
    }
}
