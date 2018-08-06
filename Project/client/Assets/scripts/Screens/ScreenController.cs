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
    public OpenedTour opened_tour;

    public Login login;
    public Registrate registrate;

    public AppScreen current_screen;
    public AppScreen previous_screen;

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
    }

    public void GoBack(){
        current_screen.gameObject.SetActive(false);
        previous_screen.gameObject.SetActive(true);
        current_screen = previous_screen;
    }
    public void SwitchScreens(AppScreen a, AppScreen b){
        if (a != null) a.gameObject.SetActive(false);
        if (b != null) b.gameObject.SetActive(true);
    }

    public void GoToLogin(){
        SwitchScreens(current_screen, login);
        previous_screen = current_screen;
        current_screen = login;
        login.Load(); 
    }

    public void GoToRegistration(){
        SwitchScreens(current_screen, registrate);
        previous_screen = current_screen;
        current_screen = registrate;
        registrate.Load();
    }

    public void GoToFeed(){
        SwitchScreens(current_screen, feed);
        previous_screen = current_screen;
        current_screen = feed;
        feed.Load();
    }

    public void GoToSearch(){
        SwitchScreens(current_screen, search);
        previous_screen = current_screen;
        current_screen = search;
    }

    public void OpenPost(int id){
        SwitchScreens(current_screen, opened_post);
        previous_screen = current_screen;
        current_screen = opened_post;
        opened_post.id = id;
        opened_post.Load();
    }

    public void OpenUser(int id){
        SwitchScreens(current_screen, opened_user);
        previous_screen = current_screen;
        current_screen = opened_user;
        if (id == -1) opened_user.id = Server.user_id;
        else opened_user.id = id;
        opened_user.Load();
    }

    public void ViewTour(int id){
        SwitchScreens(current_screen, opened_tour);
        previous_screen = current_screen;
        current_screen = opened_tour;
        opened_tour.id = id;
        opened_tour.Load();
    }

    private void Start(){
        GoToLogin();
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }
}
