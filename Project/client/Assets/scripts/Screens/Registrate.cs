using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Registrate : AppScreen {

    IEnumerator _DoRegistrate(string login, string password){
        string path = "/create_profile?login={0}&password={1}&name={2}&bio={3}&url={4}";
        string res = string.Format(path, login, password, "", "", "");
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
        }
    }
}
