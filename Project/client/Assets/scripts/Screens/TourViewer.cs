using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourViewer : EditorScreen {

    public Tour tour;
    public TourUploader tour_uploader;

    public void View(int id){
        foreach (Panorama panorama in tour.panoramas)
            Destroy(panorama.gameObject);
        foreach (Interaction interaction in tour.interactions)
            Destroy(interaction.gameObject);
        
        tour.id = id;
        tour_uploader.DownloadFiles();
    }
}
