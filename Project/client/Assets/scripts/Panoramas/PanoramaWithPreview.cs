using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanoramaWithPreview : MonoBehaviour {
    public delegate void Pressed(PanoramaWithPreview pwp);
    public event Pressed OnPressed;

    public Image preview;
    public Panorama panorama;

    public PanoramaWithPreview(Image new_preview, Panorama new_panorama)
    {
        preview = new_preview;
        panorama = new_panorama;
    }

    public void Press(){
        OnPressed(this);
    }
}
