﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaEditor : EditorScreen {
    /// <summary>
    /// editor variation
    /// panorama editor -- create interactions & photos
    /// </summary>

    public override event OnProceed Proceed;
    public override event OnCancel Cancel;

    //tour to edit
    public Tour editable_tour;

    //panoramas in tour
    public Panorama current_photo;
    private Panorama previous_photo;
    //transition in previous panorama
    private Transition previous_trans;

    public Transform camera;

    public GameObject transition_prefab;
    // true when transition is edited
    public bool editing_transition;

    public GameObject photo_prefab;


    public void AddTransition(){
        StartCoroutine(EditingTransition());
    }
    private IEnumerator EditingTransition(){
        while (true){
            if (Input.GetMouseButtonDown(0)){
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit)){

                    if (hit.rigidbody != null){
                        //create new transition object
                        GameObject sphere = Instantiate(transition_prefab) as GameObject;
                        sphere.transform.position = hit.point; //move it
                        sphere.transform.LookAt(current_photo.transform.position); //rotate it
                        sphere.transform.parent = current_photo.transform;

                        Transition new_transition = sphere.GetComponent<Transition>();
                        new_transition.tour = editable_tour;

                        new_transition.id = editable_tour.interactions.Count; //set interaction id
                        new_transition.panorama_id = current_photo.id; //save panorama id
                        editable_tour.interactions.Add(new_transition);

                        if(editing_transition){ //when 2 transitions were created
                            editing_transition = false;

                            Vector3 new_pos = new Vector3(previous_trans.transform.position.x - previous_photo.transform.position.x,
                                                          0,
                                                          previous_trans.transform.position.z - previous_photo.transform.position.z);
                            

                            current_photo.transform.position = new Vector3(3 * new_pos.x, current_photo.transform.position.y, 3 * new_pos.z); //move connected panorama

                            Vector3 cur_pos = new Vector3(sphere.transform.position.x - current_photo.transform.position.x,
                                                          0,
                                                          sphere.transform.position.z - current_photo.transform.position.z);
                            float degree = Vector3.Angle(new_pos, cur_pos - 3 * new_pos);
                            current_photo.transform.Rotate(new Vector3(0, degree, 0)); //rotate it

                            //save id-s
                            previous_trans.target_id = current_photo.id; 
                            new_transition.target_id = previous_photo.id;

                            Select(previous_photo); //goto prev. photo
                        }
                        else{ //when one transiton was created and second one is going to be created
                            editing_transition = true;
                            previous_trans = new_transition;
                            previous_photo = current_photo;
                            Finish();
                        }

                        break;
                    }
                }
            }
            yield return null;
        }
    }

    public void AddPhoto(){
        StartCoroutine(EditingPhoto());
    }
    private IEnumerator EditingPhoto()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {

                    if (hit.rigidbody != null)
                    {
                        Debug.Log(hit.point);

                        //create new photo object
                        GameObject sphere = Instantiate(photo_prefab) as GameObject;
                        sphere.transform.position = hit.point; //move it
                        sphere.transform.LookAt(current_photo.transform.position); //totate it
                        sphere.transform.parent = current_photo.transform;

                        Photo new_photo = sphere.GetComponent<Photo>();

                        new_photo.id = editable_tour.interactions.Count; //set interaction id
                        new_photo.panorama_id = current_photo.id; //save panorama id
                        editable_tour.interactions.Add(new_photo);
                        new_photo.Load();

                        break;
                    }
                }
            }
            yield return null;
        }
    }

    public void Select(Panorama photo){
        current_photo = photo;
        camera.position = photo.transform.position;
    }

    public void Finish(){
        Proceed();
    }

}
