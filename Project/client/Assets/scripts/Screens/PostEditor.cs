using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PostEditor : EditorScreen {
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;

    public Tour tour;
    public Sprite preview;
    public Image preview_image;
    public string description, tags, location;
    public Text description_text, tags_text, location_text;

    private string path = "/create_tour?user_id={0}&geotag={1}&desc={2}&tags={3}&time={4}";

    public void Finish(){
        description = description_text.text;
        tags = tags_text.text;
        location = location_text.text;
        Upload();
    }

    private void Upload()
    {
        string new_path = Server.base_url + string.Format(path, 1, location, description, tags, System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        StartCoroutine(GetRequest(new_path));
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            if(int.TryParse(uwr.downloadHandler.text, out tour.id)){
                Proceed();
            }
        }
    }

    public void CreatePreview(){
        Texture2D photo = NativeGallery.LoadImageAtPath(
            tour.panoramas[0].link
            , -1);
        preview = Sprite.Create(photo, new Rect(0.0f, 0.0f, photo.height, photo.height), new Vector2(0.5f, 0.5f));
        preview_image.sprite = preview;
    }
}
