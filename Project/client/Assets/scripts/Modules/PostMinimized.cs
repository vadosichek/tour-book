using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PostMinimized : Module {
    public Image image;
    public int id;

    public override void Load(){
        StartCoroutine(LoadPic());
    }

    IEnumerator LoadPic(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_panorama?id=" + id + "&name=thumb_" + 0)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void Open(){
        ScreenController.instance.OpenPost(id);
    }
}