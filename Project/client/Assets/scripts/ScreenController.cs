using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour {
    public TourEditor tour_editor;
    public PanoramaEditor panorama_editor;

    private void Awake(){
        tour_editor.onEditPhoto += EditPhoto;
        panorama_editor.Proceed += ProceedPhotoEdit;
        panorama_editor.Cancel += ProceedPhotoEdit;
    }
    private void OnDestroy(){
        tour_editor.onEditPhoto -= EditPhoto;
        panorama_editor.Proceed -= ProceedPhotoEdit;
        panorama_editor.Cancel -= ProceedPhotoEdit;
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

}
