using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Post : Module {

    public int id;
    public int user_id = -1;
    public Text name;
    public Text description;
    public Text likes, comments;

    public Image preview, user;


    public void LoadPost(){
        StartCoroutine(_LoadPost());
    }
    IEnumerator _LoadPost(){
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

            user_id = result.id;
            name.text = result.name;
            description.text = result.description;
            likes.text = result.likes.ToString();
            comments.text = result.comments.ToString();
        }
    }

    private void LoadPic(){
        StartCoroutine(_LoadPic());
    }
    IEnumerator _LoadPic(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_panorama?id=" + id + "&name=thumb_" + 0)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            preview.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    private void LoadUsr(){
        StartCoroutine(_LoadUsr());
    }
    IEnumerator _LoadUsr(){
        while (user_id == -1) yield return null;
        Debug.Log(user_id);
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_user?name=thumb_" + user_id)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            user.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    public override void Load(){
        LoadPost();
        LoadPic();
        LoadUsr();
    }

    public void Open(){
        ScreenController.instance.OpenPost(id);
    }
    public void OpenUser(){
        ScreenController.instance.OpenUser(user_id);
    }
    public void View(){
        ScreenController.instance.ViewTour(id);
    }

    public void CreateLike(){
        StartCoroutine(_CreateLike());
    }
    IEnumerator _CreateLike(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", Server.user_id);
        form.AddField("tour_id", id);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/create_like", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            LoadPost();
        }
    }

    public void Clear(){
        preview.sprite = null;
        user.sprite = null;
        name.text = "";
        description.text = "";
        comments.text = "0";
        likes.text = "0";
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