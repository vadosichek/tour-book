using System;
using System.Collections.Generic;
using UnityEngine;

public class Tour : MonoBehaviour{
    /// <summary>
    /// tour data holder
    /// </summary>

    public int id;

    public List<Panorama> panoramas;
    public List<Interaction> interactions;

    public bool editing = false;
    public Transform camera;
    public CameraTrackball trackball;

    public Panorama GetPanoramaById(int id){
        foreach (var panorama in panoramas){
            if (panorama.id == id)
                return panorama;
        }
        return null;
    }

    //transit from one panorama to another
    public void Move(int id){
        Panorama current_photo = GetPanoramaById(id);
        if (id == 0) trackball.ToDefault();
        if (editing){
            FindObjectOfType<PanoramaEditor>().Select(current_photo);
        }
        else{
            camera.position = current_photo.transform.position;
        }
    }
}