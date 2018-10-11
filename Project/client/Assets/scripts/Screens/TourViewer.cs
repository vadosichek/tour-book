using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TourViewer : EditorScreen {
    /// <summary>
    /// editor variation
    /// screen to view panoramas in tour
    /// </summary>

    //tour to load panoramas from
    public Tour tour;
    //tour exporter to download panoramas from server
    public TourExporter tour_exporter;
    public static TourViewer instance;

    void Awake(){
        instance = this;
    }

    //remove old panorama
    public void Clear(){
        foreach (Panorama panorama in tour.panoramas)
            Destroy(panorama.gameObject);
        tour.panoramas.Clear();
        foreach (Interaction interaction in tour.interactions)
            Destroy(interaction.gameObject);
        tour.interactions.Clear();
    }

    //switch to new panorama
    public void View(int id){
        Clear();
        tour.id = id;
        DownloadFiles();
    }

    //download image from server
    public void DownloadFiles(){
        StartCoroutine(DownloadTour());
    }
    //download coroutine
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
