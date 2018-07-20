using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Post : Module {

    public int id;

    public Text name;
    public Text description;
    public Text likes, comments;

    IEnumerator GetPost(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_post/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            PostJSON result = JsonUtility.FromJson<PostJSON>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));

            name.text = result.name;
            description.text = result.description;
            likes.text = result.likes.ToString();
            comments.text = result.comments.ToString();
        }
    }

    public override void Load(){
        StartCoroutine(GetPost());
    }

    public void Open(){
        ScreenController.instance.OpenPost(id);
    }

    public void CreateLike(){
        StartCoroutine(_CreateLike());
    }
    IEnumerator _CreateLike(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/create_like?user_id=" + 1 + "&tour_id=" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Load();
        }
    }
}

[Serializable]
public struct PostJSON{
    public int id;
    public string description;
    public string name;
    public int comments;
    public int likes;
    public string time;
    public string geotag;
    public string tags;
};