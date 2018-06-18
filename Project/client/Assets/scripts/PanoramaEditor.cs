using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaEditor : EditorScreen {
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;

    public Tour editable_tour;

    public Panorama current_photo;
    private Panorama previous_photo;
    private Transform previous_trans;
    public Transform camera;

    public GameObject transition_prefab;
    public bool editing_transition;

    public GameObject photo_prefab;

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
            Finish();
    }

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
                        Debug.Log(hit.point);
                        GameObject sphere = Instantiate(transition_prefab) as GameObject;
                        sphere.transform.position = hit.point;
                        sphere.transform.LookAt(current_photo.transform.position);
                        sphere.transform.parent = current_photo.transform;
                        editable_tour.interactions.Add(sphere.GetComponent<Interaction>());
                        if(editing_transition){
                            editing_transition = false;

                            Vector3 new_pos = new Vector3(previous_trans.position.x - previous_photo.transform.position.x,
                                                          0,
                                                          previous_trans.position.z - previous_photo.transform.position.z);
                            

                            current_photo.transform.position = 3 * new_pos;

                            Vector3 cur_pos = new Vector3(sphere.transform.position.x - current_photo.transform.position.x,
                                                          0,
                                                          sphere.transform.position.z - current_photo.transform.position.z);
                            float degree = Vector3.Angle(new_pos, cur_pos - 3 * new_pos);
                            current_photo.transform.Rotate(new Vector3(0, degree, 0));

                            Select(previous_photo);
                        }
                        else{
                            editing_transition = true;
                            previous_trans = sphere.transform;
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
                        GameObject sphere = Instantiate(photo_prefab) as GameObject;
                        sphere.transform.position = hit.point;
                        sphere.transform.LookAt(current_photo.transform.position);
                        sphere.transform.parent = current_photo.transform;
                        editable_tour.interactions.Add(sphere.GetComponent<Interaction>());
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
        Debug.Log("Finish");
        Proceed();
    }

}
