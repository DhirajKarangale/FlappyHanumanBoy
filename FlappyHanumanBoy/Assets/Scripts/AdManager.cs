using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Advertisement.Initialize("4557605", true);
        // if (UnityEngine.Random.Range(0, 3) == 2) Invoke("ShowInterstitialAd", 2f);
    }

    // public void ShowInterstitialAd()
    // {
    //     // Check if UnityAds ready before calling Show method:
    //     if (Advertisement.IsReady())
    //     {
    //         Advertisement.Show("Interstitial_Android");
    //     }
    // }


    // public void ShowRewardedVideoAd(Text msText) 
    // {
    //     msgText = msText;
    //     if (Advertisement.IsReady("Rewarded_Android"))
    //     {
    //         Advertisement.Show("Rewarded_Android");
    //     }
    //     else
    //     {
    //         Debug.Log("Reward Ad is not loaded");
    //         if (msgText != null)
    //         {
    //             msgText.color = Color.white;
    //             msgText.text = "Ad not Loaded Try Again";
    //         }
    //     }
    // }

    // public void OnUnityAdsDidError(string message)
    // {
    //     if (msgText != null)
    //     {
    //         msgText.color = Color.white;
    //         msgText.text = "Error" + message;
    //     }
    // }

    // public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    // {
    //     if ((placementId == "Rewarded_Android") && (showResult == ShowResult.Finished))
    //     {
    //         // GameDataVariable.dataVariables[1] += 100;
    //        // Advertisement.RemoveListener(this);
    //         if (msgText != null)
    //         {
    //             msgText.color = Color.green;
    //             msgText.text = "You Received Reward";
    //         }
    //     }
    // }
}
