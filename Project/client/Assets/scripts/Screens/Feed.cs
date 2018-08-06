using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Feed : AppScreen {
    public Transform scroll_content;
    public GameObject post;
    public int[] posts;

    public override void Load(){
        StartCoroutine(GetFeed());
    }

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

    void GeneratePosts(){
        Clear();
        foreach(int id in posts){
            GameObject created_post = Instantiate(post, scroll_content) as GameObject;
            Post created_post_post = created_post.GetComponent<Post>();
            created_post_post.id = id;
            created_post_post.Load();
        }
    }

    void Clear(){
        foreach (Transform child in scroll_content)
            Destroy(child.gameObject);
    }

}

[Serializable]
public struct FeedJSON{
    public int[] posts;
};
