using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Feed : AppScreen {
    /// <summary>
    /// appScreen vatiation
    /// feed screen (main screen)
    /// </summary>

    //scroll ui-object
    public ScrollRect scroll;
    //all posts' parent object
    public Transform scroll_content;
    //post prefab
    public GameObject post;
    //feed (posts' ids)
    public int[] posts;

    //load override -- download feed
    public override void Load(){
        StartCoroutine(GetFeed());
    }
    //downloading coroutine
    IEnumerator GetFeed(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_feed/" + Server.user_id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            FeedJSON result = JsonUtility.FromJson<FeedJSON>(www.downloadHandler.text);
            posts = result.posts;
            GeneratePosts();
        }
    }

    //instantiate post objects for all posts in feed
    void GeneratePosts(){
        Clear();
        foreach(int id in posts){
            GameObject created_post = Instantiate(post, scroll_content) as GameObject;
            Post created_post_post = created_post.GetComponent<Post>();
            created_post_post.id = id;
            created_post_post.Load();
        }
    }

    //remove old posts
    void Clear(){
        foreach (Transform child in scroll_content)
            Destroy(child.gameObject);
    }

}


//struct for feed data
[Serializable]
public struct FeedJSON{
    public int[] posts;
};
