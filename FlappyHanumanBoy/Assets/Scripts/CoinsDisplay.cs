using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CoinsDisplay : MonoBehaviour
{
    private Text cointext;

    // Start is called before the first frame update
    void Start(){
        cointext = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update(){
        cointext.text = CoinPrefManager.coins.ToString();
    }
}
