using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Feed : AppScreen {
    public ScrollRect scroll;
    public Transform scroll_content;
    public GameObject post;
    public int[] posts;

    float lTime;

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

    void Start(){
        scroll.onValueChanged.AddListener(ScrollListener);
    }

    public void ScrollListener(Vector2 value){
        if (value.y > 1 && Time.time - lTime > 3){
            Load();
            lTime = Time.time;
        }
    }

}

[Serializable]
public struct FeedJSON{
    public int[] posts;
};
