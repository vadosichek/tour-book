using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanoramaWithPreview : MonoBehaviour {
    /// <summary>
    /// panorama with preview ui-block
    /// </summary>

    //event to call when pressed
    public delegate void Pressed(PanoramaWithPreview pwp);
    public event Pressed OnPressed;

    //preview pic image object
    public Image preview;
    //panorama object ref
    public Panorama panorama;

    //contructor
    public PanoramaWithPreview(Image new_preview, Panorama new_panorama){
        preview = new_preview;
        panorama = new_panorama;
    }

    //call when pressed
    public void Press(){
        OnPressed(this);
    }
}
