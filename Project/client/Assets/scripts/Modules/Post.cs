using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Post : Module {
    /// <summary>
    /// module variation
    /// post UI-block
    /// </summary>

    //post id
    public int id;
    //post author id
    public int user_id = -1;
    //if was liked by user
    public bool liked;
    //user name text object
    public Text name;
    //post description object
    public Text description;
    //likes, comments counter objects
    public Text likes, comments;

    //post preview, user pic image objects
    public Image preview, user;
    //black like icon object
    public GameObject filled_like;

    //button to delete post
    public GameObject delete_button;

    //download data from server
    public void LoadPost(){
        StartCoroutine(_LoadPost());
    }
    //downloading coroutine
    IEnumerator _LoadPost(){
        WWWForm form = new WWWForm();

        form.AddField("tour_id", id);
        form.AddField("user_id", Server.user_id);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/get_post", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            PostJSON result = JsonUtility.FromJson<PostJSON>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));

            user_id = result.id;
            name.text = result.name;
            description.text = result.description.Substring(0, Math.Min(40, result.description.Length));
            likes.text = result.likes.ToString();
            comments.text = result.comments.ToString();

            filled_like.SetActive(result.liked);
            liked = result.liked;

            delete_button.SetActive(user_id == Server.user_id);
        }
    }

    //download preview pic from server
    private void LoadPic(){
        StartCoroutine(_LoadPic());
    }
    //downloading coroutine
    IEnumerator _LoadPic(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_panorama?id=" + id + "&name=thumb_" + 0)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            preview.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    //download user pic from server
    private void LoadUsr(){
        StartCoroutine(_LoadUsr());
    }
    //downloading coroutine
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

    //load override -- start all downloading processes
    public override void Load(){
        LoadPost();
        LoadPic();
        LoadUsr();
    }

    //do when user clicks on preview pic
    public void Open(){
        ScreenController.instance.OpenPost(id);
    }
    //do when user clicks on user pic
    public void OpenUser(){
        ScreenController.instance.OpenUser(user_id);
    }
    //do when user clicks on preview pic after post was opened
    public void View(){
        ScreenController.instance.ViewTour(id);
    }

    //like post
    public void CreateLike(){
        if(!liked) StartCoroutine(_CreateLike());
    }
    //upload data coroutine
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

    //delete current post
    public void DeleteTour(){
        StartCoroutine(_DeleteTour());
    }
    //upload data coroutine
    IEnumerator _DeleteTour(){
        Debug.Log("delete");
        WWWForm form = new WWWForm();

        form.AddField("user_id", Server.user_id);
        form.AddField("id", id);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/delete_tour", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            ScreenController.instance.DeletedPost();
        }
    }

    //reset all ui-objects
    public void Clear(){
        preview.sprite = null;
        user.sprite = null;
        name.text = "";
        description.text = "";
        comments.text = "0";
        likes.text = "0";
    }
}

//strcut for post data
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
    public bool liked;
};