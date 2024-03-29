﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;

public class DailySpinManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{

    int randVal;
    private float timeInterval;
    private bool isCoroutine;
    private int finalAngle;
    // private Button wheelButton;
    private ulong lastTimerSpunTime;
    private float msToWaitForSpin = 2000000.0f;
    private GameObject hitWheelButton;
    private int scoreValueOfSpin;

    [SerializeField]
    private Animator animHitButton;

    //show Hide Objects Here
    [SerializeField]
    private GameObject hitButton, hitButtonAlias, collectCoinHolder, watchAdButtonHolder;

    [SerializeField]
    private TextMeshProUGUI spinWheelTimerText;
    public Text winText;
    public int section;
    float totalAngle;
    public int[] PrizeName;

    //For Coin Collect Sound
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip coinClip;

    // private RewardBasedVideoAd adReward;
    private string idReward;

    [SerializeField]
    private Text textRewardedAdStatus;

    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    // Start is called before the first frame update
    void Start()
    {
        _adUnitId = _androidAdUnitId;
        LoadAd();
        //  wheelButton = GetComponent<Button>();
        lastTimerSpunTime = ulong.Parse(PlayerPrefs.GetString("LastTimerSpunTime", "0"));
        isCoroutine = true;
        totalAngle = 360 / section;

        hitWheelButton = GameObject.FindWithTag("HitWheelButton");


        if (IsSpinWheelReady())
        {
            hitButton.SetActive(true);
            hitButtonAlias.SetActive(true);
        }
        else
        {
            //   wheelButton.interactable = true;
            hitButton.SetActive(false);
            hitButtonAlias.SetActive(false);
            collectCoinHolder.SetActive(false);
            watchAdButtonHolder.SetActive(true);
        }

        //       adReward = RewardBasedVideoAd.Instance;
        //       MobileAds.Initialize(initStatus => { });
        idReward = "ca-app-pub-3092873485358336/7734204616";
    }

    // Update is called once per frame
    void Update()
    {

        if (IsSpinWheelReady())
        {
            //  wheelButton.interactable = false;
            hitButton.SetActive(true);
            hitButtonAlias.SetActive(true);

            collectCoinHolder.SetActive(false);
            watchAdButtonHolder.SetActive(false);
            return;
        }

        float secondsLeft = CalculateTimerTime();
        string r = "";
        // Hours
        r += ((int)secondsLeft / 3600).ToString("00") + ":";
        secondsLeft -= ((int)secondsLeft / 3600) * 3600;
        // Minutes
        r += ((int)secondsLeft / 60).ToString("00") + ":";
        // Seconds
        r += (secondsLeft % 60).ToString("00");
        spinWheelTimerText.text = r;
    }




    private bool IsSpinWheelReady()
    {
        CalculateTimerTime();

        if (CalculateTimerTime() < 0)
        {
            spinWheelTimerText.text = "Ready!";
            return true;
        }
        else { return false; }

    }

