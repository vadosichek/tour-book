using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaDownloader : MonoBehaviour {

    public string url;

    void Start()
    {
        PickImage(-1);
    }

    void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                GetComponent<Renderer>().material.mainTexture = texture;

                Destroy(texture, 20f);
            }
        }, "Select a PNG image", "image/png", maxSize);

        Debug.Log("Permission result: " + permission);
    }
}
