using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TourEditor : EditorScreen{
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;


    public delegate void EditPhoto(Panorama photo);
    public event EditPhoto onEditPhoto;

    public Transform scroll_content;
    public GameObject preview_prefab, panorama_prefab;


    //public struct PanoramaWithPreview{
    //    public Image preview;
    //    public Panorama panorama;

    //    public PanoramaWithPreview(Image new_preview, Panorama new_panorama){
    //        preview = new_preview;
    //        panorama = new_panorama;
    //    }
    //};
    public List<PanoramaWithPreview> panoramas = new List<PanoramaWithPreview>();

    public int count = 0;

    private bool deleting = false;

    public void AddPhoto(){
        string new_photo_path = PickImage(-1);

        Texture2D new_photo = null;
        if (new_photo_path != null)
            new_photo = NativeGallery.LoadImageAtPath(new_photo_path, -1);

        Vector3 new_pos = new Vector3(count, 0, 0) * panorama_prefab.transform.localScale.x;
        GameObject new_panorama = Instantiate(panorama_prefab, new_pos, Quaternion.identity) as GameObject;
        Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
        new_panorama_panorama.id = count;
        new_panorama_panorama.link = new_photo_path;
        new_panorama.GetComponent<Renderer>().material.mainTexture = new_photo;

        GameObject new_preview = Instantiate(preview_prefab, scroll_content) as GameObject;
        Image new_preview_image = new_preview.GetComponent<Image>();
        //Button new_preview_button = new_preview.GetComponent<Button>();

        #region Platfotm
            #if !UNITY_EDITOR
                new_preview_image.sprite = Sprite.Create(new_photo, new Rect(0.0f, 0.0f, new_photo.height, new_photo.height), new Vector2(0.5f, 0.5f));
            #endif
        #endregion

        //PanoramaWithPreview new_panorama_with_preview = new PanoramaWithPreview(new_preview_image, new_panorama_panorama);
        PanoramaWithPreview new_panorama_with_preview = new_preview.GetComponent<PanoramaWithPreview>();
        new_panorama_with_preview.preview = new_preview_image;
        new_panorama_with_preview.panorama = new_panorama_panorama;

        panoramas.Add(new_panorama_with_preview);
        new_panorama_with_preview.OnPressed += OnPanoramaChosen;

        //new_preview_button.onClick.AddListener(() => { onEditPhoto(new_panorama_panorama); });
        count++;
    }

    public void RemovePhoto(){
        deleting = true;
    }

    private void OnPanoramaChosen(PanoramaWithPreview pwp){
        if(deleting){
            pwp.OnPressed -= OnPanoramaChosen;
            Destroy(pwp.preview.gameObject);
            Destroy(pwp.panorama.gameObject);
            Destroy(pwp);
            deleting = false;
        }
    }

    private string PickImage(int maxSize){
        Texture2D texture = null;
        string result = null;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) => {
            result = path;
        }, "Select a PNG image", "image/png", maxSize);
        return result;
    }
}
