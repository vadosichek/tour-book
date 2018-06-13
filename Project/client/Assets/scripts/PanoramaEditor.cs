using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaEditor : EditorScreen {
    public override event OnProceed Proceed;
    public override event OnCancel Cancel;


    public Panorama current_photo;
    private Panorama previous_photo;
    private Transform previous_trans;
    public Transform camera;

    public bool editing_transition;

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
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = hit.point;
                        sphere.transform.parent = current_photo.transform;

                        if(editing_transition){
                            editing_transition = false;

                            Vector3 new_pos = new Vector3(previous_trans.position.x - previous_photo.transform.position.x,
                                                          0,
                                                          previous_trans.position.z - previous_photo.transform.position.z);
                            

                            current_photo.transform.position = 2 * new_pos;

                            Vector3 cur_pos = new Vector3(sphere.transform.position.x - current_photo.transform.position.x,
                                                          0,
                                                          sphere.transform.position.z - current_photo.transform.position.z);
                            float degree = Vector3.Angle(new_pos, cur_pos - 2 * new_pos);
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

    public void Select(Panorama photo){
        current_photo = photo;
        camera.position = photo.transform.position;
    }

    public void Finish(){
        Debug.Log("Finish");
        Proceed();
    }

}
