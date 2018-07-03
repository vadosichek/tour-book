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

    void Start(){
        StartCoroutine(GetPost());
    }

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

    public PostJSON(int _id, string _description, string _name, int _comments, int _likes, string _time, string _geotag, string _tags){
        id = _id;
        description = _description;
        name = _name;
        comments = _comments;
        likes = _likes;
        time = _time;
        geotag = _geotag;
        tags = _tags;
    }
};