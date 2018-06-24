using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour {
    public TourEditor tour_editor;
    public PanoramaEditor panorama_editor;
    public TourUploader tour_uploader;
    public PostEditor post_editor;

    private void Awake(){
        tour_editor.onEditPhoto += EditPhoto;
        panorama_editor.Proceed += ProceedPhotoEdit;
        panorama_editor.Cancel += ProceedPhotoEdit;
        tour_editor.Proceed += ProceedTourEdit;
        post_editor.Proceed += ProceedPostEdit;
    }
    private void OnDestroy(){
        tour_editor.onEditPhoto -= EditPhoto;
        panorama_editor.Proceed -= ProceedPhotoEdit;
        panorama_editor.Cancel -= ProceedPhotoEdit;
        tour_editor.Proceed -= ProceedTourEdit;
        post_editor.Proceed -= ProceedPostEdit;
    }

    private void EditPhoto(Panorama photo){
        tour_editor.gameObject.SetActive(false);
        panorama_editor.gameObject.SetActive(true);
        panorama_editor.Select(photo);
    }

    private void ProceedPhotoEdit(){
        panorama_editor.gameObject.SetActive(false);
        tour_editor.gameObject.SetActive(true);
    }

    private void ProceedTourEdit(){
        tour_editor.gameObject.SetActive(false);
        post_editor.gameObject.SetActive(true);
        post_editor.CreatePreview();
    }

    private void ProceedPostEdit()
    {
        Debug.Log("Finished");
        tour_uploader.UploadFiles();
    }
}
