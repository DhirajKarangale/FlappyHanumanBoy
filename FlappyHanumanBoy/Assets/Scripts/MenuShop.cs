using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuShop : MonoBehaviour
{

    [System.Serializable]
    public class AvatarEnvItemList {
        public Sprite ImageAvatarEnv;
        public int CoinPrice;
        public RuntimeAnimatorController AnimateAvatarEnv;
        public bool IsPurchased = false;
        public bool IsSelected = false;
    }

    public List<AvatarEnvItemList> avatarItemLists;
    public List<AvatarEnvItemList> envItemLists;

    [SerializeField] GameObject exitPanel;
    [SerializeField] GameObject AvatarUITemplate;
    [SerializeField] GameObject EnvironmentUITemplate;
    [SerializeField] Transform AvatarsScrollView;
    [SerializeField] Transform EnvironmentScrollView;
    [SerializeField] Sprite SelectIcon, UnselectIcon, LockIcon;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] GameObject addCoinPanel;
    [SerializeField] Color ActiveAvatarColor;
    [SerializeField] Color DefaultAvatarColor;
    [SerializeField] Image AvatarImageModal;
    [SerializeField] Text CoinValueAvatarImageModal;
    [SerializeField] Image EnvironmentImageModal;
    [SerializeField] Button BuyNowModal, coinHolderButton, closeAddCoinButton;
    [SerializeField] GameObject WatchAdAnimationObject;
    [SerializeField] Animator NoCoinsAnim;

    GameObject g;
    Button AvatarImageButton, EnvImageButton;
    private ulong lastTimerSpunTime;
    private float msToWaitForSpin = 2000000.0f;

    [SerializeField]
    private Text SmallSpinWheelTimerText;

    [SerializeField]
    private Text textRewardedAdStatus;


    // Start is called before the first frame update
    void Start(){
        CheckPrefPlayerSelection();
        exitPanel.SetActive(false);
        // Add All Avatars and Environment To Menu
        lastTimerSpunTime = ulong.Parse(PlayerPrefs.GetString("LastTimerSpunTime", "0"));
        ReloadAvatarMenu();
        ReloadEnvironmentMenu();
        // if (Application.platform == RuntimePlatform.Android) {  ConnectOnGooglePlayGames(); }
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            exitPanel.SetActive(true);
        }

        if (IsSpinWheelReady()){
            SmallSpinWheelTimerText.text = "Ready!";
        }
        else{
            float secondsLeft = CalculateTimerTime();
            string r = "";
            // Hours
            r += ((int)secondsLeft / 3600).ToString("00") + ":";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            // Minutes
            r += ((int)secondsLeft / 60).ToString("00") + ":";
            // Seconds
            r += (secondsLeft % 60).ToString("00");
            SmallSpinWheelTimerText.text = r;
        }

    }

    void CheckPrefPlayerSelection() { 
       
        for(int i = 0; i< avatarItemLists.Capacity;i++)
        {
            if(PlayerPrefs.GetInt(avatarItemLists[i].CoinPrice + "AVT",0) == 1)
            {
                avatarItemLists[i].IsPurchased = true;
                Debug.Log(avatarItemLists[i] + " is Purchased unlock ");
            }
            else
            {
                avatarItemLists[i].IsPurchased = false;
            }
        }


        for(int i = 0; i< envItemLists.Capacity; i++)
        {
            if(PlayerPrefs.GetInt(envItemLists[i].CoinPrice + "ENV",0) == 1)
            {
                envItemLists[i].IsPurchased = true;
            }
            else
            {
                envItemLists[i].IsPurchased = false;
            }
        }

        avatarItemLists[0].IsPurchased = true;
        avatarItemLists[1].IsPurchased = true;

        envItemLists[0].IsPurchased = true;
        envItemLists[1].IsPurchased = true;
        
        avatarItemLists[GameController.instance.GetSelectedBird()].IsSelected = true;
        envItemLists[GameController.instance.GetSelectedWorld()].IsSelected = true;
    }

    public void PlayGame(){
        SceneFader.instance.FadeIn("Gameplay");
    }

    public void LaunchDailySpinScene(){
        SceneFader.instance.FadeIn("DailySpin");
    }

    void AddAvatarToMenu(int index) {
        g = Instantiate(AvatarUITemplate, AvatarsScrollView);
        g.transform.GetChild(0).GetComponent<Image>().sprite = avatarItemLists[index].ImageAvatarEnv;
        AvatarImageButton = g.transform.GetComponent<Button>();
        AvatarImageButton.AddEventListener(index, AvatarImageButtonClick);

        if (!avatarItemLists[index].IsPurchased) {
            g.transform.GetChild(1).GetComponent<Image>().sprite = LockIcon;
            g.transform.GetComponent<Image>().color = DefaultAvatarColor;
        }
        else {
            if (avatarItemLists[index].IsSelected) {
                g.transform.GetChild(1).GetComponent<Image>().sprite = SelectIcon;
                g.transform.GetComponent<Image>().color = ActiveAvatarColor;
                GameController.instance.SetSelectedBird(index);
            }
            else{ g.transform.GetChild(1).GetComponent<Image>().sprite = UnselectIcon;
                g.transform.GetComponent<Image>().color = DefaultAvatarColor;
            }            
        }
        
    }

    void AddEnvToMenu(int index) {
        g = Instantiate(EnvironmentUITemplate, EnvironmentScrollView);
        g.transform.GetChild(0).GetComponent<Image>().sprite = envItemLists[index].ImageAvatarEnv;
        EnvImageButton = g.transform.GetComponent<Button>();
        EnvImageButton.AddEventListener(index, EnvImageButtonClick);


        if (!envItemLists[index].IsPurchased){
            g.transform.GetChild(1).GetComponent<Image>().sprite = LockIcon;
            g.transform.GetComponent<Image>().color = DefaultAvatarColor;
        }
        else{
            if (envItemLists[index].IsSelected){
                g.transform.GetChild(1).GetComponent<Image>().sprite = SelectIcon;
                g.transform.GetComponent<Image>().color = ActiveAvatarColor;
                GameController.instance.SetSelectedWorld(index);
            }
            else{
                g.transform.GetChild(1).GetComponent<Image>().sprite = UnselectIcon;
                g.transform.GetComponent<Image>().color = DefaultAvatarColor;
            }
        }
    }

    void ReloadEnvironmentMenu() {
        foreach (Transform child in EnvironmentScrollView) { Destroy(child.gameObject); }
        for (int i = 0; i < envItemLists.Count; i++){
            AddEnvToMenu(i);
        }
    }

    void ReloadAvatarMenu() {

        foreach(Transform child in AvatarsScrollView) { Destroy(child.gameObject); }
        
        for(int i=0; i < avatarItemLists.Count; i++) { AddAvatarToMenu(i); }
    }

     void AvatarImageButtonClick(int index) {
        if (!avatarItemLists[index].IsPurchased){
            OpenShop(index, 1);
        }
        else {
            if (!avatarItemLists[index].IsSelected) {
                ChangeSelectedAvatar(index);
                Debug.Log("New Selection");
            }
            else { Debug.Log("Already Selected"); }
                
            Debug.Log("It is Purchased");
        }

        ReloadAvatarMenu();

    }

    void EnvImageButtonClick(int index) {
        if (!envItemLists[index].IsPurchased) {
            OpenShop(index, 2);
        }
        else {
            if (!envItemLists[index].IsSelected) {
                ChangeSelectedEnvironment(index);
                Debug.Log("New Environment Selection");
            }
            else { Debug.Log("Already Env Selected"); }
        }
        ReloadEnvironmentMenu();
    }

    public void OpenShop(int index, int envAvValue) {
        ShopPanel.SetActive(true);
      
        if (envAvValue == 1){
            // AvatarImageModal.GetComponent<Image>().sprite = avatarItemLists[index].ImageAvatarEnv;
            // RectTransform rt = AvatarImageModal.GetComponent<RectTransform>();
            // rt.sizeDelta = new Vector2(224, 224);
            AvatarImageModal.GetComponent<Animator>().runtimeAnimatorController = avatarItemLists[index].AnimateAvatarEnv;
            CoinValueAvatarImageModal.GetComponent<Text>().text = avatarItemLists[index].CoinPrice.ToString();

            if (Game.Instance.HasEnoughCoins(avatarItemLists[index].CoinPrice))
            {
                BuyNowModal.gameObject.SetActive(true);
                WatchAdAnimationObject.gameObject.SetActive(false);
                NoCoinsAnim.gameObject.SetActive(false);
                BuyNowModal.onClick.AddListener(() => BuyAvatar(index));
            }
            else{
                BuyNowModal.gameObject.SetActive(false);
                WatchAdAnimationObject.gameObject.SetActive(true);
                NoCoinsAnim.gameObject.SetActive(true);
                NoCoinsAnim.SetTrigger("NoCoins");
            }
        }
        else {
            //  AvatarImageModal.GetComponent<Image>().sprite = envItemLists[index].ImageAvatarEnv;
            // RectTransform rt = AvatarImageModal.GetComponent<RectTransform>();
            // rt.sizeDelta = new Vector2(340, 224);

            AvatarImageModal.GetComponent<Animator>().runtimeAnimatorController = envItemLists[index].AnimateAvatarEnv;
            CoinValueAvatarImageModal.GetComponent<Text>().text = envItemLists[index].CoinPrice.ToString();

            if (Game.Instance.HasEnoughCoins(envItemLists[index].CoinPrice))
            {
                BuyNowModal.gameObject.SetActive(true);
                WatchAdAnimationObject.gameObject.SetActive(false);
                NoCoinsAnim.gameObject.SetActive(false);
                BuyNowModal.onClick.AddListener(() => BuyEnvironment(index));
            }
            else
            {
                BuyNowModal.gameObject.SetActive(false);
                WatchAdAnimationObject.gameObject.SetActive(true);
                NoCoinsAnim.gameObject.SetActive(true);
                NoCoinsAnim.SetTrigger("NoCoins");
            }
        }  

    }

    public void CloseShop() { ShopPanel.SetActive(false); ReloadAvatarMenu(); ReloadEnvironmentMenu(); }

    public void ShowCoinAddPanel(){
        addCoinPanel.SetActive(true);
        Time.timeScale = 1f;
        textRewardedAdStatus.text = "Earn Coins For Free!";
    }
    public void HideCoinAddPanel(){
        addCoinPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ChangeSelectedAvatar(int index) {
        for(int i = 0; i < avatarItemLists.Count; i++) {
            avatarItemLists[i].IsSelected = false;
        }
        avatarItemLists[index].IsSelected = true;
        GameController.instance.SetSelectedBird(index);
    }

    void ChangeSelectedEnvironment(int index) {
        for (int i = 0; i < envItemLists.Count; i++){
            envItemLists[i].IsSelected = false;
        }
        envItemLists[index].IsSelected = true;
        GameController.instance.SetSelectedWorld(index);
    }

    void BuyAvatar(int index) {
        //purchase Avatar
        Game.Instance.UseCoins(avatarItemLists[index].CoinPrice);
        avatarItemLists[index].IsPurchased = true;

        PlayerPrefs.SetInt(avatarItemLists[index].CoinPrice + "AVT",1);
        Debug.Log(avatarItemLists[index].ToString() + " Purchase");
        
        Game.Instance.UpdateAllCoinsUiText();
        CoinPrefManager.instance.UpdateCoins(BuyNowModal.transform.position, -avatarItemLists[index].CoinPrice,false);
        CloseShop();
    }

    void BuyEnvironment(int index) {
        //purchase Environment
        Game.Instance.UseCoins(envItemLists[index].CoinPrice);
        envItemLists[index].IsPurchased = true;

        PlayerPrefs.SetInt(envItemLists[index].CoinPrice + "ENV",1);
        Debug.Log(envItemLists[index].ToString() + " Purchase");

        Game.Instance.UpdateAllCoinsUiText();
        CoinPrefManager.instance.UpdateCoins(BuyNowModal.transform.position, -envItemLists[index].CoinPrice,false);
        CloseShop();
    }



    private float CalculateTimerTime(){
        ulong diff = ((ulong)DateTime.Now.Ticks - lastTimerSpunTime);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (float)(msToWaitForSpin - m) / 1000.0f;

        return secondsLeft;
    }

    private bool IsSpinWheelReady(){
        CalculateTimerTime();
        if (CalculateTimerTime() < 0)   { return true; }
        else { return false; }

    }

    public void ExitYesButton()
    {
        Application.Quit();
    }

    public void ExitNoButton()
    {
        exitPanel.SetActive(false);
    }


    // void ConnectOnGooglePlayGames() { LeaderboardsController.instance.ConnectOrDisconnectOnGooglePlayames(); }
    // public void OpenLeaderboardScoreUI() { LeaderboardsController.instance.OpenLeaderboardScore(); }

}
