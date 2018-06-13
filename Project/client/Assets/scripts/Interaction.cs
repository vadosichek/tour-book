using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    public int id;
    public int type;
    public bool editable;
    public string link;

    private void Open(){
        
    }

    private void OnMouseDown(){
        Open();
    }
}
