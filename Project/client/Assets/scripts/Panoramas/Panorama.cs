using System;
using UnityEngine;

[Serializable]
public class Panorama : MonoBehaviour {
    public int id;
    public int size;
    public string link;
    public PictureDownloader picture_downloader;

    public void Download(){
        picture_downloader.Load(link);
    }
}
