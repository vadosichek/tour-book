using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchSwitch : MonoBehaviour {

    public GameObject users;
    public GameObject posts;

    public void Switch(){
        posts.SetActive(!posts.active);
        users.SetActive(!users.active);
    } 
}
