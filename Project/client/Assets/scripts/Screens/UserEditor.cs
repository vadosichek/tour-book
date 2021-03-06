﻿using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UserEditor : AppScreen {
    /// <summary>
    /// appScreen variation
    /// edit user data screen
    /// </summary>

    //user id
    public int id;
    //user data ui-objects
    public Image pic;
    public InputField login;
    public InputField name;
    public InputField email;
    public InputField desc;

    Texture2D new_pic;
    string new_photo_path;

    public User user_screen;

    //choose new profile pic
    public void LoadNewPic(){
        new_photo_path = FilePicker.PickImage(-1);

        new_pic = null;
        if (new_photo_path != null){
            new_pic = NativeGallery.LoadImageAtPath(new_photo_path, -1);
            int axis = Math.Min(new_pic.height, new_pic.width);
            pic.sprite = Sprite.Create(new_pic, new Rect(new_pic.width / 2 - axis / 2, new_pic.height / 2 - axis / 2, axis, axis), new Vector2(0.5f, 0.5f));

        }
    }
    //upload coroutine
    IEnumerator UploadPic() {
        WWW localFile = new WWW("file:///" + new_photo_path);
        yield return localFile;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", localFile.bytes, id.ToString() + Path.GetExtension(new_photo_path));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_user", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }

    //finish editing
    public void Send(){
        if (new_pic != null) StartCoroutine(UploadPic());
        StartCoroutine(_Send());
    }
    //upload new data coroutine
    IEnumerator _Send(){
        WWWForm form = new WWWForm();

        form.AddField("id", id);
        form.AddField("login", login.text);
        form.AddField("name", name.text);
        form.AddField("bio", desc.text);
        form.AddField("url", email.text);
        form.AddField("password", PlayerPrefs.GetString("password", ""));

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/update_user", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            ScreenController.instance.GoBack();
        }
    }

    //load user data into fields
    public override void Load(){
        login.text = user_screen.login;
        name.text = user_screen.name.text;
        desc.text = user_screen.bio.text;
        email.text = user_screen.url;
        pic.sprite = user_screen.pic.sprite;
    }
}