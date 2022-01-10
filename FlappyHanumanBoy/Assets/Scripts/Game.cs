using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    #region SIngleton:Game

    public static Game Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] Text[] allCoinsUIText;


    public int Shopcoins;

    void Start() 
    {
        UpdateAllCoinsUiText();
    }

    public void UseCoins (int amount)
    {
        Shopcoins -= amount;
    }

    public bool HasEnoughCoins(int amount)
    {
        return (Shopcoins >= amount);
    }

    public void UpdateAllCoinsUiText()
    {
        for(int i = 0; i < allCoinsUIText.Length; i++) {
            allCoinsUIText[i].text = Shopcoins.ToString();
        }
    }
}
