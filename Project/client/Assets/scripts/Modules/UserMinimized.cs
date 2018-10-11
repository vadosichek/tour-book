using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserMinimized : Module {
    /// <summary>
    /// module variation
    /// </summary>

    //user pic object
    public Image pic;
    //user login text object
    public Text login;
    //user name text object
    public Text name;
    //user id
    public int id;
    //struct to store user data
    public UserMinimizedJSON data;


    //load override -- load user data
    public override void Load(){
        id = data.id;
        login.text = data.login;
        name.text = data.name;
        LoadUsr();
    }

    //download user data from server
    private void LoadUsr(){
        StartCoroutine(_LoadUsr());
    }
    //downloading coroutine
    IEnumerator _LoadUsr(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_user?name=thumb_" + data.id)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            pic.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    //do when user clicks on user pic
    public void Open(){
        ScreenController.instance.OpenUser(id);
    }
}

//struct for minimized user data
[Serializable]
public struct UserMinimizedJSON{
    public int id;
    public string login;
    public string name;
};
