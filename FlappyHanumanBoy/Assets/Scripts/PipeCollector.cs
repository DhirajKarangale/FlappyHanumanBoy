using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeCollector : MonoBehaviour
{

    private GameObject[] pipeHolders;
    private float distance = 3.2f;
    private float lastPipesX;
    private float pipeMin = -1.7f;
    private float pipeMax = 1.7f;

    void Awake(){
        pipeHolders = GameObject.FindGameObjectsWithTag("PipeHolder");

        for (int i = 0; i < pipeHolders.Length; i++) {
            Vector3 temp = pipeHolders[i].transform.position;
            temp.y = Random.Range(pipeMin, pipeMax);
            pipeHolders[i].transform.position = temp;
        }

        lastPipesX = pipeHolders[0].transform.position.x;

        for (int i = 1; i < pipeHolders.Length; i++) {
            if(lastPipesX < pipeHolders[i].transform.position.x) {
                lastPipesX = pipeHolders[i].transform.position.x;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D target)  {
        if (target.tag == "PipeHolder") {
            Vector3 temp = target.transform.position;
           
            temp.x = lastPipesX + distance;
            temp.y = Random.Range(pipeMin, pipeMax);
            target.transform.position = temp;

            lastPipesX = temp.x;
        }
    }


}
