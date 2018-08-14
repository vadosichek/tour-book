using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Panorama : MonoBehaviour {
    public int id;
    public int size;
    public string link;

    public void Download(){
        StartCoroutine(_Download());
    }

    IEnumerator _Download(){
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(Server.base_url + "/" + link)){
            yield return www;
            www.LoadImageIntoTexture(tex);
            GetComponent<Renderer>().material.mainTexture = tex;
        }
    }
}
