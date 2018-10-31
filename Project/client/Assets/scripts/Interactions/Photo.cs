using System.Collections;
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

    public bool opened = false;

    //load photo to texture
    public void Load(){
        link = FilePicker.PickImage(-1);

        if (link != null)
            transform.GetComponentInChildren<Renderer>().material.mainTexture = 
                NativeGallery.LoadImageAtPath(link, -1);
    }

    //resize image on open
    public override void Open(){
        if(opened) transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        else transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        opened = !opened;
    }

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
            transform.GetComponentInChildren<Renderer>().material.mainTexture = tex;
        }
    }
     
}
