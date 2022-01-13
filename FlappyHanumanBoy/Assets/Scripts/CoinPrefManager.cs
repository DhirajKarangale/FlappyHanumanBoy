using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinPrefManager : MonoBehaviour{

    public static CoinPrefManager instance;

    [Header ("UI references")]
    public static string Coins = "Coins";
    public static int coins = 0;

    [SerializeField] GameObject animatedCoinPrefab;
    [SerializeField] Transform target;

    [Space]
    [Header("Avialable Coins: (coins to pool)")]
    [SerializeField] int maxCoins;
    Queue<GameObject> coinsQueue = new Queue<GameObject> ();

    [Space]
    [Header("Animation Settings")]
    [SerializeField] [Range(0.5f, 0.9f)] float minAnimDuration;
    [SerializeField] [Range(0.9f, 2f)] float maxAnimDuration;

    [SerializeField] Ease easeType;

    Vector3 targetPosition;



    // Start is called before the first frame update
    void Start() {
        coins = PlayerPrefs.GetInt("Coins", 0);

    }

    private void Update()
    {
        coins = Mathf.Clamp(coins,0,int.MaxValue);
    }

    void Awake(){
        MakeSingleton();
        targetPosition = target.position;

        PrepareCoins();
    }

    void MakeSingleton(){
        if (instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void PrepareCoins() {
        GameObject coinl;
        for (int i = 0; i<maxCoins; i++) {
            coinl = Instantiate(animatedCoinPrefab);
            coinl.transform.parent = transform;
            coinl.SetActive(false);
            coinsQueue.Enqueue(coinl);
        }        
    }

    void AnimateCoin(Vector3 collectedCoinPosition, int amount , bool changePos){
        for(int i =0; i<amount; i++) {
            if (coinsQueue.Count > 0) {
                GameObject coinl = coinsQueue.Dequeue();
                coinl.SetActive(true);

                //move coin to collected coin position
                coinl.transform.position = collectedCoinPosition;
               
                Vector3 pos = Vector3.zero;
                if(changePos)
                {
                    pos = collectedCoinPosition;
                }

                Vector3 goPos = targetPosition + pos;
                if(RewardAd.isRewardCoinInGame)
                {
                    goPos = new Vector3(-18.22f, 4.2f, 10) + collectedCoinPosition + targetPosition;
                }
                //animate coin to target position
                float duration = Random.Range (minAnimDuration, maxAnimDuration);
                coinl.transform.DOMove(goPos,duration)
                    .SetEase(easeType)
                    .OnComplete(() =>
                    {
                        coinl.SetActive(false);
                        coinsQueue.Enqueue(coinl);
                       
                    });         

            }
        }

    }

    public void UpdateCoins(Vector3 collectedCoinPosition, int amount, bool changePos){
       
        AnimateCoin(collectedCoinPosition, amount , changePos);
         coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        coins = PlayerPrefs.GetInt("Coins", 0);
        PlayerPrefs.Save();
    }
}
