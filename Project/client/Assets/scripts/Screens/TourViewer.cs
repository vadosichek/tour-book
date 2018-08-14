using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TourViewer : EditorScreen {

    public Tour tour;
    public TourExporter tour_exporter;

    public void View(int id){
        foreach (Panorama panorama in tour.panoramas)
            Destroy(panorama.gameObject);
        tour.panoramas.Clear();
        foreach (Interaction interaction in tour.interactions)
            Destroy(interaction.gameObject);
        tour.interactions.Clear();
        
        tour.id = id;
        DownloadFiles();
    }

    public void DownloadFiles(){
        StartCoroutine(DownloadTour());
    }

    IEnumerator DownloadTour(){
        UnityWebRequest www = UnityWebRequest.Get(Server.base_url + "/get_tour?tour=" + tour.id);
        Debug.Log(Server.base_url + "/get_tour?tour=" + tour.id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(www.downloadHandler.text);
            tour_exporter.result = www.downloadHandler.text;
            tour_exporter.Import();
        }
    }
}
