using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AdmobAdsScript : MonoBehaviour{

    private BannerView bannerView;
   
    // Start is called before the first frame update
    void Start(){

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        this.RequestBannerAd();

    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest(){
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("CC9DF9C9E1DABC49E2EDEF7455F8800D")
            .TagForChildDirectedTreatment(false)
            .Build();
    }

    #endregion

    #region BANNER ADS

    public void RequestBannerAd()
    {
        #if UNITY_EDITOR
        string adUnitId = "unused";
        #elif UNITY_ANDROID
           string adUnitId = "ca-app-pub-3092873485358336/3273049572";
        #else
            string adUnitId = "unexpected_platform";
        #endif


        // Clean up banner before reusing
        if (bannerView != null){
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    void OnDestroy()
    {
        DestroyBannerAd();
    }
    #endregion
}
