using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TourUploader : MonoBehaviour {
    public Tour tour;
    public TourExporter tour_exporter;

    public int counter, _counter;

    public GameObject load_screen;

    public void UploadFiles(){
        counter = 0;
        _counter = tour.panoramas.Count + tour.interactions.Count + 1;
        StartCoroutine(WaitToLoad());
        foreach(Panorama panorama in tour.panoramas){
            string new_name = "get_panorama?name=" + panorama.id + "&id=" +  tour.id;
            StartCoroutine(
                UploadPanorama(panorama.link, panorama.id)
            );
            panorama.link = new_name;
        }
        foreach (Interaction interaction in tour.interactions)
        {
            if(interaction is Photo){
                string new_name = "get_photo?name=" + interaction.id + "&id=" + tour.id;
                StartCoroutine(
                    UploadPhoto(((Photo)interaction).link, interaction.id)
                );
                ((Photo)interaction).link = new_name;
            }
        }
        StartCoroutine(
            UploadTour(tour_exporter.Export())
        );
    }

    IEnumerator WaitToLoad(){
        load_screen.SetActive(true);
        while (counter < _counter) yield return null;
        load_screen.SetActive(false);
        ScreenController.instance.FinishPostLoad();
    }

    IEnumerator UploadPanorama(string local_file_name, int id) {
        
        WWW localFile = new WWW("file:///" + local_file_name);
        yield return localFile;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", localFile.bytes, id.ToString() + Path.GetExtension(local_file_name));
        
        form.AddField("tour", tour.id);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_panorama", form);
        yield return www.SendWebRequest();

        counter++;
        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }

    IEnumerator UploadPhoto(string local_file_name, int id) {
        
        WWW localFile = new WWW("file:///" + local_file_name);
        yield return localFile;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", localFile.bytes, id.ToString() + Path.GetExtension(local_file_name));
        
        form.AddField("tour", tour.id);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_photo", form);
        yield return www.SendWebRequest();

        counter++;
        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }

    IEnumerator UploadTour(string text) {
        
        WWWForm form = new WWWForm();

        Debug.Log(text);

        form.AddField("text", text);
        form.AddField("tour", tour.id);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_tour", form);
        yield return www.SendWebRequest();

        counter++;
        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }
}
