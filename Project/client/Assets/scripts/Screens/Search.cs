using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Search : AppScreen {
    public string key;
    public GameObject post_minimized_prefab;
    public Transform posts_scroll_content;
    public GameObject user_minimized_prefab;
    public Transform users_scroll_content;

    void Start()
    {
        StartCoroutine(GetPosts());
    }

    IEnumerator GetPosts()
    {
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/search_post/" + key);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            PostsJson result = JsonUtility.FromJson<PostsJson>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));
            foreach(int i in result.posts){
                GameObject new_post = Instantiate(post_minimized_prefab, posts_scroll_content);
                new_post.GetComponent<PostMinimized>().id = i;
            }
        }
    }

    IEnumerator GetUsers()
    {
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/search_user/" + key);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            UsersJson result = JsonUtility.FromJson<UsersJson>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));
        }
    }

}

[Serializable]
public struct PostsJson{
    public int[] posts;
};

[Serializable]
public struct UsersJson{
    public UserMinimizedJSON[] users;
};