    private float CalculateTimerTime()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastTimerSpunTime);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (float)(msToWaitForSpin - m) / 1000.0f;

        return secondsLeft;
    }


    public void SpinTheWheel()
    {

        if (isCoroutine)
        {
            StartCoroutine(Spin());

            lastTimerSpunTime = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString("LastTimerSpunTime", lastTimerSpunTime.ToString());

        }
        else { print("no spin"); }
    }

    public void CollectCoin()
    {
        Debug.Log("Coin Collected");
        hitButton.SetActive(false);
        hitButtonAlias.SetActive(false);
        collectCoinHolder.SetActive(false);
        watchAdButtonHolder.SetActive(true);
        textRewardedAdStatus.text = "Spin Again! Click Now";
        // CoinPrefManager.coins += scoreValueOfSpin;
        CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, scoreValueOfSpin,false);
        Debug.Log(scoreValueOfSpin);
        audioSource.PlayOneShot(coinClip);
    }

    public void WatchAdStarted()
    {
        Debug.Log("Watch Ad Started");
        //textRewardedAdStatus.text = "No Ad Available";
        // textRewardedAdStatus.text = "Loading Ad...";
        //RequestRewardAd();
        ShowAd();
    }

    //Function for testing click events

    public void TestInvisibleButton()
    {
        Debug.Log("working!");
    }

    private IEnumerator Spin()
    {
        //Hit Animation Here
        animHitButton.SetTrigger("SpiningTrig");
        isCoroutine = false;
        randVal = UnityEngine.Random.Range(200, 300);

        timeInterval = 0.0001f * Time.deltaTime * 2;

        for (int i = 0; i < randVal; i++)
        {

            transform.Rotate(0, 0, (totalAngle / 2));

            //To Slow down wheel
            if (i > Mathf.RoundToInt(randVal * 0.2f))
                timeInterval = 0.5f * Time.deltaTime;
            if (i > Mathf.RoundToInt(randVal * 0.5f))
                timeInterval = 1f * Time.deltaTime;
            if (i > Mathf.RoundToInt(randVal * 0.7f))
                timeInterval = 1.5f * Time.deltaTime;
            if (i > Mathf.RoundToInt(randVal * 0.8f))
                timeInterval = 2f * Time.deltaTime;
            if (i > Mathf.RoundToInt(randVal * 0.9f))
                timeInterval = 2.5f * Time.deltaTime;

            yield return new WaitForSeconds(timeInterval);
        }

        if (Mathf.RoundToInt(transform.eulerAngles.z) % totalAngle != 0) //when indicator stops between 2 numbers, it will add additional step
            transform.Rotate(0, 0, totalAngle / 2);

        finalAngle = Mathf.RoundToInt(transform.eulerAngles.z);

        print(finalAngle);

        // Show Hide once final angle gets printed
        hitButtonAlias.SetActive(false);
        hitButton.SetActive(false);
        collectCoinHolder.SetActive(true);
        watchAdButtonHolder.SetActive(false);

        //score value check
        for (int i = 0; i < section; i++)
        {

            if (finalAngle == i * totalAngle)
            {
                winText.text = PrizeName[i].ToString();
                scoreValueOfSpin = PrizeName[i];
            }
        }
        isCoroutine = true;
    }


    #region Reward video methods ---------------------------------------------

    // public void RequestRewardAd()
    // {
    //     AdRequest request = AdRequestBuild();

    //     adReward.LoadAd(request, idReward);

    //     //adReward.LoadAd(request);

    //     adReward.OnAdLoaded += this.HandleOnRewardedAdLoaded;
    //     adReward.OnAdRewarded += this.HandleOnAdRewarded;
    //     adReward.OnAdClosed += this.HandleOnRewardedAdClosed;
    // }

    // public void ShowRewardAd(){
    //     if (adReward.IsLoaded()) { adReward.Show(); }
    //     else { textRewardedAdStatus.text = "Come Tommorrow! No More Ads Today"; }
    // }

    //events
    // public void HandleOnRewardedAdLoaded(object sender, EventArgs args){
    //     //ad loaded
    //     ShowRewardAd();

    // }

    // public void HandleOnAdRewarded(object sender, EventArgs args)
    // {
    //     //user finished watching ad
    //     hitButton.SetActive(true);
    //     hitButtonAlias.SetActive(true);
    //     collectCoinHolder.SetActive(false);
    //     watchAdButtonHolder.SetActive(false);
    //     spinWheelTimerText.text = "Ready!";
    // }

    // public void HandleOnRewardedAdClosed(object sender, EventArgs args){
    //     //ad closed (even if not finished watching)

    //     adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
    //     adReward.OnAdRewarded -= this.HandleOnAdRewarded;
    //     adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
    // }

    #endregion

    // AdRequest AdRequestBuild(){
    //     return new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
    //         .AddTestDevice("CC9DF9C9E1DABC49E2EDEF7455F8800D")
    //         .TagForChildDirectedTreatment(false)
    //         .Build();
    // }
    // void OnDestroy(){
    //     adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
    //     adReward.OnAdRewarded -= this.HandleOnAdRewarded;
    //     adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
    // }




    // public void ShowRewardedVideoAd()
    // {
    //     Advertisement.Show("Rewarded_Android");
    //     // if (Advertisement.IsReady("Rewarded_Android"))
    //     // {
    //     //     Advertisement.Show("Rewarded_Android");
    //     //     Debug.Log("Showing Ad");
    //     // }
    //     // else
    //     // {
    //     //     Debug.Log("Reward Ad is not loaded");
    //     //     if (textRewardedAdStatus != null)
    //     //     {
    //     //         textRewardedAdStatus.color = Color.white;
    //     //         textRewardedAdStatus.text = "Ad not Loaded Try Again";
    //     //     }
    //     // }
    // }

    // public void OnUnityAdsDidError(string message)
    // {
    //     textRewardedAdStatus.color = Color.white;
    //     textRewardedAdStatus.text = "Error" + message;
    //     Debug.Log("Showing Ad Error");
    // }

    // public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    // {
    //     if ((placementId == "Rewarded_Android") && (showResult == ShowResult.Finished))
    //     {
    //         // Grant a reward.
    //         hitButton.SetActive(true);
    //         hitButtonAlias.SetActive(true);
    //         collectCoinHolder.SetActive(false);
    //         watchAdButtonHolder.SetActive(false);
    //         spinWheelTimerText.text = "Ready!";
    //         //Advertisement.RemoveListener(this);
    //         textRewardedAdStatus.color = Color.green;
    //         textRewardedAdStatus.text = "You Received Reward";
    //         Debug.Log("Get Reward " + scoreValueOfSpin);
    //     }
    // }





    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        textRewardedAdStatus.text = "Loading Ad: ";
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            // Enable the button for users to click:
            //  _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button.
    public void ShowAd()
    {
        // Disable the button: 
        //  _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            hitButton.SetActive(true);
            hitButtonAlias.SetActive(true);
            collectCoinHolder.SetActive(false);
            watchAdButtonHolder.SetActive(false);
            spinWheelTimerText.text = "Ready!";

            textRewardedAdStatus.text = "Reward Get, More Spin";


            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        // _showAdButton.onClick.RemoveAllListeners();
    }

}
