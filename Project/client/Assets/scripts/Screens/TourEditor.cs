using System;
using System.IO;
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

    private Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {
        float warpFactor = 1.0F;
        Color[] destPix = new Color[targetWidth * targetHeight];
        int y = 0;
        while (y < targetHeight)
        {
            int x = 0;
            while (x < targetWidth)
            {
                float xFrac = x * 1.0F / (targetWidth - 1);
                float yFrac = y * 1.0F / (targetHeight - 1);
                float warpXFrac = Mathf.Pow(xFrac, warpFactor);
                float warpYFrac = Mathf.Pow(yFrac, warpFactor);
                destPix[y * targetWidth + x] = source.GetPixelBilinear(warpXFrac, warpYFrac);
                x++;
            }
            y++;
        }
        source = new Texture2D(targetWidth, targetHeight);
        source.SetPixels(destPix);
        source.Apply();
        return source;
    }

    private Texture2D LoadPanorama(Texture2D photo, string path){
        float fit_to = 2000;
        float height_to = fit_to / 2;

        float prop = fit_to / photo.width;

        int iprop = Mathf.FloorToInt(photo.height * prop);
        int ifit_to = Mathf.FloorToInt(fit_to);
        int iheight_to = Mathf.FloorToInt(height_to);

        Debug.Log(ifit_to + "  -  " + iprop);
        photo = ScaleTexture(photo, ifit_to, iprop);
        Debug.Log(photo.width + "" + photo.height);

        if(photo.height > iheight_to){
            photo.SetPixels(
                photo.GetPixels(0,0, ifit_to, iheight_to)
            );
        }
        else if(photo.height < iheight_to){
            Texture2D new_photo = new Texture2D(ifit_to, iheight_to);
            Color32 resetColor = new Color32(255, 255, 255, 0);
            Color32[] resetColorArray = new_photo.GetPixels32();
            for (int i = 0; i < resetColorArray.Length; i++)
                resetColorArray[i] = resetColor;
            new_photo.SetPixels32(resetColorArray);
            new_photo.Apply();

            for (int x = 0; x < ifit_to; x++){
                for (int y = 0; y < photo.height; y++){
                    new_photo.SetPixel(x, y + iheight_to / 2 - photo.height / 2,
                                       photo.GetPixel(x, y));
                }
            }

            new_photo.Apply();
            photo = new_photo;
        }

        byte[] bytes = photo.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        return photo;
    }

    public void AddPhoto(){
        string new_photo_path = FilePicker.PickImage(-1);

        Texture2D new_photo = null;
        #if !UNITY_EDITOR
        if (new_photo_path != null){
            new_photo = NativeGallery.LoadImageAtPath(new_photo_path, -1, false);

            Vector3 new_pos = new Vector3(editable_tour.panoramas.Count, 0, 0) * panorama_prefab.transform.localScale.x;
            GameObject new_panorama = Instantiate(panorama_prefab, new_pos, Quaternion.identity) as GameObject;
            Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
            new_panorama_panorama.id = editable_tour.panoramas.Count;
            new_panorama_panorama.link = new_photo_path;
            
            new_panorama.GetComponent<Renderer>().material.mainTexture = LoadPanorama(new_photo, new_photo_path);

            GameObject new_preview = Instantiate(preview_prefab, scroll_content) as GameObject;
            Image new_preview_image = new_preview.GetComponent<Image>();

            int axis = Math.Min(new_photo.height, new_photo.width);

            new_preview_image.sprite = Sprite.Create(new_photo, new Rect(0.0f, 0.0f, axis, axis), new Vector2(0.5f, 0.5f));
       

            PanoramaWithPreview new_panorama_with_preview = new_preview.GetComponent<PanoramaWithPreview>();
            panoramas.Add(new_panorama_with_preview);
            new_panorama_with_preview.preview = new_preview_image;
            new_panorama_with_preview.panorama = new_panorama_panorama;
            new_panorama_with_preview.OnPressed += OnPanoramaChosen;

            editable_tour.panoramas.Add(new_panorama_panorama);

            
        }
        #endif
        #if UNITY_EDITOR
        if (true){
            Vector3 new_pos = new Vector3(editable_tour.panoramas.Count, 0, 0) * panorama_prefab.transform.localScale.x;
            GameObject new_panorama = Instantiate(panorama_prefab, new_pos, Quaternion.identity) as GameObject;
            Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
            new_panorama_panorama.id = editable_tour.panoramas.Count;
            new_panorama_panorama.link = new_photo_path;

            GameObject new_preview = Instantiate(preview_prefab, scroll_content) as GameObject;
            Image new_preview_image = new_preview.GetComponent<Image>();

            PanoramaWithPreview new_panorama_with_preview = new_preview.GetComponent<PanoramaWithPreview>();
            panoramas.Add(new_panorama_with_preview);
            new_panorama_with_preview.preview = new_preview_image;
            new_panorama_with_preview.panorama = new_panorama_panorama;
            new_panorama_with_preview.OnPressed += OnPanoramaChosen;

            editable_tour.panoramas.Add(new_panorama_panorama);
        }
        #endif
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

    public void Clear(){
        editable_tour.panoramas.Clear();
        foreach(var pwp in panoramas){
            pwp.OnPressed -= OnPanoramaChosen;
            Destroy(pwp.preview.gameObject);
            Destroy(pwp.panorama.gameObject);
            Destroy(pwp);
        }
    }
}
