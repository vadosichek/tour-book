using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class User : AppScreen {
    public Transform scroll_content;
    public GameObject post;
    public int[] posts;
    public int id;

    public Text name;
    public Text bio;
    public Text subscriptions;
    public Text subscribers;
    public Text tours;
    public Image pic;

    public override void Load(){
        StartCoroutine(GetUser());
        StartCoroutine(GetPosts());
        StartCoroutine(LoadPic());
    }

    IEnumerator GetUser(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_profile/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            UserJSON result = JsonUtility.FromJson<UserJSON>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));
            name.text = result.name;
            bio.text = result.bio;
            subscribers.text = result.subscribers.ToString();
            subscriptions.text = result.subscriptions.ToString();
            tours.text = result.tours.ToString();
        }
    }

    IEnumerator GetPosts(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_posts/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            UserPostsJSON result = JsonUtility.FromJson<UserPostsJSON>(www.downloadHandler.text);
            posts = result.posts;
            GeneratePosts();
        }
    }

    IEnumerator LoadPic(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_user?name=" + id)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            pic.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    void GeneratePosts(){
        Clear();
        foreach(int id in posts){
            GameObject created_post = Instantiate(post, scroll_content) as GameObject;
            PostMinimized created_post_post = created_post.GetComponent<PostMinimized>();
            created_post_post.id = id;
            created_post_post.Load();
        }
    }

    void Clear(){
        foreach (Transform child in scroll_content)
            if(child.gameObject.GetComponent<PostMinimized>() != null) Destroy(child.gameObject);
    }

    public void Subscribe(){
        StartCoroutine(_Subscribe());
    }
    IEnumerator _Subscribe(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/create_subscription?user_id=" + id + "&subscriber_id=" + Server.user_id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
        }
    }
}

[Serializable]
public struct UserJSON{
    public int id;
    public string name;
    public string bio;
    public string url;
    public int subscriptions;
    public int subscribers;
    public int tours;
};

[Serializable]
public struct UserPostsJSON{
    public int[] posts;
};