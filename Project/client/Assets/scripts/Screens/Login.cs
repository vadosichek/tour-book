using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : AppScreen {
    public InputField login;
    public InputField password;
    public GameObject error_text;
    public Button login_button;

    public void DoLogin(){
        StartCoroutine(
            _DoLogin(login.text, password.text)
        );
    }

    void Update(){
        if (login.text.Length == 0 || password.text.Length == 0) login_button.interactable = false;
        else login_button.interactable = true;
    }

    IEnumerator _DoLogin(string _login, string _password){
        WWWForm form = new WWWForm();

        form.AddField("login", _login);
        form.AddField("password", _password);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/login", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            int id = int.Parse(www.downloadHandler.text);
            if(id == -1 || id == -2){
                error_text.SetActive(true);
            }
            else{
                PlayerPrefs.SetString("login", _login);
                PlayerPrefs.SetString("password", _password);
                Server.user_id = id;
                error_text.SetActive(false);
                Debug.Log(Server.user_id);
                ScreenController.instance.GoToFeed();
            }
        }
    }

    public override void Load(){
        if (PlayerPrefs.HasKey("login") && PlayerPrefs.HasKey("password")){
            login.text = PlayerPrefs.GetString("login", "");
            password.text = PlayerPrefs.GetString("password", "");
            StartCoroutine(
                _DoLogin(login.text, password.text)
            );
        }
    }

}
