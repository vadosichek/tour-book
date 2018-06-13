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


    public struct PanoramaWithPreview{
        public Image preview;
        public Panorama panorama;

        public PanoramaWithPreview(Image new_preview, Panorama new_panorama){
            preview = new_preview;
            panorama = new_panorama;
        }
    };
    public List<PanoramaWithPreview> panoramas = new List<PanoramaWithPreview>();

    public int count = 0;

    public void AddPhoto(){
        Texture2D new_photo = PickImage(-1);

        Vector3 new_pos = new Vector3(count, 0, 0) * panorama_prefab.transform.localScale.x;
        count++;
        GameObject new_panorama = Instantiate(panorama_prefab, new_pos, Quaternion.identity) as GameObject;
        Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
        new_panorama.GetComponent<Renderer>().material.mainTexture = new_photo;

        GameObject new_preview = Instantiate(preview_prefab, scroll_content) as GameObject;
        Image new_preview_image = new_preview.GetComponent<Image>();
        Button new_preview_button = new_preview.GetComponent<Button>();

        #region Platfotm
            #if !UNITY_EDITOR
                new_preview_image.sprite = Sprite.Create(new_photo, new Rect(0.0f, 0.0f, new_photo.height, new_photo.height), new Vector2(0.5f, 0.5f));
            #endif
        #endregion

        PanoramaWithPreview new_panorama_with_preview = new PanoramaWithPreview(new_preview_image, new_panorama_panorama);
        panoramas.Add(new_panorama_with_preview);

        new_preview_button.onClick.AddListener(() => { onEditPhoto(new_panorama_panorama); });
    }

    private Texture2D PickImage(int maxSize){
        Texture2D texture = null;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) => {
            if (path != null){
                texture = NativeGallery.LoadImageAtPath(path, maxSize);
            }
        }, "Select a PNG image", "image/png", maxSize);
        return texture;
    }
}
