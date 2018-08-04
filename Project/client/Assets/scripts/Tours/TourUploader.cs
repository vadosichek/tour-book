using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TourUploader : MonoBehaviour {
    public Tour tour;
    public TourExporter tour_exporter;

    public void UploadFiles(){
        foreach(Panorama panorama in tour.panoramas){
            string new_name = "get_panorama?name=" + panorama.id + "&id=" +  tour.id;
            StartCoroutine(
                UploadPanorama(panorama.link)
            );
            panorama.link = new_name;
        }
        foreach (Interaction interaction in tour.interactions)
        {
            if(interaction is Photo){
                string new_name = "get_photo?name=" + interaction.id + "&id=" + tour.id;
                StartCoroutine(
                    UploadPhoto(((Photo)interaction).link)
                );
                ((Photo)interaction).link = new_name;
            }
        }
        StartCoroutine(
            UploadTour(tour_exporter.Export())
        );
    }

    IEnumerator UploadPanorama(string local_file_name) {
        
        WWW localFile = new WWW("file:///" + local_file_name);
        yield return localFile;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", localFile.bytes);
        
        form.AddField("tour", 0);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_panorama", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }

    IEnumerator UploadPhoto(string local_file_name) {
        
        WWW localFile = new WWW("file:///" + local_file_name);
        yield return localFile;

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", localFile.bytes);
        
        form.AddField("tour", 0);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_photo", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }

    IEnumerator UploadTour(string text) {
        
        WWWForm form = new WWWForm();
        //form.AddBinaryData("file", text); // binary?
        
        form.AddField("tour", 0);

        UnityWebRequest www = UnityWebRequest.Post(Server.base_url + "/upload_tour", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }
        else{
            Debug.Log("success");
        }
    }
}
