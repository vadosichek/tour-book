using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class User : AppScreen {
    /// <summary>
    /// appScreen variation
    /// user profile screen
    /// </summary>

    public Transform scroll_content;
    //post prefab
    public GameObject post;
    public int[] posts;
    public int id;

    public string login;
    public string url;

    //if user subbed
    public bool subbed;

    //user data text objects
    public Text name;
    public Text bio;
    public Text subscriptions;
    public Text subscribers;
    public Text tours;
    public Image pic;

    //buttons to interact with profile
    public GameObject sub, desub, edit;

    //clear old data
    private void HeaderReset(){
        name.text = "";
        bio.text = "";
        subscriptions.text = "";
        subscribers.text = "";
        tours.text = "";
        pic.sprite = null;
    }

    //load user data from server
    public override void Load(){
        HeaderReset();
        StartCoroutine(GetUser());
        StartCoroutine(GetPosts());
        StartCoroutine(LoadPic());
    }
    //download coroutines
    IEnumerator GetUser(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", id);
        form.AddField("viewer_id", Server.user_id);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/get_profile", form);
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
            login = result.login;
            url = result.url;

            if(id != Server.user_id){
                subbed = result.subbed;
                sub.SetActive(!subbed);
                desub.SetActive(subbed);
                edit.SetActive(false);
            }
            else{
                edit.SetActive(true);
                sub.SetActive(false);
                desub.SetActive(false);
            }
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
            int axis = Math.Min(tex.height, tex.width);
            pic.sprite = Sprite.Create(tex, new Rect(tex.width/2 - axis/2, tex.height/2 - axis/2, axis, axis), new Vector2(0.5f, 0.5f));
        }
    }

    //instantiate post objects
    void GeneratePosts(){
        Clear();
        foreach(int id in posts){
            GameObject created_post = Instantiate(post, scroll_content) as GameObject;
            PostMinimized created_post_post = created_post.GetComponent<PostMinimized>();
            created_post_post.id = id;
            created_post_post.Load();
        }
    }

    //remove old posts
    void Clear(){
        foreach (Transform child in scroll_content)
            if(child.gameObject.GetComponent<PostMinimized>() != null) Destroy(child.gameObject);
    }

    public void Subscribe(){
        if(!subbed) StartCoroutine(_Subscribe());
    }
    IEnumerator _Subscribe(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", id);
        form.AddField("subscriber_id", Server.user_id);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/create_subscription", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            subbed = true;
            sub.SetActive(!subbed);
            desub.SetActive(subbed);
        }
    }

    public void Unsubscribe(){
        if (subbed) StartCoroutine(_Unsubscribe());
    }
    IEnumerator _Unsubscribe(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", id);
        form.AddField("subscriber_id", Server.user_id);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/delete_subscription", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            subbed = false;
            sub.SetActive(!subbed);
            desub.SetActive(subbed);
        }
    }

    public void Edit(){
        ScreenController.instance.OpenUserEditor();
    }
}

//struct for user data
[Serializable]
public struct UserJSON{
    public int id;
    public string login;
    public string name;
    public string bio;
    public string url;
    public int subscriptions;
    public int subscribers;
    public int tours;
    public bool subbed;
};

//struct for posts
[Serializable]
public struct UserPostsJSON{
    public int[] posts;
};