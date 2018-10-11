﻿using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Photo : Interaction {
    /// <summary>
    /// interaction variation
    /// small picture in the panorama
    /// </summary>

    //path to photo
    public string link;

    //load photo to texture
    public void Load(){
        link = FilePicker.PickImage(-1);

        if (link != null)
            transform.GetComponentInChildren<Renderer>().material.mainTexture = 
                NativeGallery.LoadImageAtPath(link, -1);
    }

    //do nothing on open
    public override void Open(){}

    //download photo from server
    public void Download(){
        StartCoroutine(_Download());
    }

    //coroutine for download
    IEnumerator _Download(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/" + link)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
     
}
