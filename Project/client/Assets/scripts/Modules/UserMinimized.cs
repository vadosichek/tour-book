using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserMinimized : Module {
    public Image pic;
    public Text login;
    public Text name;
    public int id;
    public UserMinimizedJSON data;

    public override void Load(){
        id = data.id;
        login.text = data.login;
        name.text = data.name;
        LoadUsr();
    }

    private void LoadUsr(){
        StartCoroutine(_LoadUsr());
    }
    IEnumerator _LoadUsr(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_user?name=thumb_" + data.id)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            pic.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void Open(){
        ScreenController.instance.OpenUser(id);
    }
}

[Serializable]
public struct UserMinimizedJSON{
    public int id;
    public string login;
    public string name;
};
