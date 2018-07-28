using UnityEngine;

public class Photo : Interaction {

    public string link;

    public void Load(){
        link = FilePicker.PickImage(-1);

        if (link != null)
            transform.GetComponentInChildren<Renderer>().material.mainTexture = 
                NativeGallery.LoadImageAtPath(link, -1);
    }

    public override void Open()
    {
        
    }

    public PictureDownloader picture_downloader;

    public void Download()
    {
        picture_downloader.LoadTexture(link);
    }
}
