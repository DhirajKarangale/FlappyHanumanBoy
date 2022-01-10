using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DailyWheelController : MonoBehaviour
{

    private static DailyWheelController instance;

    [SerializeField]
    private Button coinHolderButton, closeAddCoinButton;

    [SerializeField]
    private GameObject addCoinPanel;


    void Awake(){
        MakeInstance();
    }

    void MakeInstance(){
        if (instance == null){
            instance = this;
        }
    }

    public void ShowCoinAddPanel(){
        addCoinPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void HideCoinAddPanel(){
        addCoinPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoBackToMenu(){
        SceneFader.instance.FadeIn("CustomMenu");
    }

}
