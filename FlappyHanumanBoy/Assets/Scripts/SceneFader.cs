using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;


    [SerializeField]
    private GameObject fadeCanvas;

    [SerializeField]
    private Animator fadeAnim;

    // Start is called before the first frame update
    void Awake()
    {
        MakeSingleton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeSingleton() {
        if (instance != null) {
            Destroy(gameObject);
        }else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FadeIn(string levelName) {
        StartCoroutine(FadeInAnimation(levelName));
    }

    public void FadeOut() {
        StartCoroutine(FadeOutAnimation());
    }

    IEnumerator FadeInAnimation(string levelName) {
        fadeCanvas.SetActive(true);
        fadeAnim.Play("FadeIn");
        yield return StartCoroutine(MyCoroutine.WaitForRealSeconds(0.7f));
        SceneManager.LoadScene(levelName);
        FadeOut();
       }
    IEnumerator FadeOutAnimation(){
        fadeAnim.Play("FadeOut");
        yield return StartCoroutine(MyCoroutine.WaitForRealSeconds(1f));
        fadeCanvas.SetActive(false);
    }

}
