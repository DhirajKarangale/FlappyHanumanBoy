using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class LeaderboardsController : MonoBehaviour
{
    public static LeaderboardsController instance;

    private const string LEADERBOARDS_SCORE = "CgkI74_B_vEQEAIQBg";

    void Start(){
        PlayGamesPlatform.Activate();
    }

    void OnLevelWasLoaded(){
        ReportScore(GameController.instance.GetHighScore());
    }

    void Awake(){
        MakeSigleton();
    }

    void MakeSigleton()
    {
        if (instance != null){ Destroy(gameObject);}
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ConnectOrDisconnectOnGooglePlayames(){

        if (Social.localUser.authenticated)  {
            PlayGamesPlatform.Instance.SignOut();

        }
        else{
            Social.localUser.Authenticate((bool success) => {
                if (success) { Debug.Log("Logged In"); }
                else { Debug.Log("Not Logged In"); }
            });
        }
    }

    public void OpenLeaderboardScore(){
        if (Social.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARDS_SCORE);
            Debug.Log("Player is authenticated");
        }
        else { Debug.Log("No Player is not authenticated"); }
    }

    void ReportScore(int score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, LEADERBOARDS_SCORE, (bool success) => {
                if (success) { Debug.Log("Score Reported"); }
                else { Debug.Log("Not Score Reported"); }
            });

        }
    }

}