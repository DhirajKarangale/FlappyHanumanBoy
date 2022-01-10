using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AppContainer : MonoBehaviour{

    [System.Serializable]
    public class AppTemplate{
        public string appImage;
        public string appName;
        public string appDescription;
        public string appLink;

    }

    AppTemplate[] appTemplate;

    [SerializeField] Transform AppItemScrollView;
    [SerializeField] GameObject AppIconUITemplate;

    Button AppClickButton;

    [SerializeField] GameObject OpenAppList;
    [SerializeField] GameObject CloseAppList;
    [SerializeField] GameObject AppListPanel;



    // Start is called before the first frame update
    void Start()
    {
        appTemplate = JsonHelper.GetArray<AppTemplate>(JsonFileReader.LoadJsonAsResource("Json/app_list.json"));
        AddAppTemplateToModal();
    }


    void AddAppTemplateToModal() {

      //  GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int N = appTemplate.Length;

        for (int i = 0; i < N; i++) {
            g = Instantiate(AppIconUITemplate, AppItemScrollView);

            g.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("AppImages/"+ appTemplate[i].appImage.ToString());
            g.transform.GetChild(1).GetComponent<Text>().text = appTemplate[i].appName;
            g.transform.GetChild(2).GetComponent<Text>().text = appTemplate[i].appDescription;

            g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

       // Destroy(buttonTemplate);
    }

    void ItemClicked(int itemIndex) {
        Debug.Log("app Index: "+ itemIndex);
        string url = appTemplate[itemIndex].appLink;
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + url);
    }


    public void CloseAppListPanel() { AppListPanel.SetActive(false); }
    public void OpenAppListPanel() { AppListPanel.SetActive(true); }
    
}

public static class ButtonClickExtension{

    public static void AddEventListner<T> (this Button button, T param, Action<T> OnClick) {

        button.onClick.AddListener(delegate (){
            OnClick(param);
        });
    }
    }