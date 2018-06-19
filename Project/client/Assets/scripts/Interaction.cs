using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    public int id;
    public int panorama_id;

    public virtual void Open(){
        
    }

    private void OnMouseDown(){
        Open();
    }
}
