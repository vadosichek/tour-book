using System;
using System.Collections.Generic;
using UnityEngine;

public class TourExporter : MonoBehaviour {

    public Tour tour;
    public string result;

    public GameObject panorama_prefab;
    public GameObject photo_prefab;
    public GameObject transition_prefab;


    public string Export(){
        List<SavedPanorama> saved_panoramas_list = new List<SavedPanorama>();
        foreach(Panorama panorama in tour.panoramas){
            saved_panoramas_list.Add(
                new SavedPanorama(panorama.id, panorama.size, panorama.link, panorama.transform.position, panorama.transform.rotation)
            );
        }

        List<SavedPhoto> saved_photos_list = new List<SavedPhoto>();
        List<SavedTransition> saved_transitions_list = new List<SavedTransition>();
        foreach (Interaction interaction in tour.interactions)
        {
            if(interaction is Photo)
                saved_photos_list.Add(
                    new SavedPhoto(interaction.id, interaction.panorama_id, ((Photo)interaction).link, interaction.transform.position, interaction.transform.rotation)
                );
            else if(interaction is Transition)
                saved_transitions_list.Add(
                    new SavedTransition(interaction.id, interaction.panorama_id, ((Transition)interaction).target_id, interaction.transform.position, interaction.transform.rotation)
                );
        }

        SavedPanoramas saved_panoramas = new SavedPanoramas(saved_panoramas_list);
        SavedPhotos saved_photos = new SavedPhotos(saved_photos_list);
        SavedTransitions saved_transitions = new SavedTransitions(saved_transitions_list);

        SavedTour savedTour = new SavedTour(tour.id, saved_panoramas, saved_photos, saved_transitions);

        return JsonUtility.ToJson(savedTour);
    }

    public void Import(){
        SavedTour savedTour = JsonUtility.FromJson<SavedTour>(result);

        foreach(SavedPanorama saved_panorama in savedTour.saved_panoramas.list){
            GameObject new_panorama = Instantiate(panorama_prefab, saved_panorama.position, saved_panorama.rotation) as GameObject;
            Panorama new_panorama_panorama = new_panorama.GetComponent<Panorama>();
            new_panorama_panorama.id = saved_panorama.id;
            new_panorama_panorama.size = saved_panorama.size;
            new_panorama_panorama.link = saved_panorama.link;
            tour.panoramas.Add(new_panorama_panorama);
            new_panorama_panorama.Download();
        }
        foreach (SavedPhoto saved_photo in savedTour.saved_photos.list)
        {
            GameObject new_photo = Instantiate(photo_prefab, saved_photo.position, saved_photo.rotation) as GameObject;
            Photo new_photo_photo = new_photo.GetComponent<Photo>();
            new_photo_photo.id = saved_photo.id;
            new_photo_photo.panorama_id = saved_photo.panorama_id;
            new_photo_photo.link = saved_photo.link;
            tour.interactions.Add(new_photo_photo);
            new_photo_photo.Download();
        }
        foreach (SavedTransition saved_transition in savedTour.saved_transitions.list)
        {
            GameObject new_transition = Instantiate(transition_prefab, saved_transition.position, saved_transition.rotation) as GameObject;
            Transition new_transition_transition = new_transition.GetComponent<Transition>();
            new_transition_transition.id = saved_transition.id;
            new_transition_transition.panorama_id = saved_transition.panorama_id;
            new_transition_transition.target_id = saved_transition.target_id;
            new_transition_transition.tour = tour;
            tour.interactions.Add(new_transition_transition);
        }

        tour.Move(0);
    }
}

[Serializable]
public struct SavedPanorama{
    public int id;
    public int size;
    public string link;
    public Vector3 position;
    public Quaternion rotation;

    public SavedPanorama(int _id, int _size, string _link, Vector3 _position, Quaternion _rotation){
        id = _id;
        size = _size;
        link = _link;
        position = _position;
        rotation = _rotation;
    }
};
[Serializable]
public struct SavedPanoramas{
    public List<SavedPanorama> list;

    public SavedPanoramas(List<SavedPanorama> _list){
        list = _list;
    }
};

[Serializable]
public struct SavedPhoto{
    public int id;
    public int panorama_id;
    public string link;
    public Vector3 position;
    public Quaternion rotation;

    public SavedPhoto(int _id, int _panorama_id, string _link, Vector3 _position, Quaternion _rotation){
        id = _id;
        panorama_id = _panorama_id;
        link = _link;
        position = _position;
        rotation = _rotation;
    }
};
[Serializable]
public struct SavedPhotos{
    public List<SavedPhoto> list;

    public SavedPhotos(List<SavedPhoto> _list){
        list = _list;
    }
};

[Serializable]
public struct SavedTransition{
    public int id;
    public int panorama_id;
    public int target_id;
    public Vector3 position;
    public Quaternion rotation;

    public SavedTransition(int _id, int _panorama_id, int _target_id, Vector3 _position, Quaternion _rotation){
        id = _id;
        panorama_id = _panorama_id;
        target_id = _target_id;
        position = _position;
        rotation = _rotation;
    }
};
[Serializable]
public struct SavedTransitions{
    public List<SavedTransition> list;

    public SavedTransitions(List<SavedTransition> _list){
        list = _list;
    }
};

[Serializable]
public struct SavedTour{
    public int id;
    public SavedPanoramas saved_panoramas;
    public SavedPhotos saved_photos;
    public SavedTransitions saved_transitions;

    public SavedTour(int _id, SavedPanoramas _saved_panoramas, SavedPhotos _saved_photos, SavedTransitions _saved_transitions){
        id = _id;
        saved_panoramas = _saved_panoramas;
        saved_photos = _saved_photos;
        saved_transitions = _saved_transitions;
    }
};