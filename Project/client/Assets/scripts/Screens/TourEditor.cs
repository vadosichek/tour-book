using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourEditor : EditorScreen{
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;

    public delegate void EditPhoto(Panorama photo);
    public event EditPhoto onEditPhoto;

    public Tour editable_tour;

    public Transform scroll_content;
    public GameObject preview_prefab, panorama_prefab;

    public List<PanoramaWithPreview> panoramas = new List<PanoramaWithPreview>();

    private bool deleting = false;

    private void Start(){
        editable_tour.editing = true;
    }

    public void AddPhoto(){
        string new_photo_path = FilePicker.PickImage(-1);

        Texture2D new_photo = null;
        if (new_photo_path != null)
            new_photo = NativeGallery.LoadImageAtPath(new_photo_path, -1);

        Vector3 new_pos = new Vector3(editable_tour.panoramas.Count, 0, 0) * panorama_prefab.transform.localScale.x;
        GameObject new_panorama = Instantiate(panorama_prefab, new_pos, Quaternion.identity) as GameObject;
        Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
        new_panorama_panorama.id = editable_tour.panoramas.Count;
        new_panorama_panorama.link = new_photo_path;
        new_panorama.GetComponent<Renderer>().material.mainTexture = new_photo;

        GameObject new_preview = Instantiate(preview_prefab, scroll_content) as GameObject;
        Image new_preview_image = new_preview.GetComponent<Image>();

        #region Platfotm
            #if !UNITY_EDITOR
                new_preview_image.sprite = Sprite.Create(new_photo, new Rect(0.0f, 0.0f, new_photo.height, new_photo.height), new Vector2(0.5f, 0.5f));
            #endif
        #endregion

        PanoramaWithPreview new_panorama_with_preview = new_preview.GetComponent<PanoramaWithPreview>();
        new_panorama_with_preview.preview = new_preview_image;
        new_panorama_with_preview.panorama = new_panorama_panorama;
        new_panorama_with_preview.OnPressed += OnPanoramaChosen;

        editable_tour.panoramas.Add(new_panorama_panorama);
    }

    public void RemovePhoto(){
        deleting = true;
    }

    private void OnPanoramaChosen(PanoramaWithPreview pwp){
        if(deleting){
            editable_tour.panoramas.Remove(pwp.panorama);
            pwp.OnPressed -= OnPanoramaChosen;
            Destroy(pwp.preview.gameObject);
            Destroy(pwp.panorama.gameObject);
            Destroy(pwp);
            deleting = false;
        }
        else{
            onEditPhoto(pwp.panorama);
        }
    }

    public void Finish(){
        Proceed();
    }
}
