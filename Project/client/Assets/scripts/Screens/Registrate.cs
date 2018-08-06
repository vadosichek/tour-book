using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Registrate : AppScreen {
    public InputField login;
    public InputField name;
    public InputField email;
    public InputField password;
    public InputField password_repeat;
    public GameObject error_text;

    public void DoRegistrate(){
        if(password.text.Equals(password_repeat.text)){
            error_text.SetActive(false);
            StartCoroutine(
                _DoRegistrate(login.text, password.text, name.text, "", email.text)
            );
        }
        else{
            error_text.SetActive(true);
        }
    }

    IEnumerator _DoRegistrate(string _login, string _password, string _name, string _bio, string _url){
        string path = "/create_profile?login={0}&password={1}&name={2}&bio={3}&url={4}";
        string res = string.Format(path, _login, _password, _name, _bio, _url);

        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + res);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            int id = int.Parse(www.downloadHandler.text);

            Server.user_id = id;
            Debug.Log(Server.user_id);
            ScreenController.instance.GoToFeed();
        }
    }
}
