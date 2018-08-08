using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : AppScreen {
    public InputField login;
    public InputField password;
    public GameObject error_text;

    public void DoLogin(){
        StartCoroutine(
            _DoLogin(login.text, password.text)
        );
    }

    IEnumerator _DoLogin(string _login, string _password){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/login?login=" + _login + "&password=" + _password);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            int id = int.Parse(www.downloadHandler.text);
            if(id == -1){
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
