using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private GameObject[] coinScoreHit;
    private float lastCoinX;
    private float coinDistanceMax = 15f;
    private float coinDistanceMin = 4f;

    void Awake() {
        coinScoreHit = GameObject.FindGameObjectsWithTag("CoinScoreHit");

        lastCoinX = coinScoreHit[0].transform.position.x;

        for (int i = 1; i < coinScoreHit.Length; i++){
            if (lastCoinX < coinScoreHit[i].transform.position.x){
                lastCoinX = coinScoreHit[i].transform.position.x;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "CoinScoreHit")
        {
            Vector3 temp = target.transform.position;

            temp.x = lastCoinX + Random.Range(coinDistanceMin, coinDistanceMax);
            target.transform.position = temp;

            lastCoinX = temp.x;
            
        }

    }
}
