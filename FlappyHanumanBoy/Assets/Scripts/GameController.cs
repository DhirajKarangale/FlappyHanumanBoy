using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public const float WatingTimeForSpinWheel = 924000.0f;

    private const string HIGH_SCORE = "High Score";
    private const string SELECTED_BIRD = "Selected Bird";
    private const string SELECTED_WORLD = "Selected World";


    void Awake(){
        MakeSingleton();
        IsTheGameStartedForFirstTime();
        //   PlayerPrefs.DeleteAll();
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


    void IsTheGameStartedForFirstTime(){
        if (!PlayerPrefs.HasKey("IsTheGameStartedForFirstTime")){
            Debug.Log("Not First Time ");
            PlayerPrefs.SetInt(HIGH_SCORE, 0);
            PlayerPrefs.SetInt(SELECTED_BIRD, 0);
            PlayerPrefs.SetInt("IsTheGameStartedForFirstTime", 0);
            PlayerPrefs.SetInt(SELECTED_WORLD, 0);
        }
    }

    public void SetSelectedBird(int selectedBird){
        PlayerPrefs.SetInt(SELECTED_BIRD, selectedBird);
    }
    public int GetSelectedBird(){
        return PlayerPrefs.GetInt(SELECTED_BIRD);
    }

    public void SetSelectedWorld(int selectedWorld) {
        PlayerPrefs.SetInt(SELECTED_WORLD, selectedWorld);
    }
    public int GetSelectedWorld(){
        return PlayerPrefs.GetInt(SELECTED_WORLD);
    }

    public void SetHighScore(int score){
        PlayerPrefs.SetInt(HIGH_SCORE, score);
    }
    public int GetHighScore(){
        return PlayerPrefs.GetInt(HIGH_SCORE);
    }

}
