using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
// using GoogleMobileAds.Api;

public class GameplayController : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static GameplayController instance;

    [SerializeField]
    private Text scoreText, endScore, bestScore, gameOvertext, coinCollectedScore;

    [SerializeField]
    private Button restartGameButton, instructionsButton, coinHolderButton, closeAddCoinButton, noThanksAdsButton;

    [SerializeField]
    private GameObject pausePanel, addCoinPanel, flyingHanuman, hitWatchAdPanel;

    [SerializeField]
    private GameObject[] birds;

    [SerializeField]
    private Sprite[] medals;

    [SerializeField]
    private Image medalImage;

    [SerializeField]
    private Text chancesLeftText, noThanksAdsText;

    private int currentScore, coinScoreGame;

    //   private RewardBasedVideoAd adRewardedEarnLives;
    private string idRewardedAd, idInterstitialAd;
    //   private InterstitialAd adInterstitialGoToMenu;

    public static int adLifeChancesBird = 0;

    public static int currentScoreSt, coinScoreGameSt;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId;

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
        MakeInstance();
        Time.timeScale = 0f;
    }

    void Start()
    {

        LoadAd();
        //      adRewardedEarnLives = RewardBasedVideoAd.Instance;
        //     MobileAds.Initialize(initStatus => { });
        idRewardedAd = "ca-app-pub-3092873485358336/8888338330";
        idInterstitialAd = "ca-app-pub-3092873485358336/8851721450";
    }

    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PauseGame()
    {
        if (BirdScript.instance != null)
        {
            if (BirdScript.instance.isAlive)
            {
                pausePanel.SetActive(true);
                gameOvertext.gameObject.SetActive(false);
                endScore.text = BirdScript.instance.score.ToString();
                coinCollectedScore.text = BirdScript.instance.coinScore.ToString();
                bestScore.text = GameController.instance.GetHighScore().ToString();
                Time.timeScale = 0f;
                restartGameButton.onClick.RemoveAllListeners();
                restartGameButton.onClick.AddListener(() => ResumeGame());
            }
        }

    }

    public void GoToMenuButton()
    {
        // if (Application.platform == RuntimePlatform.Android)
        // { RequestInterstitialAd(); }
        // else { SceneFader.instance.FadeIn("CustomMenu"); }

        SceneFader.instance.FadeIn("CustomMenu");

    }
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

    }

    public void RestartGame(int score, int coinScore)
    {
        currentScoreSt = score;
        coinScoreGameSt = coinScore;
        SceneFader.instance.FadeIn(SceneManager.GetActiveScene().name);

    }

    public void ShowCoinAddPanel()
    {
        addCoinPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void HideCoinAddPanel()
    {
        addCoinPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PlayGame()
    {
        scoreText.gameObject.SetActive(true);
        flyingHanuman.SetActive(true);
        //remove the line immediately below
        // birds[1].SetActive(true);
        // Comment the line below and uncomment the one above to play directly from Game Scene
        birds[GameController.instance.GetSelectedBird()].SetActive(true);
        instructionsButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
        SetScore(currentScoreSt);
        currentScoreSt = 0;
        coinScoreGameSt = 0;

    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void WatchAd()
    {
        Debug.Log("Watching Ad");
        // //hitWatchAdPanel.SetActive(false);
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     noThanksAdsText.text = "Loading Ad....";
        //     noThanksAdsButton.interactable = false;
        //     // RequestRewardAd();
        // }
        // else { IfEditorIsUnityAdmobSkip(); }
        ShowAd();

    }

    public void ShowHitWatchAdPanel(int score, int coinScore)
    {
        hitWatchAdPanel.SetActive(true);
        currentScore = score;
        coinScoreGame = coinScore;
        chancesLeftText.text = (3 - adLifeChancesBird).ToString();

        noThanksAdsText.text = "No Thanks";
        noThanksAdsButton.interactable = true;
        noThanksAdsButton.onClick.RemoveAllListeners();
        noThanksAdsButton.onClick.AddListener(() => PlayerDiedShowScore(currentScore, coinScoreGame));

    }

    public void PlayerDiedShowScore(int score, int coinScore)
    {

        pausePanel.SetActive(true);
        hitWatchAdPanel.SetActive(false);
        gameOvertext.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        currentScore = score;
        coinScoreGame = coinScore;

        endScore.text = currentScore.ToString();
        coinCollectedScore.text = coinScoreGame.ToString();


        if (currentScore > GameController.instance.GetHighScore())
        {
            GameController.instance.SetHighScore(currentScore);
        }

        bestScore.text = GameController.instance.GetHighScore().ToString();

        if (currentScore <= 20) { medalImage.sprite = medals[0]; }
        else if (currentScore > 20 && currentScore < 40) { medalImage.sprite = medals[1]; }
        else if (currentScore > 40 && currentScore < 60) { medalImage.sprite = medals[2]; }
        else if (currentScore > 60 && currentScore < 100) { medalImage.sprite = medals[3]; }
        else { medalImage.sprite = medals[4]; }


        restartGameButton.onClick.RemoveAllListeners();
        restartGameButton.onClick.AddListener(() => RestartGame(0, 0));

        currentScoreSt = 0;
        coinScoreGameSt = 0;
    }


    #region Reward video methods ---------------------------------------------

    // public void RequestRewardAd(){

    //     AdRequest request = AdRequestBuild();
    //     adRewardedEarnLives.LoadAd(request, idRewardedAd);

    //     adRewardedEarnLives.OnAdLoaded += this.HandleOnRewardedAdLoaded;
    //     adRewardedEarnLives.OnAdRewarded += this.HandleOnAdRewarded;
    //     adRewardedEarnLives.OnAdClosed += this.HandleOnRewardedAdClosed;
    // }

    // public void ShowRewardAd(){
    //     if (adRewardedEarnLives.IsLoaded()) { adRewardedEarnLives.Show(); }
    //     else { SceneFader.instance.FadeIn("CustomMenu"); }
    // }

    //events
    public void HandleOnRewardedAdLoaded(object sender, EventArgs args)
    {
        //ad loaded
        //       ShowRewardAd();
    }

    public void HandleOnAdRewarded(object sender, EventArgs args)
    {
        //user finished watching ad

        adLifeChancesBird++;
        Debug.Log("adLifeChancesBird++ After HandleRewarded Gameplaycontroller #198: " + adLifeChancesBird);
        hitWatchAdPanel.SetActive(false);
        RestartGame(currentScore, coinScoreGame);
    }

    // public void HandleOnRewardedAdClosed(object sender, EventArgs args){
    //     //ad closed (even if not finished watching)
    //     PlayerDiedShowScore(currentScore, coinScoreGame);
    //     adRewardedEarnLives.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
    //     adRewardedEarnLives.OnAdRewarded -= this.HandleOnAdRewarded;
    //     adRewardedEarnLives.OnAdClosed -= this.HandleOnRewardedAdClosed;
    // }

    #endregion


    // AdRequest AdRequestBuild(){
    //     return new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
    //         .AddTestDevice("CC9DF9C9E1DABC49E2EDEF7455F8800D")
    //         .TagForChildDirectedTreatment(false)
    //         .Build();
    // }
    // void OnDestroy() {
    //     adRewardedEarnLives.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
    //     adRewardedEarnLives.OnAdRewarded -= this.HandleOnAdRewarded;
    //     adRewardedEarnLives.OnAdClosed -= this.HandleOnRewardedAdClosed;

    //     DestroyInterstitialAd();
    //     //dettach events
    //     adInterstitialGoToMenu.OnAdLoaded -= this.HandleOnAdLoaded;
    //     adInterstitialGoToMenu.OnAdOpening -= this.HandleOnAdOpening;
    //     adInterstitialGoToMenu.OnAdClosed -= this.HandleOnAdClosed;

    // }

    private void IfEditorIsUnityAdmobSkip()
    {
        adLifeChancesBird++;
        Debug.Log("adLifeChancesBird++ After IfEditorUnity Gameplaycontroller #227: " + adLifeChancesBird);
        hitWatchAdPanel.SetActive(false);
        RestartGame(currentScore, coinScoreGame);
    }

    #region Interstitial methods ---------------------------------------------

    // public void RequestInterstitialAd()
    // {
    //     adInterstitialGoToMenu = new InterstitialAd(idInterstitialAd);
    //     AdRequest request = AdRequestBuild();
    //     adInterstitialGoToMenu.LoadAd(request);

    //     //attach events
    //     adInterstitialGoToMenu.OnAdLoaded += this.HandleOnAdLoaded;
    //     adInterstitialGoToMenu.OnAdOpening += this.HandleOnAdOpening;
    //     adInterstitialGoToMenu.OnAdClosed += this.HandleOnAdClosed;
    //     adInterstitialGoToMenu.OnAdFailedToLoad += this.HandleFailedToLoad;
    //     adInterstitialGoToMenu.OnAdLeavingApplication += this.HandleAdleavingApplication;
    // }

    // public void ShowInterstitialAd()
    // {
    //     if (adInterstitialGoToMenu.IsLoaded())
    //     {
    //         adInterstitialGoToMenu.Show();
    //     }


    // }

    // public void DestroyInterstitialAd() {
    //     if (Application.platform == RuntimePlatform.Android){
    //         adInterstitialGoToMenu.Destroy();
    //     }
    // }

    //interstitial ad events
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        //this method executes when interstitial ad is Loaded and ready to show
        //BtnInterstitial.interactable = true; //button is ready to click (enabled)
        //   ShowInterstitialAd();
    }

    public void HandleOnAdOpening(object sender, EventArgs args)
    {
        //this method executes when interstitial ad is shown
        // BtnInterstitial.interactable = false; //disable the button
        SceneFader.instance.FadeIn("CustomMenu");
    }

    public void HandleFailedToLoad(object sender, EventArgs args)
    {
        SceneFader.instance.FadeIn("CustomMenu");
    }

    public void HandleAdleavingApplication(object sender, EventArgs args)
    {
        SceneFader.instance.FadeIn("CustomMenu");
    }



    // public void HandleOnAdClosed(object sender, EventArgs args)
    // {
    //     //this method executes when interstitial ad is closed
    //     adInterstitialGoToMenu.OnAdLoaded -= this.HandleOnAdLoaded;
    //     adInterstitialGoToMenu.OnAdOpening -= this.HandleOnAdOpening;
    //     adInterstitialGoToMenu.OnAdClosed -= this.HandleOnAdClosed;

    //    SceneFader.instance.FadeIn("CustomMenu");

    // }

    #endregion


    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
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
            adLifeChancesBird++;
            Debug.Log("adLifeChancesBird++ After HandleRewarded Gameplaycontroller #198: " + adLifeChancesBird);
            hitWatchAdPanel.SetActive(false);
            RestartGame(currentScore, coinScoreGame);
                noThanksAdsText.text = "Get Life ";

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        noThanksAdsText.text = "Ad fail to Load ";
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        noThanksAdsText.text = "Ad fail to Show ";
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
