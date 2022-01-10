using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GoogleMobileAds.Api;

public class DailySpinManager : MonoBehaviour{

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

    private RewardBasedVideoAd adReward;
    private string idReward;

    [SerializeField]
    private Text textRewardedAdStatus;

    // Start is called before the first frame update
    void Start(){
        //  wheelButton = GetComponent<Button>();
        lastTimerSpunTime = ulong.Parse(PlayerPrefs.GetString("LastTimerSpunTime", "0"));
        isCoroutine = true;
        totalAngle = 360 / section;

        hitWheelButton = GameObject.FindWithTag("HitWheelButton");


        if (IsSpinWheelReady()){
            hitButton.SetActive(true);
            hitButtonAlias.SetActive(true);
        }
        else{
            //   wheelButton.interactable = true;
            hitButton.SetActive(false);
            hitButtonAlias.SetActive(false);
            collectCoinHolder.SetActive(false);
            watchAdButtonHolder.SetActive(true);
        }

        adReward = RewardBasedVideoAd.Instance;
        MobileAds.Initialize(initStatus => { });
        idReward = "ca-app-pub-3092873485358336/7734204616";
    }

    // Update is called once per frame
    void Update(){

        if (IsSpinWheelReady()){
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




    private bool IsSpinWheelReady(){
        CalculateTimerTime();

        if (CalculateTimerTime() < 0){
            spinWheelTimerText.text = "Ready!";
            return true;
        }
        else { return false; }

    }

    private float CalculateTimerTime(){
        ulong diff = ((ulong)DateTime.Now.Ticks - lastTimerSpunTime);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (float)(msToWaitForSpin - m) / 1000.0f;

        return secondsLeft;
    }


    public void SpinTheWheel(){

        if (isCoroutine){
            StartCoroutine(Spin());

            lastTimerSpunTime = (ulong)DateTime.Now.Ticks;
            PlayerPrefs.SetString("LastTimerSpunTime", lastTimerSpunTime.ToString());

        }else{ print("no spin"); }
    }

    public void CollectCoin(){
        Debug.Log("Coin Collected");
        hitButton.SetActive(false);
        hitButtonAlias.SetActive(false);
        collectCoinHolder.SetActive(false);
        watchAdButtonHolder.SetActive(true);
        textRewardedAdStatus.text = "Spin Again! Click Now";
        // CoinPrefManager.coins += scoreValueOfSpin;
        CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, scoreValueOfSpin);
        Debug.Log(scoreValueOfSpin);
        audioSource.PlayOneShot(coinClip);
    }

    public void WatchAdStarted(){
        Debug.Log("Watch Ad Started");
        textRewardedAdStatus.text = "Loading Ad...";
        RequestRewardAd();
    }

    //Function for testing click events

    public void TestInvisibleButton(){
        Debug.Log("working!");
    }

    private IEnumerator Spin(){
        //Hit Animation Here
        animHitButton.SetTrigger("SpiningTrig");
        isCoroutine = false;
        randVal = UnityEngine.Random.Range(200, 300);

        timeInterval = 0.0001f * Time.deltaTime * 2;

        for (int i = 0; i < randVal; i++){

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
        for (int i = 0; i < section; i++){

            if (finalAngle == i * totalAngle){
                winText.text = PrizeName[i].ToString();
                scoreValueOfSpin = PrizeName[i];
            }
        }
        isCoroutine = true;
    }


    #region Reward video methods ---------------------------------------------

    public void RequestRewardAd()
    {
        AdRequest request = AdRequestBuild();

        adReward.LoadAd(request, idReward);

        //adReward.LoadAd(request);

        adReward.OnAdLoaded += this.HandleOnRewardedAdLoaded;
        adReward.OnAdRewarded += this.HandleOnAdRewarded;
        adReward.OnAdClosed += this.HandleOnRewardedAdClosed;
    }

    public void ShowRewardAd(){
        if (adReward.IsLoaded()) { adReward.Show(); }
        else { textRewardedAdStatus.text = "Come Tommorrow! No More Ads Today"; }
    }

    //events
    public void HandleOnRewardedAdLoaded(object sender, EventArgs args){
        //ad loaded
        ShowRewardAd();
        
    }

    public void HandleOnAdRewarded(object sender, EventArgs args){
        //user finished watching ad
        hitButton.SetActive(true);
        hitButtonAlias.SetActive(true);
        collectCoinHolder.SetActive(false);
        watchAdButtonHolder.SetActive(false);
        spinWheelTimerText.text = "Ready!";
    }

    public void HandleOnRewardedAdClosed(object sender, EventArgs args){
        //ad closed (even if not finished watching)
        
        adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
        adReward.OnAdRewarded -= this.HandleOnAdRewarded;
        adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
    }

    #endregion

    AdRequest AdRequestBuild(){
        return new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("CC9DF9C9E1DABC49E2EDEF7455F8800D")
            .TagForChildDirectedTreatment(false)
            .Build();
    }
    void OnDestroy(){
        adReward.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
        adReward.OnAdRewarded -= this.HandleOnAdRewarded;
        adReward.OnAdClosed -= this.HandleOnRewardedAdClosed;
    }
}