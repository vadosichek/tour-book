using System;
using System.Collections.Generic;
using UnityEngine;

public class Tour : MonoBehaviour
{
    public int id;

    public List<Panorama> panoramas;
    public List<Interaction> interactions;

    public bool editing = false;
    public Transform camera;

    public void Start()
    {
        panoramas = new List<Panorama>();
        interactions = new List<Interaction>();
    }

    public Panorama GetPanoramaById(int id)
    {
        foreach (var panorama in panoramas)
        {
            if (panorama.id == id)
                return panorama;
        }
        return null;
    }

    public void Move(int id)
    {
        Panorama current_photo = GetPanoramaById(id);
        if (editing)
        {
            FindObjectOfType<PanoramaEditor>().Select(current_photo);
        }
        else
        {
            camera.position = current_photo.transform.position;
        }
    }
}