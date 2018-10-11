using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PostMinimized : Module {
    /// <summary>
    /// module variation
    /// button with preview pic
    /// </summary>

    ///preview image object
    public Image image;
    //post id
    public int id;

    //load override -- load preview pic from server
    public override void Load(){
        StartCoroutine(LoadPic());
    }
    //downloading coroutine
    IEnumerator LoadPic(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/get_panorama?id=" + id + "&name=thumb_" + 0)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.height, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    //do when user clicks
    public void Open(){
        ScreenController.instance.OpenPost(id);
    }
}