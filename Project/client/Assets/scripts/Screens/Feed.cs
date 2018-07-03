using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Feed : AppScreen {
    public Transform scroll_content;
    public int[] posts;

    void Start(){
        StartCoroutine(GetFeed());
    }

    IEnumerator GetFeed(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_feed/" + "2");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            FeedJSON result = JsonUtility.FromJson<FeedJSON>(www.downloadHandler.text);
            posts = result.posts;
        }
    }
}

[Serializable]
public struct FeedJSON{
    public int[] posts;
    public FeedJSON(int[] _posts){
        posts = _posts;
    }
};
