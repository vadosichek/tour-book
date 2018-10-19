using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : AppScreen {
    /// <summary>
    /// appScreen vatiation
    /// login screen
    /// </summary>

    //login input field object
    public InputField login;
    //password input field object
    public InputField password;
    //error text object
    public GameObject error_text;
    //login button object
    public Button login_button;

    bool onScreen = false;

    //enable login only when some data is written
    void Update(){
        if(onScreen){
            if (login.text.Length == 0 || password.text.Length == 0) login_button.interactable = false;
            else login_button.interactable = true;
        }
    }

    //login action
    public void DoLogin(){
        StartCoroutine(
            _DoLogin(login.text, password.text)
        );
    }

    //server interaction coroutine
    IEnumerator _DoLogin(string _login, string _password){
        WWWForm form = new WWWForm();

        form.AddField("login", _login);
        form.AddField("password", _password);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/login", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
            error_text.SetActive(true);
            error_text.GetComponent<Text>().text = www.error;
        }
        else{
            Debug.Log(www.downloadHandler.text);
            int id = int.Parse(www.downloadHandler.text);
            if(id == -1 || id == -2){
                error_text.SetActive(true);
            }
            else{
                onScreen = false;
                PlayerPrefs.SetString("login", _login);
                PlayerPrefs.SetString("password", _password);
                Server.user_id = id;
                error_text.SetActive(false);
                Debug.Log(Server.user_id);
                ScreenController.instance.GoToFeed();
            }
        }
    }

    //load override -- check if credits saved & login
    public override void Load(){
        onScreen = true;
        if (PlayerPrefs.HasKey("login") && PlayerPrefs.HasKey("password")){
            login.text = PlayerPrefs.GetString("login", "");
            password.text = PlayerPrefs.GetString("password", "");
            StartCoroutine(
                _DoLogin(login.text, password.text)
            );
        }
    }

}
