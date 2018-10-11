using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class OpenedPost : AppScreen {
    /// <summary>
    /// appScreen variation
    /// opened post screen - post & comments
    /// </summary>

    //post id
    public int id;
    public Transform scroll_content;
    public GameObject comment_prefab;

    //opened post object
    public Post opened_post;
    //comment text input field
    public InputField input;

    //download data coroutine
    IEnumerator GetComments()
    {
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_comments/" + id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            CommentsJSON result = JsonUtility.FromJson<CommentsJSON>(www.downloadHandler.text);
            Debug.Log(JsonUtility.ToJson(result));
            foreach(CommentJSON comment in result.comments){
                Comment new_comment = Instantiate(comment_prefab, scroll_content).GetComponent<Comment>();
                new_comment.SetValues(comment.user_name, comment.text);
                new_comment.Load();
            }
        }
    }

    //reset screen
    private void UpdatePost(){
        Clear();
        opened_post.LoadPost();
        StartCoroutine(GetComments());
    }

    //load override -- download data from server
    public override void Load(){
        Clear();
        opened_post.Clear();
        opened_post.id = id;
        opened_post.Load();
        StartCoroutine(GetComments());
    }

    //delete comments
    void Clear(){
        foreach (Transform child in scroll_content){
            if(child.GetComponent<Comment>() != null)
                Destroy(child.gameObject);
        }
    }

    //upload comment
    public void SendComment(){
        if(input.text.Length > 0) StartCoroutine(_SendComment());
    }
    //upload coroutine
    IEnumerator _SendComment(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", Server.user_id);
        form.AddField("tour_id", id);
        form.AddField("text", input.text);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/create_comment", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            input.text = "";
            UpdatePost();
        }
   }
}

//struct to save comments
[Serializable]
public struct CommentsJSON{
    public CommentJSON[] comments;
};

//struct to save comment data
[Serializable]
public struct CommentJSON{
    public string user_name;
    public string text;
};