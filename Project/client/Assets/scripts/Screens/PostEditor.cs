using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PostEditor : EditorScreen {
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;

    public Tour tour;
    public Sprite preview;
    public Image preview_image;
    public string description, tags, location;
    public Text description_text, tags_text, location_text;

    public void Finish(){
        description = description_text.text;
        tags = tags_text.text;
        location = location_text.text;
        Upload();
    }

    private void Upload(){
        StartCoroutine(_Upload());
    }

    IEnumerator _Upload(){
        WWWForm form = new WWWForm();

        form.AddField("user_id", Server.user_id);
        form.AddField("geotag", location);
        form.AddField("desc", description);
        form.AddField("tags", tags);
        form.AddField("time", System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest uwr = UnityWebRequest.Post(Server.base_url + "/create_tour", form);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError){
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else{
            Debug.Log("Received: " + uwr.downloadHandler.text);

            if(int.TryParse(uwr.downloadHandler.text, out tour.id)){
                Proceed();
            }
        }
    }

    public void CreatePreview(){
        Texture2D photo = NativeGallery.LoadImageAtPath(
            tour.panoramas[0].link
            , -1);
        int axis = Math.Min(photo.height, photo.width);
        preview = Sprite.Create(photo, new Rect(0.0f, 0.0f, axis, axis), new Vector2(0.5f, 0.5f));
        preview_image.sprite = preview;
    }
}
