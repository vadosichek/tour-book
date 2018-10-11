using System;
using UnityEngine;

[Serializable]
public class Interaction : MonoBehaviour {
    /// <summary>
    /// Base interaction class
    /// </summary>

    //interaction id in tour
    public int id;

    //panorama id in tour
    public int panorama_id;

    //base open action
    public virtual void Open(){
        
    }

    //open on mouse click
    private void OnMouseDown(){
        Open();
    }
}
