using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour {
    public static ScreenController instance;

    public TourEditor tour_editor;
    public PanoramaEditor panorama_editor;
    public TourUploader tour_uploader;
    public PostEditor post_editor;

    public Feed feed;
    public Search search;
    public OpenedPost opened_post;
    public User opened_user;
    public UserEditor user_editor;
    public OpenedTour opened_tour;

    public Login login;
    public Registrate registrate;

    public List<AppScreen> previous_screens;

    private void Awake(){
        instance = this;

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
        post_editor.gameObject.SetActive(false);
        GoToFeed();
    }

    public void GoBack(){
        previous_screens[previous_screens.Count - 1].gameObject.SetActive(false);
        previous_screens.RemoveAt(previous_screens.Count - 1);
        previous_screens[previous_screens.Count - 1].gameObject.SetActive(true);
    }
    public void SwitchScreens(AppScreen a, AppScreen b){
        if (a != null) a.gameObject.SetActive(false);
        if (b != null) b.gameObject.SetActive(true);
    }

    public void GoTo(AppScreen to){
        if (previous_screens.Count > 0)
            SwitchScreens(previous_screens[previous_screens.Count - 1], to);
        else
            SwitchScreens(null, to);
        previous_screens.Add(to);
    }

    public void GoToLogin(){
        GoTo(login);
        login.Load(); 
    }

    public void GoToRegistration(){
        GoTo(registrate);
        registrate.Load();
    }

    public void GoToFeed(){
        GoTo(feed);
        feed.Load();
    }

    public void GoToSearch(){
        GoTo(search);
    }

    public void OpenPost(int id){
        GoTo(opened_post);
        opened_post.id = id;
        opened_post.Load();
    }

    public void OpenUser(int id){
        GoTo(opened_user);
        if (id == -1) opened_user.id = Server.user_id;
        else opened_user.id = id;
        opened_user.Load();
    }

    public void OpenUserEditor(){
        GoTo(user_editor);
        user_editor.id = Server.user_id;
        user_editor.Load();
    }

    public void ViewTour(int id){
        GoTo(opened_tour);
        opened_tour.id = id;
        opened_tour.Load();
    }

    public void GoToTourEditor(){
        tour_editor.Clear();
        post_editor.gameObject.SetActive(false);
        GoTo(tour_editor);
    }

    private void Start(){
        GoToLogin();
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }
}
