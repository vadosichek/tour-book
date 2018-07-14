using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class OpenedPost : AppScreen {

    public int id;

    public Transform scroll_content;
    public GameObject comment_prefab;

    public Post opened_post;

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

    public override void Load(){
        Clear();
        opened_post.id = id;
        opened_post.Load();
        StartCoroutine(GetComments());
    }

    void Clear(){
        foreach (Transform child in scroll_content){
            if(child.GetComponent<Comment>() != null)
                Destroy(child.gameObject);
        }
    }
}

[Serializable]
public struct CommentsJSON
{
    public CommentJSON[] comments;
};

[Serializable]
public struct CommentJSON
{
    public string user_name;
    public string text;
};