using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class IAPManagerScript : MonoBehaviour, IStoreListener{

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    public static IAPManagerScript instance { set; get; }

    public static string PRODUCT_RUPEES19 = "19_rupees";
    public static string PRODUCT_RUPEES49 = "49_rupees";
    public static string PRODUCT_RUPEES199 = "199_rupees";
    public static string PRODUCT_RUPEES499 = "499_rupees";
    public static string kProductIdSubscription = "subscription";

    [SerializeField]
    private AudioClip coinPurchasedClip;
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private GameObject collectCoinHolder;
    [SerializeField]
    private Text textRewardedAdStatus;

    private RewardBasedVideoAd adRewardCoins;
    private string idReward;

    private void Awake(){
        instance = this;
    }

    void Start() {
        if(m_StoreController == null) { InitializePurchasing(); }

        adRewardCoins = RewardBasedVideoAd.Instance;
        MobileAds.Initialize(initStatus => { });
        idReward = "ca-app-pub-3092873485358336/9364294131";
    }

    public void InitializePurchasing() {

        if (IsInitialized()) {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(PRODUCT_RUPEES19, ProductType.Consumable);
        builder.AddProduct(PRODUCT_RUPEES49, ProductType.Consumable);
        builder.AddProduct(PRODUCT_RUPEES199, ProductType.Consumable);
        builder.AddProduct(PRODUCT_RUPEES499, ProductType.Consumable);
    }

    private bool IsInitialized() {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyRupees19() { BuyProductID(PRODUCT_RUPEES19); }
    public void BuyRupees49() { BuyProductID(PRODUCT_RUPEES49); }
    public void BuyRupees199() { BuyProductID(PRODUCT_RUPEES199); }
    public void BuyRupees499() { BuyProductID(PRODUCT_RUPEES499); }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase) {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions){
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error){
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

   

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
       
        if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_RUPEES19, StringComparison.Ordinal)){
            Debug.Log(string.Format("19 Rupees Coin Purchase Success!", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, 2000);
            audioSource.PlayOneShot(coinPurchasedClip);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_RUPEES49, StringComparison.Ordinal)){
            Debug.Log(string.Format("49 Rupees Coin Purchase Success!", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, 5000);
            audioSource.PlayOneShot(coinPurchasedClip);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_RUPEES199, StringComparison.Ordinal)){
            Debug.Log(string.Format("199 Rupees Coin Purchase Success!", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, 80000);
            audioSource.PlayOneShot(coinPurchasedClip);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, PRODUCT_RUPEES499, StringComparison.Ordinal)) {
            Debug.Log(string.Format("199 Rupees Coin Purchase Success!", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, 500000);
            audioSource.PlayOneShot(coinPurchasedClip);
        }

        else{
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {

        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }




    #region Reward video methods ---------------------------------------------

    public void RequestRewardAd(){
        AdRequest request = AdRequestBuild();

        adRewardCoins.LoadAd(request, idReward);

        //adReward.LoadAd(request);

        adRewardCoins.OnAdLoaded += this.HandleOnRewardedAdLoaded;
        adRewardCoins.OnAdRewarded += this.HandleOnAdRewarded;
        adRewardCoins.OnAdClosed += this.HandleOnRewardedAdClosed;

        textRewardedAdStatus.text = "Loading Ad! Please Wait...";
    }

    public void ShowRewardAd()
    {
        if (adRewardCoins.IsLoaded()) { adRewardCoins.Show(); }
        else {

            textRewardedAdStatus.text = "Come Tommorrow! No More Ads Today";
        }
    }

    //events
    public void HandleOnRewardedAdLoaded(object sender, EventArgs args)
    {
        //ad loaded
        ShowRewardAd();

    }

    public void HandleOnAdRewarded(object sender, EventArgs args)
    {
        //user finished watching ad
        CoinPrefManager.instance.UpdateCoins(collectCoinHolder.transform.position, 50);
        audioSource.PlayOneShot(coinPurchasedClip);
    }

    public void HandleOnRewardedAdClosed(object sender, EventArgs args)
    {
        //ad closed (even if not finished watching)

        adRewardCoins.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
        adRewardCoins.OnAdRewarded -= this.HandleOnAdRewarded;
        adRewardCoins.OnAdClosed -= this.HandleOnRewardedAdClosed;
    }

    #endregion

    AdRequest AdRequestBuild(){
        return new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("CC9DF9C9E1DABC49E2EDEF7455F8800D")
            .TagForChildDirectedTreatment(false)
            .Build();
    }
    void OnDestroy(){
        adRewardCoins.OnAdLoaded -= this.HandleOnRewardedAdLoaded;
        adRewardCoins.OnAdRewarded -= this.HandleOnAdRewarded;
        adRewardCoins.OnAdClosed -= this.HandleOnRewardedAdClosed;
    }

}
